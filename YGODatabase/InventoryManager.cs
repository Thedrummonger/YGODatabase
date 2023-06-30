using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class InventoryManager : Form
    {
        MainInterface _DatabaseForm;

        private DuplicateCardContainer? selectedCard = null;

        public List<CardCollection> Collections = new List<CardCollection>();
        public int CurrentCollectionInd;

        public Dictionary<Guid, List<Collection>> UndoLists = new Dictionary<Guid, List<Collection>>();
        public Dictionary<Guid, List<Collection>> RedoLists = new Dictionary<Guid, List<Collection>>();

        public Dictionary<Guid, string> DeckPathDictionary = new Dictionary<Guid, string>();

        public InventoryManager(MainInterface DatabaseForm)
        {
            _DatabaseForm = DatabaseForm;
            LoadInventoryAndDecks();
            InitializeComponent();
            cmbAddTo.DataSource = CategoryNames.Select(x => new ComboBoxItem { DisplayName = x.Value, tag = x.Key }).ToArray();
        }

        private void LoadInventoryAndDecks()
        {
            var InventoryFile = YGODataManagement.GetInventoryFilePath();
            var DeckFolder = YGODataManagement.GetDeckDirectoryPath();
            CardCollection Inventory = null;
            if (File.Exists(InventoryFile))
            {
                try { Inventory = JsonConvert.DeserializeObject<CardCollection>(File.ReadAllText(InventoryFile)); } catch { Inventory = null; }
            }
            if (Inventory == null)
            {
                Inventory = new CardCollection() { data = new Dictionary<Guid, InventoryDatabaseEntry>(), LastEdited = DateTime.Now, Name = "Inventory" };
                File.WriteAllText(InventoryFile, JsonConvert.SerializeObject(Inventory, Formatting.Indented));
            }
            foreach (var i in Inventory.data) { i.Value.Category = Categories.MainDeck; }
            Collections.Add(Inventory);

            if (!Directory.Exists(DeckFolder)) { Directory.CreateDirectory(DeckFolder); }
            foreach(var i in Directory.GetFiles(DeckFolder))
            {
                try
                {
                    CardCollection Deck = JsonConvert.DeserializeObject<CardCollection>(File.ReadAllText(i));
                    Collections.Add(Deck);
                    DeckPathDictionary.Add(Deck.UUID, i);
                } 
                catch { Debug.WriteLine($"{Path.GetFileName(i)} Was not a valid deck"); }
            }
        }

        #region Search Functions
        private void UpdateSearchResults(object sender, EventArgs e)
        {
            PrintSearchResults();
        }
        private void PrintSearchResults()
        {
            bool NameSearch = cmbFilterBy.SelectedIndex == 0 || cmbFilterBy.SelectedIndex == 2;
            bool CodeSearch = cmbFilterBy.SelectedIndex == 1 || cmbFilterBy.SelectedIndex == 2;
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                lbSearchResults.DataSource =  new List<string>() { "Search for card to add" };
                return;
            }

            HashSet<string> MatchedDisplayName = new HashSet<string>();
            List<CardSearchResult> Formattedresults = new List<CardSearchResult>();

            foreach (var i in YGODataManagement.MasterDataBase.data)
            {
                if (i.card_sets == null || !i.card_sets.Any()) { continue; }
                foreach (var j in i.card_sets)
                {
                    string DisplayName = $"{i.name}";
                    if (chkShowRarity.Checked) { DisplayName += $" {j.GetRarityCode()}"; }
                    if (chkShowSet.Checked) { DisplayName += $" ({j.set_name})"; }
                    if (CodeSearch) { DisplayName = $"({j.set_code}) {DisplayName}"; }

                    if (MatchedDisplayName.Contains(DisplayName)) { continue; }

                    if (SearchParser.CardMatchesFilter(DisplayName, i, j, txtSearch.Text, NameSearch, CodeSearch))
                    {
                        MatchedDisplayName.Add(DisplayName);
                        Formattedresults.Add(new CardSearchResult { DisplayName = DisplayName, Card = i, Set = j, FilteringRarity = chkShowRarity.Checked || CodeSearch, FilteringSet = chkShowSet.Checked || CodeSearch });
                    }
                }
            }

            lbSearchResults.DataSource = Formattedresults;
        }
        private void MoveSearchResultListBox(int pos)
        {
            int CurrentPos = lbSearchResults.SelectedIndex;
            int NewPos = CurrentPos + pos;
            if (NewPos < 0 || NewPos >= lbSearchResults.Items.Count) { return; }
            lbSearchResults.SelectedIndex = NewPos;
        }
        private void lbSearchResults_DoubleClick(object sender, EventArgs e)
        {
            AddSelectedCard();
        }
        private void AddSelectedCard()
        {
            if (lbSearchResults.SelectedIndex < 0) { return; }
            if (lbSearchResults.SelectedItem is not CardSearchResult SelectedCard) { return; }
            txtSearch.SelectAll();
            txtSearch.Focus();

            Categories SelectedCategory = Categories.MainDeck;
            if (cmbAddTo.SelectedItem is ComboBoxItem addToSelection)
            {
                SelectedCategory = (Categories)addToSelection.tag;
            }

            Guid UUID = Guid.NewGuid();

            string? ForceSet = SelectedCard.FilteringSet ? SelectedCard.Set.set_code : null;
            string? ForceRarity = SelectedCard.FilteringRarity ? SelectedCard.Set.set_rarity : null;

            InventoryDatabaseEntry Template = new InventoryDatabaseEntry { cardID = SelectedCard.Card.id, set_code = ForceSet, set_rarity = ForceRarity };
            var BestSetMatch = SmartCardSetSelector.GetBestSetPrinting(Template, Collections, Collections[CurrentCollectionInd], out int ImageIndex);

            if (BestSetMatch == null) 
            {
                MessageBox.Show($"ERROR: selected card was invalid. This is a bug.\n\n{SelectedCard.Card.name} | {ForceSet??"Any Set"} | {ForceSet??"Any Rarity"}" );
                return;
            }
            Debug.WriteLine($"Adding to collection {SelectedCard.Card.name} | {BestSetMatch.set_name} | {BestSetMatch.set_rarity}");

            Collections[CurrentCollectionInd].data.Add(UUID, new InventoryDatabaseEntry()
            {
                cardID = SelectedCard.Card.id,
                set_code = BestSetMatch.set_code,
                set_rarity = BestSetMatch.set_rarity,
                Category = CurrentCollectionInd == 0 ? Categories.None : SelectedCategory,
                Condition = SafeGetDefaultCondition(),
                ImageIndex = ImageIndex < 0 ? 0 : ImageIndex,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], UUID);
            PrintSelectedCard("Last Added Card");

            PrintInventory();
            UpdatePopoutForms(false);

        }

        #endregion Search Functions

        #region Selected Item

        bool SelectedCardUpdating = false;
        private void PrintSelectedCard(string Source, int AmountToEdit = 1)
        {
            SelectedCardUpdating = true;

            bool DisableControls = selectedCard == null;
            gbSelectedCard.Enabled = !DisableControls;
            if (DisableControls)
            {
                gbSelectedCard.Text = "N/A";
                lblSelectedCard.Text = "N/A";
                Utility.ManageNUD(numericUpDown1, 0, 0, 0);
                Utility.ManageNUD(numericUpDown2, 0, 0, 0);
                cmbSelctedCardRarity.DataSource = null;
                cmbSelectedCardSet.DataSource = null;
                cmbSelectedCardCondition.DataSource = null;
                SelectedCardUpdating = false;
                return;
            }
            var InventoryObject = selectedCard.InvData;
            var Card = Utility.GetCardByID(InventoryObject.cardID);
            var SetEntry = Utility.GetExactCard(Card, InventoryObject.set_code, InventoryObject.set_rarity);

            gbSelectedCard.Text = Source;
            lblSelectedCard.Text = $"{Card.name}";

            cmbSelectedCardSet.DataSource = Card.GetAllSetsContainingCard();
            foreach (var i in cmbSelectedCardSet.Items) { if (i.ToString() == SetEntry.set_name) { cmbSelectedCardSet.SelectedItem = i; break; } }
            cmbSelctedCardRarity.DataSource = Card.GetAllRaritiesInSet(SetEntry.set_name);
            foreach (var i in cmbSelctedCardRarity.Items) { if (i.ToString() == SetEntry.set_rarity) { cmbSelctedCardRarity.SelectedItem = i; break; } }
            cmbSelectedCardCondition.DataSource = BulkData.Conditions.Keys.ToArray();
            foreach (var i in cmbSelectedCardCondition.Items) { if (i.ToString() == InventoryObject.Condition) { cmbSelectedCardCondition.SelectedItem = i; break; } }
            cmbCollectedCardCategory.DataSource = CategoryNames.Select(x => new ComboBoxItem { DisplayName = x.Value, tag = x.Key }).ToArray();
            foreach (ComboBoxItem i in cmbCollectedCardCategory.Items) { if ((Categories)i.tag == InventoryObject.Category) { cmbCollectedCardCategory.SelectedItem = i; break; } }

            var IdenticalCards = selectedCard.Entries.Count;
            if (AmountToEdit > IdenticalCards) { AmountToEdit = IdenticalCards; }
            Utility.ManageNUD(numericUpDown1, 1, IdenticalCards, AmountToEdit);
            Utility.ManageNUD(numericUpDown2, 1, Card.card_images.Length, InventoryObject.ImageIndex + 1);

            SelectedCardUpdating = false;

        }
        private void SelectedCardValueEdited(object sender, EventArgs e)
        {
            if (SelectedCardUpdating) { return; }

            string Rarity = (string)cmbSelctedCardRarity.SelectedItem;
            string Set = (string)cmbSelectedCardSet.SelectedItem;
            string Condition = (string)cmbSelectedCardCondition.SelectedItem;
            int Image = (int)numericUpDown2.Value - 1;
            Categories Category = (Categories)((ComboBoxItem)cmbCollectedCardCategory.SelectedItem).tag;

            Guid[] SelectedCards = selectedCard.Entries.ToArray().Reverse().ToArray();
            Guid[] CardsToEdit = SelectedCards.Take((int)numericUpDown1.Value).ToArray();

            foreach(var i in CardsToEdit)
            {
                EditCard(i, Rarity, Set, Condition, sender == cmbSelectedCardSet, Category, Image);
            }

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], CardsToEdit.First());

            SaveCollection(Collections[CurrentCollectionInd]);

            PrintSelectedCard(gbSelectedCard.Text, (int)numericUpDown1.Value);
            PrintInventory();
            UpdatePopoutForms(false);
            UpdatepictureBox(selectedCard.CardData(), selectedCard.InvData.ImageIndex);
        }

        private void EditCard(Guid UUID, string NewRarity, string NewSetName, string NewCondition, bool EditingSet, Categories NewCategory, int ImageIndex)
        {
            var InventoryObject = Collections[CurrentCollectionInd].data[UUID];
            var Card = Utility.GetCardByID(InventoryObject.cardID);
            var OldSetEntry = Card.card_sets.First(x => x.set_code == InventoryObject.set_code && x.set_rarity == InventoryObject.set_rarity);

            if (EditingSet)
            {
                var ValidRaritiesForNewSet = Card.GetAllRaritiesInSet(NewSetName);
                if (!ValidRaritiesForNewSet.Contains(OldSetEntry.set_rarity))
                {
                    NewRarity = ValidRaritiesForNewSet.First();
                }
            }
            var NewSetEntry = Card.card_sets.First(x => x.set_name == NewSetName && x.set_rarity == NewRarity);

            InventoryObject.LastUpdated = DateTime.Now;
            InventoryObject.set_rarity = NewSetEntry.set_rarity;
            InventoryObject.set_code = NewSetEntry.set_code;
            InventoryObject.Condition = NewCondition;
            InventoryObject.Category = NewCategory;
            InventoryObject.ImageIndex = ImageIndex;
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            List<Guid> SelectedCards = selectedCard.Entries;
            List<Guid> RemovedCards = new List<Guid>();

            for (var i = (int)numericUpDown1.Value - 1; i >= 0; i--)
            {
                Collections[CurrentCollectionInd].data.Remove(SelectedCards[i]);
                RemovedCards.Add(SelectedCards[i]);
            }
            var RemainingInventory = SelectedCards.Where(x => !RemovedCards.Contains(x)).ToList();
            selectedCard = RemainingInventory.Any() ? Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], RemainingInventory.Last()) : null;

            SaveCollection(Collections[CurrentCollectionInd]);
            PrintSelectedCard(gbSelectedCard.Text);
            PrintInventory();
            UpdatePopoutForms(false);
        }

        private void btnAddOneSelected_Click(object sender, EventArgs e)
        {
            Guid UUID = Guid.NewGuid();

            var CurrentCard = selectedCard.InvData;

            //Only using the DuplicateCardContainer class for it's ability to clone and InventoryDatabaseEntry 
            DuplicateCardContainer TempNewInvObjectContainer = new();
            TempNewInvObjectContainer.InvData = CurrentCard.Clone();
            TempNewInvObjectContainer.InvData.DateAdded = DateTime.Now;
            TempNewInvObjectContainer.InvData.LastUpdated = DateTime.Now;

            Collections[CurrentCollectionInd].data.Add(UUID, TempNewInvObjectContainer.InvData);

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], UUID);
            PrintSelectedCard("Last Added Card");

            PrintInventory();
            UpdatePopoutForms(false);
        }

        #endregion Selected Item

        #region FormFunctions
        private void InventoryManager_Load(object sender, EventArgs e)
        {
            CurrentCollectionInd = 0;
            cmbOrderBy.SelectedIndex = _DatabaseForm.Settings.InventoryShowCollectionOrderBy >= cmbOrderBy.Items.Count ? 0 : _DatabaseForm.Settings.InventoryShowCollectionOrderBy;
            cmbFilterBy.SelectedIndex = _DatabaseForm.Settings.InventorySearchBy >= cmbFilterBy.Items.Count ? 0 : _DatabaseForm.Settings.InventorySearchBy;
            chkShowRarity.Checked = _DatabaseForm.Settings.InventorySearchShowRarity;
            chkShowSet.Checked = _DatabaseForm.Settings.InventorySearchShowSet;
            chkInvDescending.Checked = _DatabaseForm.Settings.InventoryShowCollectionDescending;
            DefaultConditionSelectComboBox.Items.Clear();
            foreach (var i in BulkData.Conditions.Keys) 
            { 
                DefaultConditionSelectComboBox.Items.Add(i);
                if (_DatabaseForm.Settings.DefaultCondition == i) { DefaultConditionSelectComboBox.SelectedItem = i; }
            }

            UpdateCollectionsList();
            comboBox1.SelectedIndex = 0;

        }
        private void InventoryManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            SaveData();
            _DatabaseForm.Settings.InventorySearchShowRarity = chkShowRarity.Checked;
            _DatabaseForm.Settings.InventorySearchShowSet = chkShowSet.Checked;
            _DatabaseForm.Settings.InventoryShowCollectionOrderBy = cmbOrderBy.SelectedIndex;
            _DatabaseForm.Settings.InventorySearchBy = cmbFilterBy.SelectedIndex;
            _DatabaseForm.Settings.DefaultCondition = SafeGetDefaultCondition();
            _DatabaseForm.Settings.InventoryShowCollectionDescending = chkInvDescending.Checked;
        }
        private void CaptureKeyPress(object sender, KeyEventArgs e)
        {
            if (txtSearch.Focused)
            {
                if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true;
                    MoveSearchResultListBox(-1);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true;
                    MoveSearchResultListBox(1);
                }
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }
        private void CaptureTabPress(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab || e.KeyData == Keys.Enter)
            {
                AddSelectedCard();
                e.IsInputKey = true;
            }
        }
        private void HighlightCard(object sender, EventArgs e)
        {
            int ImageIndex = 0;
            YGOCardOBJ Card;
            if (sender == lbSearchResults && lbSearchResults.SelectedItem is CardSearchResult SearchSelectedCard)
            {
                Card = SearchSelectedCard.Card;
            }
            else if (sender == listView1 && listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is DuplicateCardContainer InventorySelectedCard)
            {
                Card = InventorySelectedCard.CardData();
                ImageIndex = InventorySelectedCard.InvData.ImageIndex;
            }
            else
            {
                return;
            }
            UpdatepictureBox(Card, ImageIndex);
        }
        private async void UpdatepictureBox(YGOCardOBJ card, int ImageIndex)
        {
            await Task.Run(() => pictureBox1.Image = YGODataManagement.GetImage(card, ImageIndex));
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    ShowContextMenu(sender, focusedItem);
                }
            }
        }
        private void InventoryManager_ResizeEnd(object sender, EventArgs e)
        {
            Utility.ResizeLowerListBox(listView1, this);
        }

        #endregion FormFunctions

        #region Inventory Display
        private void InventorySearchUpdated(object sender, EventArgs e)
        {
            PrintInventory();
        }
        private void PrintInventory()
        {
            InventoryDisplay.PrintInventoryData(listView1, gbCurrentCollection, Collections, CurrentCollectionInd, txtInventoryFilter.Text, cmbOrderBy.SelectedIndex, chkInvDescending.Checked);
        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1 || 
                listView1.SelectedItems[0] is null || 
                listView1.SelectedItems[0].Tag is null || 
                listView1.SelectedItems[0].Tag is not DuplicateCardContainer Data) 
            { return; }

            selectedCard = Data;
            PrintSelectedCard("Selected Card");
        }
        private void btnImportCards_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Select YDK File",
                DefaultExt = "ydk",
                Filter = "ydk files (*.ydk)|*.ydk"
            };
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }

            AddYDKToCollection(Collections[CurrentCollectionInd], File.ReadAllLines(openFileDialog1.FileName));

            LoadCollection(CurrentCollectionInd);
        }
        private void ShowContextMenu(object sender, ListViewItem SelectedEntry)
        {
            if (SelectedEntry.Tag is null) { return; }

            ContextMenuStrip contextMenu = new();
            ToolStripItem RefreshContextItem = contextMenu.Items.Add("Refresh");
            RefreshContextItem.Click += (sender, e) => { PrintInventory(); };

            if (SelectedEntry.Tag is DuplicateCardContainer inventoryObject)
            {
                ToolStripItem SelectCard = contextMenu.Items.Add("Select Card");
                SelectCard.Click += (sender, e) => { selectedCard = inventoryObject; PrintSelectedCard("Selected Card"); };

                if (SelectedEntry.BackColor == Color.LightPink)
                {
                    ToolStripItem ShowAltPrintings = contextMenu.Items.Add("Show other available printings");
                    ShowAltPrintings.Click += (sender, e) => { ShowOtherAvailablePrinting(inventoryObject); };
                }
                ToolStripItem ShowOtherdecks = contextMenu.Items.Add("Show other decks using card");
                ShowOtherdecks.Click += (sender, e) => { ShowOtherDecksUsingCard(inventoryObject); };
            }
            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(Cursor.Position);
            }
        }

        private void ShowOtherDecksUsingCard(DuplicateCardContainer inventoryObject)
        {
            Dictionary<Guid, Tuple<string, int, int>> DeckCounts = new Dictionary<Guid, Tuple<string, int, int>>();
            foreach(var i in Collections.Where(x => x.UUID != Guid.Empty && x.UUID != Collections[CurrentCollectionInd].UUID))
            {
                var AmountinDeck = CollectionSearchUtils.GetIdenticalCardsFromCollection(i, inventoryObject.InvData, new CardMatchFilters().SetAll(true).Set(_FilterCategory: false));
                var SimilarAmountinDeck = CollectionSearchUtils.GetIdenticalCardsFromCollection(i, inventoryObject.InvData, new CardMatchFilters().SetAll(false));
                DeckCounts[i.UUID] = new(i.Name, AmountinDeck.Count(), SimilarAmountinDeck.Count());
            }
            string Message = $"Decks containing: {inventoryObject.CardData().name} {inventoryObject.SetData().GetRarityCode()} {inventoryObject.SetData().set_name}\n\n";
            foreach(var i in DeckCounts.Values)
            {
                if (i.Item3 <= 0) { continue; }
                Message += $"{i.Item1.ToUpper()}\n";
                Message += $"Exact Printing: {i.Item2}\n";
                int OtherPrinting = i.Item3 - i.Item2;
                if (OtherPrinting > 0) { Message += $"Other Printing: {OtherPrinting} \n"; }
                Message += $"\n";

            }
            MessageBox.Show(Message);
        }

        public void ShowOtherAvailablePrinting(DuplicateCardContainer inventoryContainer)
        {
            var OtherPrintings = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], inventoryContainer.InvData, new CardMatchFilters().SetAll(false)); ;
            Dictionary<string, Tuple<string, int, int>> AltPrintings = new();
            foreach (var i in OtherPrintings)
            {
                var Entry = Collections[0].data[i];
                var IDString = Entry.CreateIDString();
                var Card = Utility.GetCardByID(Entry.cardID);
                var Set = Utility.GetExactCard(Card, Entry.set_code, Entry.set_rarity);
                var OtherDecks = CollectionSearchUtils.GetAmountOfCardInNonInventoryCollections(Collections, inventoryContainer.InvData, new int[] {CurrentCollectionInd}, new CardMatchFilters(), true);
                if (!AltPrintings.ContainsKey(IDString)) { AltPrintings[IDString] = new($"{Set.set_name} {Set.GetRarityCode()} ({Entry.Condition}) (Art{Entry.ImageIndex+1})", 0, OtherDecks); }
                AltPrintings[IDString] = new(AltPrintings[IDString].Item1, AltPrintings[IDString].Item2 + 1, AltPrintings[IDString].Item3);
            }
            var Available = AltPrintings.Where(x => x.Value.Item3 < 1).Select(x => $"{x.Value.Item2 - x.Value.Item3}X {x.Value.Item1}").ToArray();
            var Shareable = AltPrintings.Where(x => x.Value.Item3 > 0).Select(x => $"{x.Value.Item2}X {x.Value.Item1}");

            string Message = $"Printings available for use in this deck:\n{string.Join('\n', Available)}";
            if (Shareable.Any()) { Message += $"\n\nPrintings available for sharing with other decks\n{string.Join('\n', Shareable)}"; }
            MessageBox.Show(Message);
        }

        #endregion Inventory Display

        #region Collection Management

        bool Collectionloading = false;
        private void LoadCollection(int Index)
        {
            Collectionloading = true;
            CurrentCollectionInd = Index;
            deleteToolStripMenuItem.Visible = Index != 0;
            renameToolStripMenuItem.Visible = Index != 0;
            isPaperCollectionToolStripMenuItem.Visible = Index != 0;
            isPaperCollectionToolStripMenuItem.Checked = Collections[Index].PaperCollection;
            addCollectionToInventoryToolStripMenuItem.Visible = Index != 0;
            cmbCollectedCardCategory.Enabled = Index != 0;
            if (Index == 0) { cmbAddTo.SelectedIndex = 0; }
            lblAddTo.Visible = Index != 0;
            cmbAddTo.Visible = Index != 0;
            selectedCard = null;
            txtSearch.Text = string.Empty;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryManager));
            pictureBox1.Image = ((Image)(resources.GetObject("pictureBox1.Image")));
            PrintSelectedCard("N/A");
            PrintInventory();
            Collectionloading = false;
        }
        private void UpdateCollectionsList()
        {
            comboBox1.Items.Clear();
            foreach (CardCollection c in Collections)
            {
                comboBox1.Items.Add(c.Name);
            }
            UpdatePopoutForms(true);
        }
        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            if (sender is not System.Windows.Forms.ComboBox cmb) { return; }
            cmb.DropDownWidth = GetDropDownWidth(cmb);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCollection(comboBox1.SelectedIndex);
        }
        private void BTNImportCollection_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Select YDK File",
                DefaultExt = "ydk",
                Filter = "ydk files (*.ydk)|*.ydk"
            };
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }

            string DeckName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

            var YDKCollection = new CardCollection { UUID = Guid.NewGuid(), data = new Dictionary<Guid, InventoryDatabaseEntry>(), Name = DeckName, LastEdited = DateTime.Now };

            AddYDKToCollection(YDKCollection, File.ReadAllLines(openFileDialog1.FileName));

            Collections.Add(YDKCollection);
            UpdateCollectionsList();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }
        private void btnAddCollection_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Collection Name", "Add New Collection", "", 0, 0);
            if (string.IsNullOrWhiteSpace(input)) { return; }
            Collections.Add(new CardCollection { UUID = Guid.NewGuid(), data = new Dictionary<Guid, InventoryDatabaseEntry>(), Name =  input, LastEdited = DateTime.Now });
            UpdateCollectionsList();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        private void btnDeleteCollection_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 1) { return; }
            DialogResult Confirm = DialogResult.OK;
            if (ModifierKeys != Keys.Shift)
            {
                Confirm = MessageBox.Show($"Are you sure you want to delete Collection [{Collections[comboBox1.SelectedIndex].Name}]?\n\nHold shift to skip this pormpt", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            if (Confirm != DialogResult.OK) { return; }

            var UUID = Collections[comboBox1.SelectedIndex].UUID;

            Collections.Remove(Collections[comboBox1.SelectedIndex]);
            UpdateCollectionsList();
            comboBox1.SelectedIndex = 0;

            if (DeckPathDictionary.ContainsKey(UUID) && File.Exists(DeckPathDictionary[UUID]))
            {
                File.Delete(DeckPathDictionary[UUID]);
            }
        }
        private void AddYDKToCollection(CardCollection Collection, string[] YDKContent)
        {
            List<Tuple<YGOCardOBJ, Categories, int>> Cards = new();
            Categories CurrentCategory = Categories.MainDeck;
            foreach (var line in YDKContent)
            {
                if (line.Trim() == "#main") { CurrentCategory = Categories.MainDeck; }
                if (line.Trim() == "#extra") { CurrentCategory = Categories.ExtraDeck; }
                if (line.Trim() == "!side") { CurrentCategory = Categories.SideDeck; }
                if (!int.TryParse(line.Trim(), out int CardIndex)) { Debug.WriteLine($"Line Invalid {line}"); continue; }
                var card = Utility.GetCardByID(CardIndex, out int ArtID);
                if (card == null) { Debug.WriteLine($"{CardIndex} not valid"); continue; }
                if (card.card_sets is null) { Debug.WriteLine($"{card.name} Has not been released in any sets, skipping"); continue; }
                Cards.Add(new(card, CurrentCategory, ArtID));
            }

            var CommonSets = Utility.GetCommonSets(Cards.Select(x => x.Item1));

            foreach (var card in Cards) 
            {
                string? setOverride = null;
                string? RarityOverride = null;
                if (CommonSets.Any())
                {
                    var CommonSet = CommonSets.First();
                    var CommonSetData = card.Item1.card_sets.First(x => x.set_name == CommonSet);
                    setOverride = CommonSetData.set_code;
                    RarityOverride = CommonSetData.set_rarity;
                }

                InventoryDatabaseEntry Template = new InventoryDatabaseEntry { cardID = card.Item1.id, set_code = setOverride, set_rarity = RarityOverride };

                var DefaultCard = SmartCardSetSelector.GetBestSetPrinting(Template, Collections, Collection, out int ImageIndex);
                Guid UUID = Guid.NewGuid();
                Collection.data.Add(UUID, new InventoryDatabaseEntry()
                {
                    cardID = card.Item1.id,
                    set_code = DefaultCard.set_code,
                    set_rarity = DefaultCard.set_rarity,
                    Category = card.Item2,
                    Condition = SafeGetDefaultCondition(),
                    ImageIndex = ImageIndex < 0 ? card.Item3 : ImageIndex,
                    DateAdded = DateAndTime.Now,
                    LastUpdated= DateAndTime.Now
                });
            }
            SaveCollection(Collection);
        }

        #endregion Collection Management

        int GetDropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (var obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            return maxWidth + SystemInformation.VerticalScrollBarWidth;
        }

        public void SaveData()
        {
            
            foreach (var i in Collections)
            {
                if (i.UUID == null || i.UUID == Guid.Empty) { continue; }
                SaveCollection(i);
            }

        }

        public void SaveCollection(CardCollection Collection)
        {
            if (Collection.UUID == null || Collection.UUID == Guid.Empty) 
            {
                foreach(var i in Collection.data) { i.Value.Category = Categories.MainDeck; }
                File.WriteAllText(YGODataManagement.GetInventoryFilePath(), JsonConvert.SerializeObject(Collections[0], Formatting.Indented));
                return;
            }
            if (!DeckPathDictionary.ContainsKey(Collection.UUID))
            {
                DeckPathDictionary[Collection.UUID] = Utility.CreateUniqueFilename(Collection.Name, YGODataManagement.GetDeckDirectoryPath());
            }
            File.WriteAllText(DeckPathDictionary[Collection.UUID], JsonConvert.SerializeObject(Collection, Formatting.Indented));
        }

        private void chkPaperCollection_CheckedChanged(object sender, EventArgs e)
        {
            if (Collectionloading) { return; }
            Collections[CurrentCollectionInd].PaperCollection = !Collections[CurrentCollectionInd].PaperCollection;
            isPaperCollectionToolStripMenuItem.Checked = Collections[CurrentCollectionInd].PaperCollection;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            btnRemoveSelected.Text = $"Remove {numericUpDown1.Value}";
        }

        private void addCollectionToInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentCollectionInd == 0 || Collections[CurrentCollectionInd].UUID == Guid.Empty) { return; }

            string Instructions = "This will add a copy of each card in this collection to your main inventory, would you like to continue?";

            var MaybeDeck = Collections[CurrentCollectionInd].data.Where(x => x.Value.Category == Categories.MaybeDeck);
            if (MaybeDeck.Any()) { Instructions += $"\n\n WARNING: All Cards in the Maybe Deck ({MaybeDeck.Count()}) will be ignored"; }

            var DialogResult = MessageBox.Show(Instructions, "Import Collection to Inventory", MessageBoxButtons.YesNo);
            if (DialogResult != DialogResult.Yes) { return; }

            foreach(var card in Collections[CurrentCollectionInd].data)
            {
                if (card.Value.Category == Categories.MaybeDeck) { continue; }
                DuplicateCardContainer TempNewInvObjectContainer = new();
                TempNewInvObjectContainer.InvData = card.Value.Clone();
                TempNewInvObjectContainer.InvData.Category = Categories.MainDeck;
                TempNewInvObjectContainer.InvData.DateAdded = DateTime.Now;
                TempNewInvObjectContainer.InvData.LastUpdated = DateTime.Now;

                Guid UUID = Guid.NewGuid();
                Collections[0].data.Add(UUID, TempNewInvObjectContainer.InvData);
            }
            SaveCollection(Collections[0]);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentCollectionInd == 0 || Collections[CurrentCollectionInd].UUID == Guid.Empty) { return; }
            var Collection = Collections[CurrentCollectionInd];
            string CurrentName = Collection.Name;
            string input = Interaction.InputBox("Enter New Collection Name", "Rename Collection", CurrentName, 0, 0);
            if (!string.IsNullOrWhiteSpace(input)) 
            { 
                Collection.Name = input;
                SaveCollection(Collections[CurrentCollectionInd]);
            }
            UpdateCollectionsList();
            comboBox1.SelectedIndex = CurrentCollectionInd;
        }

        public List<InventoryDisplay> ActiveDisplays = new List<InventoryDisplay>();

        private void popoutDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InventoryDisplay inventoryDisplay = new InventoryDisplay(this);
            inventoryDisplay.Show();
            inventoryDisplay.UpdateData(true);
            ActiveDisplays.Add(inventoryDisplay);
        }

        private void UpdatePopoutForms(bool CollectionsChanged)
        {
            foreach(var i in ActiveDisplays)
            {
                i.UpdateData(CollectionsChanged);
            }
        }

        private void DefaultConditionSelectComboBox_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(DefaultConditionSelectComboBox.Text);
        }

        private string SafeGetDefaultCondition()
        {
            string SelectedCondition = DefaultConditionSelectComboBox.SelectedItem.ToString();
            bool ConditionValid = !string.IsNullOrWhiteSpace(SelectedCondition) && BulkData.Conditions.ContainsKey(SelectedCondition);
            if (!ConditionValid)
            {
                DefaultConditionSelectComboBox.SelectedIndex = DefaultConditionSelectComboBox.Items.IndexOf(_DatabaseForm.Settings.DefaultCondition);
                return _DatabaseForm.Settings.DefaultCondition;
            }
            return SelectedCondition;
        }

        private void compareYDKsToInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog= new FolderBrowserDialog();
            var Result = folderBrowserDialog.ShowDialog();
            if (Result != DialogResult.OK || !Directory.Exists(folderBrowserDialog.SelectedPath)) { return; }
            List<Tuple<string, int, int, double>> data = new List<Tuple<string, int, int, double>>();
            foreach (var file in Directory.GetFiles(folderBrowserDialog.SelectedPath))
            {
                if (Path.GetExtension(file).ToLower() != ".ydk") { continue; }
                var Content = File.ReadAllLines(file);
                Dictionary<int, int> Cards = new Dictionary<int, int>();
                foreach(var Line in Content)
                {
                    if (!int.TryParse(Line.Trim(), out int CardIndex) || !YGODataManagement.IDLookup.ContainsKey(CardIndex)) { continue; }
                    var Card = YGODataManagement.MasterDataBase.data[YGODataManagement.IDLookup[CardIndex].Item1];
                    if (!Cards.ContainsKey(Card.id)) { Cards.Add(Card.id, 0);}
                    Cards[Card.id]++;
                }
                int TotalNeeded = 0;
                int TotalInDeck = Cards.Values.Sum();
                foreach (var card in Cards)
                {
                    var CardsInCollection = Collections[0].data.Where(x => x.Value.cardID == card.Key).Select(x => x.Value);
                    int Needed = card.Value - CardsInCollection.Count();
                    if (Needed < 0) { Needed = 0; }
                    TotalNeeded += Needed;
                }
                int TotalObtained = TotalInDeck - TotalNeeded;
                double Percentage = Math.Round((double)TotalObtained / (double)TotalInDeck,2);
                data.Add(new(Path.GetFileName(file), TotalObtained, TotalInDeck, Percentage));
            }
            Debug.WriteLine($"{JsonConvert.SerializeObject(data.OrderBy(x => x.Item4), Formatting.Indented)}");
        }
    }
}
