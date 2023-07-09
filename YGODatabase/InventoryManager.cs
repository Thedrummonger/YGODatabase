using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class InventoryManager : Form
    {
        MainInterface _DatabaseForm;

        private DuplicateCardContainer? selectedCard = null;

        public List<CardCollection> Collections = new List<CardCollection>();
        public int CurrentCollectionInd;

        public List<string> UndoLists = new List<string>();
        public List<string> RedoLists = new List<string>();

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
            lbSearchResults.BeginUpdate();
            bool NameSearch = cmbFilterBy.SelectedIndex == 0 || cmbFilterBy.SelectedIndex == 2;
            bool CodeSearch = cmbFilterBy.SelectedIndex == 1 || cmbFilterBy.SelectedIndex == 2;
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                lbSearchResults.DataSource =  new List<string>() { "Search for card to add" };
                lbSearchResults.EndUpdate();
                return;
            }

            HashSet<string> MatchedDisplayName = new HashSet<string>();
            List<object> Formattedresults = new List<object>();

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
            int ResultLimit = 5000;
            int ResultCount = Formattedresults.Count;
            if (ResultCount > ResultLimit)
            {
                Formattedresults = Formattedresults.Take(ResultLimit).ToList();
                Formattedresults.Add($"{ResultCount - ResultLimit} Results not shown....");
            }

            lbSearchResults.DataSource = Formattedresults;
            lbSearchResults.EndUpdate();
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
            AddSelectedSearchResult();
        }
        private void AddSelectedSearchResult()
        {
            if (lbSearchResults.SelectedIndex < 0) { return; }
            if (lbSearchResults.SelectedItem is not CardSearchResult SelectedCard) { return; }
            txtSearch.SelectAll();
            txtSearch.Focus();

            string? ForceSet = SelectedCard.FilteringSet ? SelectedCard.Set.set_code : null;
            string? ForceRarity = SelectedCard.FilteringRarity ? SelectedCard.Set.set_rarity : null;

            YGOSetData NewSetData = new YGOSetData() { set_code = ForceSet, set_rarity = ForceRarity };

            AddCardToCollection(SelectedCard.Card, NewSetData);
        }

        #endregion Search Functions

        #region Selected Item

        bool SelectedCardUpdating = false;
        private bool PrintSelectedCard(string Source, int AmountToEdit)
        {
            if (selectedCard is not null && selectedCard.Entries.Any(x => !Collections[CurrentCollectionInd].data.ContainsKey(x)))
            {
                Debug.WriteLine($"Error Selected Card contained missing Entries!");
                selectedCard = null;

            }

            SelectedCardUpdating = true;
            foreach (var Control in gbSelectedCard.Controls) { if (Control is ComboBox CMB) { CMB.BeginUpdate(); } }

            bool DisableControls = selectedCard == null;
            gbSelectedCard.Enabled = !DisableControls;
            if (DisableControls)
            {
                gbSelectedCard.Text = "N/A";
                lblSelectedCard.Text = "N/A";
                Utility.ManageNUD(numericUpDown1, 0, 0, 0);
                Utility.ManageNUD(numericUpDown2, 0, 0, 0);
                foreach(var Control in gbSelectedCard.Controls) { if (Control is ComboBox CMB) { CMB.DataSource = null; CMB.EndUpdate(); } }
                SelectedCardUpdating = false;
                return true;
            }
            var InventoryObject = selectedCard.InvData;
            var Card = InventoryObject.CardData();
            var SetEntry = InventoryObject.SetData();

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
            cmbSelectedCardSetCode.DataSource = Card.card_sets.Select(x => new ComboBoxItem { DisplayName = (Card.card_sets.Where(y => y.set_code == x.set_code).Count() > 1) ? $"{x.set_code} {x.GetRarityCode()}" : x.set_code, tag = x }).OrderBy(x => x.DisplayName).ToArray();
            foreach (ComboBoxItem i in cmbSelectedCardSetCode.Items) { if (((YGOSetData)i.tag).set_code == SetEntry.set_code && ((YGOSetData)i.tag).set_rarity == SetEntry.set_rarity) { cmbSelectedCardSetCode.SelectedItem = i; break; } }

            var IdenticalCards = selectedCard.Entries.Count;
            if (AmountToEdit > IdenticalCards) { AmountToEdit = IdenticalCards; }
            Utility.ManageNUD(numericUpDown1, 1, IdenticalCards, AmountToEdit);
            Utility.ManageNUD(numericUpDown2, 1, Card.card_images.Length, InventoryObject.ImageIndex + 1);

            foreach (var Control in gbSelectedCard.Controls) { if (Control is ComboBox CMB) { CMB.EndUpdate(); } }
            SelectedCardUpdating = false;
            return true;
        }
        private void SelectedCardValueEdited(object sender, EventArgs e)
        {
            if (SelectedCardUpdating) { return; }

            SaveState();

            bool UseSetCode = sender == cmbSelectedCardSetCode;

            Guid[] SelectedCards = selectedCard.Entries.ToArray().Reverse().ToArray();
            Guid[] CardsToEdit = SelectedCards.Take((int)numericUpDown1.Value).ToArray();

            YGOSetData SetCodeTag = (YGOSetData)((ComboBoxItem)cmbSelectedCardSetCode.SelectedItem).tag;
            string NewSet = UseSetCode ? SetCodeTag.set_name : (string)cmbSelectedCardSet.SelectedItem;
            string NewRarity = UseSetCode ? SetCodeTag.set_rarity : (string)cmbSelctedCardRarity.SelectedItem;
            string NewCondition = (string)cmbSelectedCardCondition.SelectedItem;
            Categories Category = (Categories)((ComboBoxItem)cmbCollectedCardCategory.SelectedItem).tag;
            int NewImageIndex = (int)numericUpDown2.Value - 1;

            //Validate that the selected rarity is available in the selected set
            var ValidRaritiesForNewSet = selectedCard.CardData().GetAllRaritiesInSet(NewSet);
            if (!ValidRaritiesForNewSet.Contains(NewRarity)) { NewRarity = ValidRaritiesForNewSet.OrderBy(x => Utility.GetRarityIndex(x)).First(); }

            foreach(var i in CardsToEdit)
            {
                EditCard(i, NewRarity, NewSet, NewCondition, Category, NewImageIndex);
            }

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], CardsToEdit.First());

            SaveCollection(Collections[CurrentCollectionInd]);

            PrintSelectedCard(gbSelectedCard.Text, (int)numericUpDown1.Value);
            PrintInventory();
            UpdatePopoutForms(false);
            UpdatepictureBox(selectedCard.CardData(), selectedCard.InvData.ImageIndex);
        }

        private void EditCard(Guid UUID, string NewRarity, string NewSetName, string NewCondition, Categories NewCategory, int ImageIndex)
        {
            var InventoryObject = Collections[CurrentCollectionInd].data[UUID];
            var NewSetEntry = InventoryObject.CardData().card_sets.First(x => x.set_name == NewSetName && x.set_rarity == NewRarity);

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

            SaveState();

            for (var i = (int)numericUpDown1.Value - 1; i >= 0; i--)
            {
                Collections[CurrentCollectionInd].data.Remove(SelectedCards[i]);
                RemovedCards.Add(SelectedCards[i]);
            }
            var RemainingInventory = SelectedCards.Where(x => Collections[CurrentCollectionInd].data.ContainsKey(x));
            selectedCard = RemainingInventory.Any() ? Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], RemainingInventory.Last()) : null;

            SaveCollection(Collections[CurrentCollectionInd]);
            PrintSelectedCard(gbSelectedCard.Text, 1);
            PrintInventory();
            UpdatePopoutForms(false);
        }

        private void btnAddOneSelected_Click(object sender, EventArgs e)
        {
            AddCopyOfCardToCollection(selectedCard.InvData);
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
            SaveFormDataOnClose();
        }
        public void SaveFormDataOnClose()
        {
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
                AddSelectedSearchResult();
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
                    ShowContextMenu(focusedItem);
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
            PrintSelectedCard("Selected Card", 1);
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
        private void ShowContextMenu(ListViewItem SelectedEntry)
        {
            InventoryDisplay.ShowListViewContextMenu(SelectedEntry, Collections, CurrentCollectionInd, PrintInventory, ContextMenuApplySelectedCard);
            void ContextMenuApplySelectedCard(DuplicateCardContainer inventoryObject)
            {
                selectedCard = inventoryObject;
                PrintSelectedCard("Selected Card", 1);
            }
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
            PrintSelectedCard("N/A", 1);
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

            SaveState();

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
            SaveState();
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

        private void SaveState()
        {
            UndoLists.Add(fastJSON.JSON.ToJSON(Collections));
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
            if (Collections[CurrentCollectionInd].IsInventory()) { return; }

            string Instructions = "This will add a copy of each card in this collection to your main inventory, would you like to continue?";

            var MaybeDeck = Collections[CurrentCollectionInd].data.Where(x => x.Value.Category == Categories.MaybeDeck);
            if (MaybeDeck.Any()) { Instructions += $"\n\n WARNING: All Cards in the Maybe Deck ({MaybeDeck.Count()}) will be ignored"; }

            var DialogResult = MessageBox.Show(Instructions, "Import Collection to Inventory", MessageBoxButtons.YesNo);
            if (DialogResult != DialogResult.Yes) { return; }

            SaveState();

            foreach (var card in Collections[CurrentCollectionInd].data)
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
            if (Collections[CurrentCollectionInd].IsInventory()) { return; }
            var Collection = Collections[CurrentCollectionInd];
            string CurrentName = Collection.Name;
            string input = Interaction.InputBox("Enter New Collection Name", "Rename Collection", CurrentName, 0, 0);
            if (!string.IsNullOrWhiteSpace(input)) 
            {
                SaveState();
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

        public void AddCardToCollection(YGOCardOBJ Card, YGOSetData Set = null)
        {

            SaveState();

            Set ??= new YGOSetData() { set_code = null, set_rarity = null };

            Categories SelectedCategory = Categories.MainDeck;
            if (cmbAddTo.SelectedItem is ComboBoxItem addToSelection)
            {
                SelectedCategory = (Categories)addToSelection.tag;
            }

            Guid UUID = Guid.NewGuid();

            InventoryDatabaseEntry Template = new InventoryDatabaseEntry { cardID = Card.id, set_code = Set.set_code, set_rarity = Set.set_rarity };
            var BestSetMatch = SmartCardSetSelector.GetBestSetPrinting(Template, Collections, Collections[CurrentCollectionInd], out int ImageIndex);

            if (BestSetMatch == null)
            {
                MessageBox.Show($"ERROR: selected card was invalid. This is a bug.\n\n{Card.name} | {Set.set_code??"Any Set"} | {Set.set_rarity??"Any Rarity"}");
                return;
            }
            Debug.WriteLine($"Adding to collection {Card.name} | {BestSetMatch.set_name} | {BestSetMatch.set_rarity}");

            Collections[CurrentCollectionInd].data.Add(UUID, new InventoryDatabaseEntry()
            {
                cardID = Card.id,
                set_code = BestSetMatch.set_code,
                set_rarity = BestSetMatch.set_rarity,
                Category = Collections[CurrentCollectionInd].IsInventory() ? Categories.None : SelectedCategory,
                Condition = SafeGetDefaultCondition(),
                ImageIndex = ImageIndex < 0 ? 0 : ImageIndex,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], UUID);
            PrintSelectedCard("Last Added Card", 1);

            PrintInventory();
            UpdatePopoutForms(false);
            UpdatepictureBox(selectedCard.CardData(), selectedCard.InvData.ImageIndex);
        }
        public void AddCopyOfCardToCollection(InventoryDatabaseEntry Entry)
        {
            SaveState();

            Guid UUID = Guid.NewGuid();

            //Only using the DuplicateCardContainer class for it's ability to clone and InventoryDatabaseEntry 
            DuplicateCardContainer TempNewInvObjectContainer = new();
            TempNewInvObjectContainer.InvData = Entry.Clone();
            TempNewInvObjectContainer.InvData.DateAdded = DateTime.Now;
            TempNewInvObjectContainer.InvData.LastUpdated = DateTime.Now;

            Collections[CurrentCollectionInd].data.Add(UUID, TempNewInvObjectContainer.InvData);

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], UUID);
            PrintSelectedCard("Last Added Card", 1);

            PrintInventory();
            UpdatePopoutForms(false);
        }

        private void exportCollectionToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var csv = new StringBuilder();
            var collection = Collections[CurrentCollectionInd];
            List<string> Headers = new List<string>()
            {
                "#",
                "D",
                "Card Name",
                "Set Name",
                "Rarity",
                "Condition"
            };
            if (!collection.IsInventory()) { Headers.Insert(2, "I"); Headers.Add("Min Needed"); Headers.Add("Max Needed"); }

            ListView TempLV = new ListView();
            TextBox ThrowawayTXT = new TextBox();

            InventoryDisplay.PrintInventoryData(TempLV, ThrowawayTXT, Collections, CurrentCollectionInd, "", 0, false);

            csv.AppendLine(string.Join(',', Headers));
            foreach(ListViewItem item in TempLV.Items)
            {
                List<string> row = new List<string>();
                foreach (ListViewItem.ListViewSubItem i in item.SubItems)
                {
                    string text = i.Text;
                    if (text.Contains(',')) { text = $"\"{text}\""; }
                    row.Add(text);
                }
                if (!collection.IsInventory())
                {
                    bool IsHeader = string.IsNullOrWhiteSpace(row[1]);
                    if (IsHeader) { row.Add(string.Empty); }
                    else
                    {
                        int AmountInCollection = int.Parse(row[0]);
                        int AmountInOther = int.Parse(row[1]);
                        int AmountInInventory = int.Parse(row[2]);
                        int AmountNeeded = AmountInCollection - AmountInInventory;
                        if (AmountNeeded < 0) { AmountNeeded = 0; }
                        int AmountNeededForNoOver = AmountInCollection + AmountInOther - AmountInInventory;
                        if (AmountNeededForNoOver < 0) { AmountNeededForNoOver = 0; }
                        row.Add(AmountNeeded.ToString());
                        row.Add(AmountNeededForNoOver.ToString());
                    }
                }
                csv.AppendLine(string.Join(',', row));
            }
            Debug.WriteLine(csv.ToString());

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = collection.Name;
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, csv.ToString());
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            InventoryDisplay.SortColumnByClick(e.Column, chkInvDescending, cmbOrderBy, Collections, CurrentCollectionInd);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (DataTypeValid(e.Data, out _))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!DataTypeValid(e.Data, out object ParsedData)) { return; }
            if (ParsedData is DuplicateCardContainer DCC)
            {
                AddCopyOfCardToCollection(DCC.InvData);
            }
            else if (ParsedData is InventoryDatabaseEntry IDE)
            {
                AddCopyOfCardToCollection(IDE);
            }
            else if (ParsedData is YGOCardOBJ YCO)
            {
                AddCardToCollection(YCO);
            }
        }

        private bool DataTypeValid(IDataObject? Data, out object ParsedData)
        {
            ParsedData = null;
            if (Data is null)
            {
                Debug.WriteLine($"No Data Passed");
                return false;
            }
            else if (Data.GetDataPresent(typeof(DuplicateCardContainer)))
            {
                Debug.WriteLine($"Data Was DuplicateCardContainer");
                ParsedData = Data.GetData(typeof(DuplicateCardContainer));
                return true;
            }
            else if (Data.GetDataPresent(typeof(InventoryDatabaseEntry)))
            {
                Debug.WriteLine($"Data Was InventoryDatabaseEntry");
                ParsedData = Data.GetData(typeof(InventoryDatabaseEntry));
                return true;
            }
            else if (Data.GetDataPresent(typeof(YGOCardOBJ)))
            {
                Debug.WriteLine($"Data Was YGOCardOBJ");
                ParsedData = Data.GetData(typeof(YGOCardOBJ));
                YGOCardOBJ CardData = (YGOCardOBJ)ParsedData;
                bool hasSets = CardData.card_sets is not null && CardData.card_sets.Any();
                Debug.WriteLine($"Had Set Data? {hasSets}");
                return hasSets;
            }
            return false;
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            bool ctrlMod = ModifierKeys.HasFlag(Keys.Control);
            bool shiftMod = ModifierKeys.HasFlag(Keys.Shift);
            Keys Key = e.KeyCode;
            if (Key == Keys.Delete)
            {
                if (!shiftMod)
                {
                    string Warning = "";
                    if (ctrlMod) { Warning += "Do you want to delete all copies of each selected card?"; }
                    else { Warning += "Do you want to delete one copy of each selected card?\n(ctrl+del will delete all copies)"; }
                    Warning += "\n\nHold Shift to skip this confirmation in the future.";
                    var Confirm = MessageBox.Show($"{Warning}", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (Confirm != DialogResult.Yes) { return; }
                }
                List<Guid> DeletedIDs = new List<Guid>();
                foreach (var item in listView1.SelectedItems)
                {
                    if (item is not ListViewItem LVI || LVI.Tag is not DuplicateCardContainer CardContainer) { continue; }
                    if (ctrlMod) { DeletedIDs.AddRange(CardContainer.Entries); }
                    else { DeletedIDs.Add(CardContainer.Entries.Last()); }
                }
                foreach (var ID in DeletedIDs)
                {
                    Collections[CurrentCollectionInd].data.Remove(ID);
                }
                var RemainingInventory = selectedCard is null ? new List<Guid>() : selectedCard.Entries.Where(x => Collections[CurrentCollectionInd].data.ContainsKey(x));
                selectedCard = RemainingInventory.Any() ? Utility.CreateSelectedCardEntry(Collections[CurrentCollectionInd], RemainingInventory.Last()) : null;

                SaveCollection(Collections[CurrentCollectionInd]);
                PrintSelectedCard(gbSelectedCard.Text, 1);
                PrintInventory();
                UpdatePopoutForms(false);
            }
        }
    }
}
