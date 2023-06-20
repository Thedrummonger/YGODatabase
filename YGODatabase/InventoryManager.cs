using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static YGODatabase.DataModel;
using Microsoft.VisualBasic.ApplicationServices;
using System.Linq.Expressions;
using YGODatabase.Properties;
using System.Drawing;

namespace YGODatabase
{
    public partial class InventoryManager : Form
    {
        MainInterface _DatabaseForm;

        private Guid selectedCard = Guid.Empty;

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
            List<DataModel.CardSearchResult> Formattedresults = new List<DataModel.CardSearchResult>();

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
                        Formattedresults.Add(new DataModel.CardSearchResult { DisplayName = DisplayName, Card = i, Set = j, FilteringRarity = chkShowRarity.Checked || CodeSearch, FilteringSet = chkShowSet.Checked || CodeSearch });
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
            if (lbSearchResults.SelectedItem is not DataModel.CardSearchResult SelectedCard) { return; }
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
            var BestSetMatch = SmartCardSetSelector.GetBestSetPrinting(SelectedCard.Card, Collections, CurrentCollectionInd, ForceSet, ForceRarity);

            if (BestSetMatch == null) 
            {
                MessageBox.Show($"ERROR: selected card was invalid. This is a bug.\n\n{SelectedCard.Card.name} | {ForceSet??"Any Set"} | {ForceSet??"Any Rarity"}" );
                return;
            }
            Debug.WriteLine($"Adding to collection {SelectedCard.Card.name} | {BestSetMatch.set_name} | {BestSetMatch.set_rarity}");

            Collections[CurrentCollectionInd].data.Add(UUID, new DataModel.InventoryDatabaseEntry
            {
                cardID = SelectedCard.Card.id,
                set_code = BestSetMatch.set_code,
                set_rarity = BestSetMatch.set_rarity,
                Category = CurrentCollectionInd == 0 ? Categories.None : SelectedCategory,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = UUID;
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

            gbSelectedCard.Text = Source;
            cmbSelctedCardRarity.Enabled = true;
            cmbSelectedCardSet.Enabled = true;
            cmbSelectedCardCondition.Enabled = true;
            cmbCollectedCardCategory.Enabled = true;
            btnRemoveSelected.Enabled = true;
            BtnAddOneSelected.Enabled = true;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            if (selectedCard == Guid.Empty || !Collections[CurrentCollectionInd].data.ContainsKey(selectedCard))
            {
                gbSelectedCard.Text = "N/A";
                lblSelectedCard.Text = "N/A";
                cmbSelctedCardRarity.DataSource = null;
                cmbSelectedCardSet.DataSource = null;
                cmbSelectedCardCondition.DataSource = null;

                cmbSelctedCardRarity.Enabled = false;
                cmbSelectedCardSet.Enabled = false;
                cmbSelectedCardCondition.Enabled = false;
                cmbCollectedCardCategory.Enabled = false;
                btnRemoveSelected.Enabled = false;
                BtnAddOneSelected.Enabled = false;
                numericUpDown1.Enabled = false;
                numericUpDown1.Minimum = 0;
                numericUpDown1.Maximum = 0;
                numericUpDown1.Value = 0;
                numericUpDown2.Enabled = false;
                numericUpDown2.Minimum = 0;
                numericUpDown2.Maximum = 0;
                numericUpDown2.Value = 0;

                SelectedCardUpdating = false;
                return;
            }
            var InventoryObject = Collections[CurrentCollectionInd].data[selectedCard];
            var Card = Utility.GetCardByID(InventoryObject.cardID);
            var SetEntry = Card.card_sets.First(x => x.set_code == InventoryObject.set_code && x.set_rarity == InventoryObject.set_rarity);
            lblSelectedCard.Text = $"{Card.name}";
            cmbSelectedCardSet.DataSource = Card.GetAllSetsContainingCard();
            foreach (var i in cmbSelectedCardSet.Items) { if (i.ToString() == SetEntry.set_name) { cmbSelectedCardSet.SelectedItem = i; break; } }
            cmbSelctedCardRarity.DataSource = Card.GetAllRaritiesInSet(SetEntry.set_name);
            foreach (var i in cmbSelctedCardRarity.Items) { if (i.ToString() == SetEntry.set_rarity) { cmbSelctedCardRarity.SelectedItem = i; break; } }
            cmbSelectedCardCondition.DataSource = BulkData.Conditions.Keys.ToArray();
            foreach (var i in cmbSelectedCardCondition.Items) { if (i.ToString() == InventoryObject.Condition) { cmbSelectedCardCondition.SelectedItem = i; break; } }
            cmbCollectedCardCategory.DataSource = CategoryNames.Select(x => new ComboBoxItem { DisplayName = x.Value, tag = x.Key }).ToArray();
            foreach (ComboBoxItem i in cmbCollectedCardCategory.Items) { if ((Categories)i.tag == InventoryObject.Category) { cmbCollectedCardCategory.SelectedItem = i; break; } }

            numericUpDown2.Minimum = 1;
            numericUpDown2.Maximum = Card.card_images.Length;
            numericUpDown2.Value = InventoryObject.ImageIndex + 1;

            var IdenticalCards = Collections[CurrentCollectionInd].GetIdenticalCards(selectedCard, true);
            if (AmountToEdit > IdenticalCards.Length) { AmountToEdit = IdenticalCards.Length; }
            
            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = IdenticalCards.Length;
            numericUpDown1.Value = AmountToEdit;

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

            Guid[] SelectedCards = Collections[CurrentCollectionInd].GetIdenticalCards(selectedCard, true).Reverse().ToArray();
            for(var i = 0; i < numericUpDown1.Value; i++)
            {
                EditCard(SelectedCards[i], Rarity, Set, Condition, sender == cmbSelectedCardSet, Category, Image);
            }

            SaveCollection(Collections[CurrentCollectionInd]);

            PrintSelectedCard(gbSelectedCard.Text, (int)numericUpDown1.Value);
            PrintInventory();
            UpdatePopoutForms(false);
            UpdatepictureBox(Utility.GetCardByID(Collections[CurrentCollectionInd].data[selectedCard].cardID), Collections[CurrentCollectionInd].data[selectedCard].ImageIndex);
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
            List<Guid> SelectedCards = Collections[CurrentCollectionInd].GetIdenticalCards(selectedCard, true).ToList();
            List<Guid> RemovedCards = new List<Guid>();

            for (var i = (int)numericUpDown1.Value - 1; i >= 0; i--)
            {
                Collections[CurrentCollectionInd].data.Remove(SelectedCards[i]);
                RemovedCards.Add(SelectedCards[i]);
            }
            var RemainingInventory = SelectedCards.Where(x => !RemovedCards.Contains(x)).ToList();
            selectedCard = RemainingInventory.Any() ? RemainingInventory.Last() : Guid.Empty;

            SaveCollection(Collections[CurrentCollectionInd]);
            PrintSelectedCard(gbSelectedCard.Text);
            PrintInventory();
            UpdatePopoutForms(false);
        }

        private void btnAddOneSelected_Click(object sender, EventArgs e)
        {
            Guid UUID = Guid.NewGuid();

            var CurrentCard = Collections[CurrentCollectionInd].data[selectedCard];

            Collections[CurrentCollectionInd].data.Add(UUID, new DataModel.InventoryDatabaseEntry
            {
                cardID = CurrentCard.cardID,
                set_code = CurrentCard.set_code,
                set_rarity = CurrentCard.set_rarity,
                Category = CurrentCard.Category,
                ImageIndex = CurrentCard.ImageIndex,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            SaveCollection(Collections[CurrentCollectionInd]);

            selectedCard = UUID;
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
            }
        }
        private void CaptureTabPress(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                AddSelectedCard();
                e.IsInputKey = true;
            }
        }
        private void HighlightCard(object sender, EventArgs e)
        {
            int ImageIndex = 0;
            DataModel.YGOCardOBJ Card;
            if (sender == lbSearchResults && lbSearchResults.SelectedItem is DataModel.CardSearchResult SearchSelectedCard)
            {
                Card = SearchSelectedCard.Card;
            }
            else if (sender == listView1 && listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is DataModel.InventoryObject InventorySelectedCard)
            {
                Card = InventorySelectedCard.Card;
                ImageIndex = Collections[CurrentCollectionInd].data[InventorySelectedCard.InventoryID].ImageIndex;
            }
            else
            {
                return;
            }
            UpdatepictureBox(Card, ImageIndex);
        }
        private async void UpdatepictureBox(DataModel.YGOCardOBJ card, int ImageIndex)
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
                listView1.SelectedItems[0].Tag is not DataModel.InventoryObject Data) 
            { return; }

            selectedCard = Data.InventoryID;
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

            if (SelectedEntry.Tag is InventoryObject inventoryObject)
            {
                ToolStripItem SelectCard = contextMenu.Items.Add("Select Card");
                SelectCard.Click += (sender, e) => { selectedCard = inventoryObject.InventoryID; PrintSelectedCard("Selected Card"); };

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

        private void ShowOtherDecksUsingCard(InventoryObject inventoryObject)
        {
            Dictionary<Guid, Tuple<string, int, int>> DeckCounts = new Dictionary<Guid, Tuple<string, int, int>>();
            foreach(var i in Collections.Where(x => x.UUID != Guid.Empty && x.UUID != Collections[CurrentCollectionInd].UUID))
            {
                var AmountinDeck = SmartCardSetSelector.GetCardsFromInventory(inventoryObject.Card, i, inventoryObject.Set.set_code, inventoryObject.Set.set_rarity).Count();
                var SimilarAmountinDeck = SmartCardSetSelector.GetCardsFromInventory(inventoryObject.Card, i).Count();
                DeckCounts[i.UUID] = new(i.Name, AmountinDeck, SimilarAmountinDeck);
            }
            string Message = $"Decks containing: {inventoryObject.Card.name} {inventoryObject.Set.GetRarityCode()} {inventoryObject.Set.set_name}\n\n";
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

        public void ShowOtherAvailablePrinting(InventoryObject inventoryObject)
        {
            var OtherPrintings = SmartCardSetSelector.GetCardsFromInventory(inventoryObject.Card, Collections[0]);
            Dictionary<string, Tuple<string, int, int>> AltPrintings = new();
            foreach (var i in OtherPrintings)
            {
                var Entry = Collections[0].data[i];
                var IDString = Entry.CreateIDString();
                var Card = Utility.GetCardByID(Entry.cardID);
                var Set = Utility.GetExactCard(Card, Entry.set_code, Entry.set_rarity);
                var OtherDecks = SmartCardSetSelector.GetAmountOfCardInOtherDecks(Card, Collections, CurrentCollectionInd, Set.set_code, Set.set_rarity, true);
                if (!AltPrintings.ContainsKey(IDString)) { AltPrintings[IDString] = new($"{Set.set_name} {Set.GetRarityCode()} ({Entry.Condition})", 0, OtherDecks); }
                AltPrintings[IDString] = new(AltPrintings[IDString].Item1, AltPrintings[IDString].Item2 + 1, AltPrintings[IDString].Item3);
            }
            var Available = AltPrintings.Where(x => inventoryObject.Amount + x.Value.Item3 <= x.Value.Item2).Select(x => $"{x.Value.Item2}X {x.Value.Item1}").ToArray();
            var Shareable = AltPrintings.Where(x => inventoryObject.Amount <= x.Value.Item2 && inventoryObject.Amount + x.Value.Item3 > x.Value.Item2).Select(x => $"{x.Value.Item2}X {x.Value.Item1}");

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
            selectedCard = Guid.Empty;
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
            if (Control.ModifierKeys != Keys.Shift)
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
            List<Tuple<YGOCardOBJ, Categories>> Cards = new();
            Categories CurrentCategory = Categories.MainDeck;
            foreach (var line in YDKContent)
            {
                if (line.Trim() == "#main") { CurrentCategory = Categories.MainDeck; }
                if (line.Trim() == "#extra") { CurrentCategory = Categories.ExtraDeck; }
                if (line.Trim() == "!side") { CurrentCategory = Categories.SideDeck; }
                if (!int.TryParse(line.Trim(), out int CardIndex)) { Debug.WriteLine($"Line Invalid {line}"); continue; }
                var card = Utility.GetCardByID(CardIndex);
                if (card == null) { Debug.WriteLine($"{CardIndex} not valid"); continue; }
                Cards.Add(new(card, CurrentCategory));
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
                var DefaultCard = SmartCardSetSelector.GetBestSetPrinting(card.Item1, Collections, CurrentCollectionInd, setOverride, RarityOverride);
                Guid UUID = Guid.NewGuid();
                Collection.data.Add(UUID, new DataModel.InventoryDatabaseEntry
                {
                    cardID = card.Item1.id,
                    set_code = DefaultCard.set_code,
                    set_rarity = DefaultCard.set_rarity,
                    Category = card.Item2,
                    DateAdded = DateAndTime.Now,
                    LastUpdated= DateAndTime.Now
                });
            }
            SaveCollection(Collection);
        }

        #endregion Collection Management

        int GetDropDownWidth(System.Windows.Forms.ComboBox myCombo)
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
                Guid UUID = Guid.NewGuid();
                Collections[0].data.Add(UUID, new DataModel.InventoryDatabaseEntry
                {
                    cardID = card.Value.cardID,
                    set_code = card.Value.set_code,
                    set_rarity = card.Value.set_rarity,
                    Category = Categories.MainDeck,
                    DateAdded = DateAndTime.Now,
                    LastUpdated= DateAndTime.Now
                });
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
    }
}
