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
            PrintSelectedCard();

            Debug.WriteLine($"Adding Card {JsonConvert.SerializeObject(YGODataManagement.Inventory.Last(), Formatting.Indented)}");

            PrintInventory();

        }

        #endregion Search Functions

        #region Selected Item

        bool SelectedCardUpdating = false;
        private void PrintSelectedCard()
        {
            SelectedCardUpdating = true;
            cmbSelctedCardRarity.Enabled = true;
            cmbSelectedCardSet.Enabled = true;
            cmbSelectedCardCondition.Enabled = true;
            if (selectedCard is null || !YGODataManagement.Inventory.ContainsKey(selectedCard))
            {
                lblSelectedCard.Text = "Name:";
                lblSelectedCardAdded.Text = "Date Added:";
                lblSelectedCardModified.Text = "Last Modified:";
                cmbSelctedCardRarity.DataSource = null;
                cmbSelectedCardSet.DataSource = null;
                cmbSelectedCardCondition.DataSource = null;

                cmbSelctedCardRarity.Enabled = false;
                cmbSelectedCardSet.Enabled = false;
                cmbSelectedCardCondition.Enabled = false;
                SelectedCardUpdating = false;
                return;
            }
            var InventoryObject = YGODataManagement.Inventory[selectedCard];
            var Card = Utility.GetCardByID(InventoryObject.cardID);
            var SetEntry = Card.card_sets.First(x => x.set_code == InventoryObject.set_code && x.set_rarity == InventoryObject.set_rarity);
            lblSelectedCard.Text = $"Name: {Card.name}";
            lblSelectedCardAdded.Text = $"Date Added:\n{InventoryObject.DateAdded}";
            lblSelectedCardModified.Text = $"Last Modified:\n{InventoryObject.LastUpdated}";
            cmbSelectedCardSet.DataSource = Card.GetAllSetsContainingCard();
            foreach (var i in cmbSelectedCardSet.Items) { if (i.ToString() == SetEntry.set_name) { cmbSelectedCardSet.SelectedItem = i; break; } }
            cmbSelctedCardRarity.DataSource = Card.GetAllRaritiesInSet(SetEntry.set_name);
            foreach (var i in cmbSelctedCardRarity.Items) { if (i.ToString() == SetEntry.set_rarity) { cmbSelctedCardRarity.SelectedItem = i; break; } }
            cmbSelectedCardCondition.DataSource = BulkData.Conditions;
            foreach (var i in cmbSelectedCardCondition.Items) { if (i.ToString() == InventoryObject.Condition) { cmbSelectedCardCondition.SelectedItem = i; break; } }

            SelectedCardUpdating = false;
        }
        private void SelectedCardValueEdited(object sender, EventArgs e)
        {
            if (SelectedCardUpdating) { return; }
            var InventoryObject = YGODataManagement.Inventory[selectedCard];
            var Card = Utility.GetCardByID(InventoryObject.cardID);
            var OldSetEntry = Card.card_sets.First(x => x.set_code == InventoryObject.set_code && x.set_rarity == InventoryObject.set_rarity);

            string NewRarity = (string)cmbSelctedCardRarity.SelectedItem;
            string NewSetName = (string)cmbSelectedCardSet.SelectedItem;
            string NewCondition = (string)cmbSelectedCardCondition.SelectedItem;

            if (sender == cmbSelectedCardSet)
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

            PrintSelectedCard();

        }

        #endregion Selected Item

        #region FormFunctions
        private void InventoryManager_Load(object sender, EventArgs e)
        {
            cmbFilterBy.SelectedIndex = 0;
            PrintSelectedCard();
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
            Dictionary<string, dataModel.InventoryListEntry> entries = new Dictionary<string, dataModel.InventoryListEntry>();
            foreach (var i in YGODataManagement.Inventory.Values)
            {
                dataModel.YGOCardOBJ Card = Utility.GetCardByID(i.cardID);
                string Display = Card.name;
                if (!entries.ContainsKey(Display)) { entries.Add(Display, new dataModel.InventoryListEntry { Amount = 0 }); }

                entries[Display].DisplayName = Display;
                entries[Display].Amount++;
                entries[Display].CardID = i.cardID;
            }
            lbInventory.DataSource = entries.Values.Cast<dataModel.InventoryListEntry>().ToList();
        }

        #endregion Inventory Display
    }
}
