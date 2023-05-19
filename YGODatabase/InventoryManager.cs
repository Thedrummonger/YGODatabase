using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class InventoryManager : Form
    {
        MainInterface _DatabaseForm;

        private string selectedCard = null;

        public static List<CardCollection> Collections = new List<CardCollection>();
        public static int CurrentCollectionInd;

        public InventoryManager(MainInterface DatabaseForm)
        {
            _DatabaseForm = DatabaseForm;
            InitializeComponent();
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

            Dictionary<string, string> results = new Dictionary<string, string>();
            List<DataModel.CardSearchResult> Formattedresults = new List<DataModel.CardSearchResult>();

            foreach (var i in YGODataManagement.MasterDataBase.data)
            {
                if (i.card_sets == null || !i.card_sets.Any()) { continue; }
                foreach (var j in i.card_sets.OrderBy(x => x.GetRarityIndex()))
                {
                    string DisplayName = $"{i.name}";
                    if (chkShowRarity.Checked) { DisplayName += $" {j.GetRarityCode()}"; }
                    if (chkShowSet.Checked) { DisplayName += $" ({j.set_name})"; }

                    bool SearchValid = SearchParser.CardMatchesFilter(DisplayName, i, j, txtSearch.Text, NameSearch, CodeSearch);

                    if (results.ContainsKey(DisplayName)) { continue; }
                    if (SearchValid)
                    {
                        results[DisplayName] = j.set_code;
                        Formattedresults.Add(new DataModel.CardSearchResult { DisplayName = DisplayName, Card = i, Set = j });
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
        private void AddSelectedCard()
        {
            if (lbSearchResults.SelectedIndex < 0) { return; }
            if (lbSearchResults.SelectedItem is not DataModel.CardSearchResult SelectedCard) { return; }
            txtSearch.SelectAll();
            txtSearch.Focus();

            Debug.WriteLine($"Adding to collection {Collections[CurrentCollectionInd].Name}");

            string UUID = Guid.NewGuid().ToString();

            Collections[CurrentCollectionInd].data.Add(UUID, new DataModel.InventoryDatabaseEntry
            {
                cardID = SelectedCard.Card.id,
                set_code = SelectedCard.Set.set_code,
                set_rarity = SelectedCard.Set.set_rarity,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            selectedCard = UUID;
            PrintSelectedCard("Last Added Card");

            PrintInventory();

        }

        #endregion Search Functions

        #region Selected Item

        bool SelectedCardUpdating = false;
        private void PrintSelectedCard(string Source, int AmountToEdit = 1)
        {
            gbSelectedCard.Text = Source;
            SelectedCardUpdating = true;
            cmbSelctedCardRarity.Enabled = true;
            cmbSelectedCardSet.Enabled = true;
            cmbSelectedCardCondition.Enabled = true;
            btnRemoveSelected.Enabled = true;
            BtnAddOneSelected.Enabled = true;
            numericUpDown1.Enabled = false;
            if (selectedCard is null || !Collections[CurrentCollectionInd].data.ContainsKey(selectedCard))
            {
                gbSelectedCard.Text = "N/A";
                lblSelectedCard.Text = "N/A";
                cmbSelctedCardRarity.DataSource = null;
                cmbSelectedCardSet.DataSource = null;
                cmbSelectedCardCondition.DataSource = null;
                numericUpDown1.Enabled = false;

                cmbSelctedCardRarity.Enabled = false;
                cmbSelectedCardSet.Enabled = false;
                cmbSelectedCardCondition.Enabled = false;
                SelectedCardUpdating = false;
                btnRemoveSelected.Enabled = false;
                BtnAddOneSelected.Enabled = false;
                numericUpDown1.Enabled = false;
                numericUpDown1.Minimum = 0;
                numericUpDown1.Maximum = 0;
                numericUpDown1.Value = 0;
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

            SelectedCardUpdating = false;

            var IdenticalCards = Utility.GetIdenticalInventory(selectedCard, Collections[CurrentCollectionInd]);
            if (AmountToEdit > IdenticalCards.Length + 1) { AmountToEdit = IdenticalCards.Length + 1; }
            numericUpDown1.Enabled = true;
            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = IdenticalCards.Length + 1;
            numericUpDown1.Value = AmountToEdit;

        }
        private void SelectedCardValueEdited(object sender, EventArgs e)
        {
            if (SelectedCardUpdating) { return; }

            string Rarity = (string)cmbSelctedCardRarity.SelectedItem;
            string Set = (string)cmbSelectedCardSet.SelectedItem;
            string Condition = (string)cmbSelectedCardCondition.SelectedItem;

            List<string> SelectedCards = new List<string>() { selectedCard };
            SelectedCards.AddRange(Utility.GetIdenticalInventory(selectedCard, Collections[CurrentCollectionInd]).Select(x => x));
            for(var i = 0; i < numericUpDown1.Value; i++)
            {
                EditCard(SelectedCards[i], Rarity, Set, Condition, sender == cmbSelectedCardSet);
            }

            PrintSelectedCard(gbSelectedCard.Text, (int)numericUpDown1.Value);
            PrintInventory();

        }

        private void EditCard(string UUID, string NewRarity, string NewSetName, string NewCondition, bool EditingSet)
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
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            List<string> SelectedCards = Utility.GetIdenticalInventory(selectedCard, Collections[CurrentCollectionInd]).ToList();
            if (numericUpDown1.Value > SelectedCards.Count)
            {
                SelectedCards.Add(selectedCard);
                selectedCard = null;
            }
            for (var i = 0; i < numericUpDown1.Value; i++)
            {
                Collections[CurrentCollectionInd].data.Remove(SelectedCards[i]);
            }

            PrintSelectedCard(gbSelectedCard.Text);
            PrintInventory();
        }

        private void btnAddOneSelected_Click(object sender, EventArgs e)
        {
            string UUID = Guid.NewGuid().ToString();

            var CurrentCard = Collections[CurrentCollectionInd].data[selectedCard];

            Collections[CurrentCollectionInd].data.Add(UUID, new DataModel.InventoryDatabaseEntry
            {
                cardID = CurrentCard.cardID,
                set_code = CurrentCard.set_code,
                set_rarity = CurrentCard.set_rarity,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            selectedCard = UUID;
            PrintSelectedCard("Last Added Card");

            PrintInventory();
        }

        #endregion Selected Item

        private void LoadCollection(int Index)
        {
            CurrentCollectionInd = Index;
            btnDeleteCollection.Enabled = !Collections[Index].IsMainInventory;
            selectedCard = null;
            txtSearch.Text = string.Empty;
            pictureBox1.Image= null;
            PrintSelectedCard("N/A");
            PrintInventory();
        }

        private void UpdateCollectionsList()
        {
            comboBox1.Items.Clear();
            foreach (CardCollection c in Collections)
            {
                comboBox1.Items.Add(c.Name);
            }
        }

        #region FormFunctions
        private void InventoryManager_Load(object sender, EventArgs e)
        {
            Collections.Add(new CardCollection { IsMainInventory = true, data = YGODataManagement.Inventory, Name = "Inventory" });
            //Collections.Add(new CardCollection { IsMainInventory = false, data = new Dictionary<string, InventoryDatabaseEntry>(), Name = "Test Deck" });
            CurrentCollectionInd = 0;
            cmbFilterBy.SelectedIndex = 0;
            cmbOrderBy.SelectedIndex = 0;

            UpdateCollectionsList();
            comboBox1.SelectedIndex = 0;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCollection(comboBox1.SelectedIndex);
        }
        private void InventoryManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            _DatabaseForm.inventoryManager = null;
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
        private void lbSearchResults_DoubleClick(object sender, EventArgs e)
        {
            AddSelectedCard();
        }

        #endregion FormFunctions

        #region Inventory Display
        private void PrintInventory()
        {
            Debug.WriteLine($"Printing {Collections[CurrentCollectionInd].Name}");
            Dictionary<string, DataModel.InventoryObject> UniqueEntries = new Dictionary<string, DataModel.InventoryObject>();

            foreach (var i in Collections[CurrentCollectionInd].data)
            {
                var Card = Utility.GetCardByID(i.Value.cardID);
                var Set = Utility.GetExactCard(Card, i.Value.set_code, i.Value.set_rarity);
                string UUID = $"{Card.name} {Set.set_name} {Set.set_rarity} {i.Value.Condition}";
                if (!UniqueEntries.ContainsKey(UUID))
                {
                    UniqueEntries.Add(UUID, new DataModel.InventoryObject { Amount = 0 });
                }
                UniqueEntries[UUID].Amount++;
                UniqueEntries[UUID].Card = Card;
                UniqueEntries[UUID].Set = Set;
                UniqueEntries[UUID].InventoryID = i.Key;
            }

            listView1.BeginUpdate();
            listView1.Items.Clear();

            IEnumerable<DataModel.InventoryObject> PrintList = UniqueEntries.Values.OrderByDescending(x => Collections[CurrentCollectionInd].data[x.InventoryID].DateAdded).ToArray();
            bool OrderByName = cmbOrderBy.SelectedIndex == 0;
            bool OrderBySet = cmbOrderBy.SelectedIndex == 1; ;
            bool OrderByRarity = cmbOrderBy.SelectedIndex == 2; ;
            bool OrderByCondition = cmbOrderBy.SelectedIndex == 3; ;
            bool OrderByModified = cmbOrderBy.SelectedIndex == 5; ;

            if (OrderByName)
            {
                PrintList = PrintList.OrderBy(x => x.Card.name).ThenBy(x => x.Set.set_name).ThenBy(x => x.Set.GetRarityIndex()).ThenBy(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]);
            }
            else if (OrderBySet) 
            {
                PrintList = PrintList.OrderBy(x => x.Set.set_name).ThenBy(x => x.Card.name).ThenBy(x => x.Set.GetRarityIndex()).ThenBy(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]);
            }
            else if (OrderByRarity)
            {
                PrintList = PrintList.OrderBy(x => x.Set.GetRarityIndex()).ThenBy(x => x.Card.name).ThenBy(x => x.Set.set_name).ThenBy(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]);
            }
            else if (OrderByModified)
            {
                PrintList = PrintList.OrderByDescending(x => Collections[CurrentCollectionInd].data[x.InventoryID].LastUpdated);
            }
            else if (OrderByCondition)
            {
                PrintList = PrintList.OrderBy(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]).ThenBy(x => x.Card.name).ThenBy(x => x.Set.set_name).ThenBy(x => x.Set.GetRarityIndex());

            }

            foreach (var i in PrintList)
            {
                string[] DisplayData = new string[] { i.Amount.ToString(), i.Card.name, i.Set.set_name, i.Set.GetRarityCode(), BulkData.Conditions[Collections[CurrentCollectionInd].data[i.InventoryID].Condition] };
                listView1.Items.Add(Utility.CreateListViewItem(i, DisplayData));
            }
            listView1.EndUpdate();

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

        #endregion Inventory Display

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

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            if (sender is not ComboBox cmb) { return; }
            cmb.DropDownWidth = GetDropDownWidth(cmb);
        }

        private void cmbOrderBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrintInventory();
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

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(var i in Collections)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(i, Formatting.Indented));
            }
        }

        private void btnAddCollection_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter Deck Name", "Add New Deck", "", 0, 0);
            Collections.Add(new CardCollection { IsMainInventory = false, data = new Dictionary<string, InventoryDatabaseEntry>(), Name =  input, LastEdited = DateTime.Now });
            UpdateCollectionsList();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }
    }
}
