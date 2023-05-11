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

namespace YGODatabase
{
    public partial class InventoryManager : Form
    {
        MainInterface _DatabaseForm;

        private string selectedCard = null;
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
            List<dataModel.CardSearchResult> Formattedresults = new List<dataModel.CardSearchResult>();

            foreach (var i in YGODataManagement.MasterDataBase.data)
            {
                if (i.card_sets == null || !i.card_sets.Any()) { continue; }
                foreach (var j in i.card_sets.OrderBy(x => x.GetRarityIndex()))
                {
                    string DisplayName = $"{i.name}";
                    if (chkShowRarity.Checked) { DisplayName += $" {j.GetRarityCode()}"; }
                    if (chkShowSet.Checked) { DisplayName += $" ({j.set_name})"; }
                    bool NameFilterValid = i.name.CleanCardName().Contains(txtSearch.Text.CleanCardName());
                    bool CodeFilterValid = j.set_code.ToUpper().Replace("-", "").Contains(txtSearch.Text.ToUpper().Replace("-", ""));

                    if (results.ContainsKey(DisplayName)) { continue; }
                    if ((NameSearch && NameFilterValid) || (CodeSearch && CodeFilterValid))
                    {
                        results[DisplayName] = j.set_code;
                        Formattedresults.Add(new dataModel.CardSearchResult { DisplayName = DisplayName, SetCode= j.set_code, CardID = i.id });
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
            if (lbSearchResults.SelectedItem is not dataModel.CardSearchResult SelectedCard) { return; }
            var Card = YGODataManagement.MasterDataBase.data.First(x => x.card_sets is not null && x.card_sets.Any(x => x.set_code == SelectedCard.SetCode));
            var SetData = Card.card_sets.First(x => x.set_code == SelectedCard.SetCode);
            txtSearch.Text = string.Empty;

            string UUID = Guid.NewGuid().ToString();

            YGODataManagement.Inventory.Add(UUID, new dataModel.InventoryDatabaseEntry
            {
                cardID = SelectedCard.CardID,
                set_code = SelectedCard.SetCode,
                set_rarity = SetData.set_rarity,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            selectedCard = UUID;
            PrintSelectedCard("Last Added Card");

            Debug.WriteLine($"Adding Card {JsonConvert.SerializeObject(YGODataManagement.Inventory.Last(), Formatting.Indented)}");

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
            if (selectedCard is null || !YGODataManagement.Inventory.ContainsKey(selectedCard))
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
            var InventoryObject = YGODataManagement.Inventory[selectedCard];
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

            var IdenticalCards = Utility.GetIdenticalInventory(selectedCard);
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
            SelectedCards.AddRange(Utility.GetIdenticalInventory(selectedCard).Select(x => x));
            for(var i = 0; i < numericUpDown1.Value; i++)
            {
                EditCard(SelectedCards[i], Rarity, Set, Condition, sender == cmbSelectedCardSet);
            }

            PrintSelectedCard(gbSelectedCard.Text, (int)numericUpDown1.Value);
            PrintInventory();

        }

        private void EditCard(string UUID, string NewRarity, string NewSetName, string NewCondition, bool EditingSet)
        {
            var InventoryObject = YGODataManagement.Inventory[UUID];
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

            Debug.WriteLine($"Card Edited {JsonConvert.SerializeObject(YGODataManagement.Inventory[selectedCard], Formatting.Indented)}");
        }

        #endregion Selected Item

        #region FormFunctions
        private void InventoryManager_Load(object sender, EventArgs e)
        {
            cmbFilterBy.SelectedIndex = 0;
            PrintSelectedCard("N/A");
            PrintInventory();
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

            Dictionary<string, dataModel.InventoryEntryData> UniqueEntries = new Dictionary<string, dataModel.InventoryEntryData>();

            foreach (var i in YGODataManagement.Inventory)
            {
                var Card = Utility.GetCardByID(i.Value.cardID);
                var Set = Utility.GetExactCard(Card, i.Value.set_code, i.Value.set_rarity);
                string UUID = $"{Card.name} {Set.set_name} {Set.set_rarity} {i.Value.Condition}";
                if (!UniqueEntries.ContainsKey(UUID))
                {
                    UniqueEntries.Add(UUID, new dataModel.InventoryEntryData { Amount = 0 });
                }
                UniqueEntries[UUID].Amount++;
                UniqueEntries[UUID].Card = Card;
                UniqueEntries[UUID].Set = Set;
                UniqueEntries[UUID].InventoryID = i.Key;
            }

            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (var i in UniqueEntries.Values)
            {
                string[] DisplayData = new string[] { i.Amount.ToString(), i.Card.name, i.Set.set_name, i.Set.GetRarityCode(), BulkData.Conditions[YGODataManagement.Inventory[i.InventoryID].Condition] };
                listView1.Items.Add(Utility.CreateListViewItem(i, DisplayData));
            }
            listView1.EndUpdate();

        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1 || 
                listView1.SelectedItems[0] is null || 
                listView1.SelectedItems[0].Tag is null || 
                listView1.SelectedItems[0].Tag is not dataModel.InventoryEntryData Data) 
            { return; }

            selectedCard = Data.InventoryID;
            PrintSelectedCard("Selected Card");
        }

        #endregion Inventory Display

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            List<string> SelectedCards = Utility.GetIdenticalInventory(selectedCard).ToList();
            if (numericUpDown1.Value > SelectedCards.Count)
            {
                SelectedCards.Add(selectedCard);
                selectedCard = null;
            }
            for (var i = 0; i < numericUpDown1.Value; i++)
            {
                YGODataManagement.Inventory.Remove(SelectedCards[i]);
            }

            PrintSelectedCard(gbSelectedCard.Text);
            PrintInventory();
        }

        private void btnAddOneSelected_Click(object sender, EventArgs e)
        {
            string UUID = Guid.NewGuid().ToString();

            var CurrentCard = YGODataManagement.Inventory[selectedCard];

            YGODataManagement.Inventory.Add(UUID, new dataModel.InventoryDatabaseEntry
            {
                cardID = CurrentCard.cardID,
                set_code = CurrentCard.set_code,
                set_rarity = CurrentCard.set_rarity,
                DateAdded = DateAndTime.Now,
                LastUpdated= DateAndTime.Now
            });

            selectedCard = UUID;
            PrintSelectedCard("Last Added Card");

            Debug.WriteLine($"Adding Card {JsonConvert.SerializeObject(YGODataManagement.Inventory.Last(), Formatting.Indented)}");

            PrintInventory();
        }
    }
}
