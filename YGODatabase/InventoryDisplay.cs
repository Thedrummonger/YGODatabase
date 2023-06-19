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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class InventoryDisplay : Form
    {
        public InventoryManager _Parent;
        public int CurrentCollectionInd;
        bool Initializing = false;
        public InventoryDisplay(InventoryManager Parent)
        {
            Initializing = true;
            InitializeComponent();
            _Parent = Parent;
            CurrentCollectionInd = Parent.CurrentCollectionInd;
            cmbOrderBy.SelectedIndex = 0;
            Initializing = false;
        }
        private void UpdateCollectionDetails(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            CurrentCollectionInd = comboBox1.SelectedIndex;
            UpdateData(false);
        }
        public void UpdateData(bool CollectionsChanged)
        {
            if (CollectionsChanged)
            {
                comboBox1.Items.Clear();
                foreach (CardCollection c in _Parent.Collections)
                {
                    comboBox1.Items.Add(c.Name);
                }
                comboBox1.SelectedIndex = 0;
            }
            PrintInventoryData(listView1, groupBox1, _Parent.Collections, CurrentCollectionInd, txtInventoryFilter.Text, cmbOrderBy.SelectedIndex, chkInvDescending.Checked);
        }

        public static void PrintInventoryData(System.Windows.Forms.ListView LVTarget, dynamic Display, List<CardCollection> Collections, int CurrentCollectionInd, string Filter, int OrderInd, bool OrderDescending) 
        {

            int topItemIndex = 0;
            try { topItemIndex = LVTarget.TopItem?.Index??0; }
            catch (Exception ex) { }

            bool MainInventory = CurrentCollectionInd < 1;

            Debug.WriteLine($"Printing {Collections[CurrentCollectionInd].Name}");
            Dictionary<string, DataModel.InventoryObject> UniqueEntries = new Dictionary<string, DataModel.InventoryObject>();

            foreach (var i in Collections[CurrentCollectionInd].data)
            {
                var Card = Utility.GetCardByID(i.Value.cardID);
                var Set = Utility.GetExactCard(Card, i.Value.set_code, i.Value.set_rarity);
                string InventoryID = i.Value.CreateIDString();

                if (!UniqueEntries.ContainsKey(InventoryID))
                {
                    UniqueEntries.Add(InventoryID, new DataModel.InventoryObject { Amount = 0 });
                }
                UniqueEntries[InventoryID].Amount++;
                UniqueEntries[InventoryID].Card = Card;
                UniqueEntries[InventoryID].Set = Set;
                UniqueEntries[InventoryID].InventoryID = i.Key;
            }

            LVTarget.BeginUpdate();
            LVTarget.Items.Clear();

            List<int> SortPriority = new List<int>()
            {
                0, //Name
                1, //Set
                2, //Rarity
                3, //Condition
                4, //Added
                5  //modified
                //6 //Card Type
                //7 //Card Count
            };

            int MainPriority = OrderInd;
            if (MainPriority >= 0 && MainPriority < SortPriority.Count)
            {
                SortPriority.MoveItemAtIndexToFront(SortPriority.IndexOf(OrderInd));
            }
            else if (MainPriority >= 0)
            {
                SortPriority.Insert(0, OrderInd);
            }

            IOrderedEnumerable<InventoryObject> PrintList = UniqueEntries.Values.OrderBy(x => Collections[CurrentCollectionInd].data[x.InventoryID].Category);

            foreach (var i in SortPriority)
            {
                switch (i)
                {
                    case 0:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.Card.name) : PrintList.ThenBy(x => x.Card.name);
                        break;
                    case 1:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.Set.set_name) : PrintList.ThenBy(x => x.Set.set_name);
                        break;
                    case 2:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.Set.GetRarityIndex()) : PrintList.ThenBy(x => x.Set.GetRarityIndex());
                        break;
                    case 3:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]) : PrintList.ThenBy(x => BulkData.Conditions[Collections[CurrentCollectionInd].data[x.InventoryID].Condition]);
                        break;
                    case 4:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => Collections[CurrentCollectionInd].data[x.InventoryID].DateAdded) : PrintList.ThenBy(x => Collections[CurrentCollectionInd].data[x.InventoryID].DateAdded);
                        break;
                    case 5:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => Collections[CurrentCollectionInd].data[x.InventoryID].LastUpdated) : PrintList.ThenBy(x => Collections[CurrentCollectionInd].data[x.InventoryID].LastUpdated);
                        break;
                    case 6:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.Card.type) : PrintList.ThenBy(x => x.Card.type);
                        break;
                    case 7:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.Amount) : PrintList.ThenBy(x => x.Amount);
                        break;
                }
            }

            int CollectionCount = Collections[CurrentCollectionInd].data.Count;
            int CollectionShownCount = 0;

            int InvListPos = 0;
            Dictionary<Categories, Tuple<int, int>> CategoryHeaderEdits = new Dictionary<Categories, Tuple<int, int>>();

            LVTarget.Columns.Clear();
            if (MainInventory)
            {
                LVTarget.Columns.Add("#", 20);
                LVTarget.Columns.Add("Card", 296);
                LVTarget.Columns.Add("Set", 150);
                LVTarget.Columns.Add("Rarity", 42);
                LVTarget.Columns.Add("Con", 45);
            }
            else
            {
                LVTarget.Columns.Add("#", 20);
                LVTarget.Columns.Add("D", 20);
                LVTarget.Columns.Add("I", 20);
                LVTarget.Columns.Add("Card", 267);
                LVTarget.Columns.Add("Set", 138);
                LVTarget.Columns.Add("Rarity", 42);
                LVTarget.Columns.Add("Con", 45);
            }

            Categories CurrentCategory = Categories.None;
            foreach (var i in PrintList)
            {
                bool SearchValid = SearchParser.CardMatchesFilter($"{i.Card.name} {i.Set.set_name} {i.Set.set_rarity}", i.Card, i.Set, Filter, true, true);
                if (!SearchValid) { continue; }

                CollectionShownCount += i.Amount;
                List<string> DisplayData = new List<string> { i.Amount.ToString(), i.Card.name, i.Set.set_name, i.Set.GetRarityCode(), BulkData.Conditions[Collections[CurrentCollectionInd].data[i.InventoryID].Condition] };
                Color? BackColor = null;
                if (!MainInventory)
                {
                    if (Collections[CurrentCollectionInd].data[i.InventoryID].Category != CurrentCategory)
                    {
                        CurrentCategory = Collections[CurrentCollectionInd].data[i.InventoryID].Category;
                        List<string> CategoryHeader = new List<string> { "#", "", "", CategoryNames[CurrentCategory].ToUpper() + " DECK:", "", "", "" };
                        LVTarget.Items.Add(Utility.CreateListViewItem(null, CategoryHeader.ToArray(), Color.DarkGray));
                        CategoryHeaderEdits[CurrentCategory] = new(InvListPos, 0);
                        InvListPos++;
                    }
                    var OtherDecks = SmartCardSetSelector.GetAmountOfCardInOtherDecks(i.Card, Collections, CurrentCollectionInd, i.Set.set_code, i.Set.set_rarity, true);
                    var InInventory = SmartCardSetSelector.GetCardsFromInventory(i.Card, Collections[0], i.Set.set_code, i.Set.set_rarity);
                    var InInventorySimilar = SmartCardSetSelector.GetCardsFromInventory(i.Card, Collections[0]);
                    DisplayData.Insert(1, InInventory.Count().ToString());
                    DisplayData.Insert(1, OtherDecks.ToString());
                    if (i.Amount > InInventorySimilar.Count()) { BackColor = Color.LightCoral; }
                    else if (i.Amount > InInventory.Count()) { BackColor = Color.LightPink; }
                    else if ((i.Amount + OtherDecks) > InInventory.Count()) { BackColor = Color.LightYellow; }
                }
                LVTarget.Items.Add(Utility.CreateListViewItem(i, DisplayData.ToArray(), BackColor));
                if (CategoryHeaderEdits.ContainsKey(CurrentCategory)) { CategoryHeaderEdits[CurrentCategory] = new(CategoryHeaderEdits[CurrentCategory].Item1, CategoryHeaderEdits[CurrentCategory].Item2+i.Amount); }
                InvListPos++;
            }

            foreach (var item in CategoryHeaderEdits.Values)
            {
                var OldHeader = LVTarget.Items[item.Item1];
                OldHeader.SubItems[0] = new ListViewItem.ListViewSubItem { Text = item.Item2.ToString() };
                OldHeader.BackColor = Color.LightGray;
                LVTarget.Items[item.Item1] = OldHeader;
            }
            LVTarget.EndUpdate();

            try { LVTarget.TopItem = LVTarget.Items[topItemIndex]; }
            catch (Exception ex) { }

            Display.Text = $"Current Collection: {CollectionShownCount}\\{CollectionCount}";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ImageIndex = 0;
            if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is DataModel.InventoryObject InventorySelectedCard)
            {
                richTextBox1.Text = InventorySelectedCard.Card.desc;
                ImageIndex = _Parent.Collections[CurrentCollectionInd].data[InventorySelectedCard.InventoryID].ImageIndex; 
            }
            else { return; }
            UpdatepictureBox(InventorySelectedCard.Card, ImageIndex);
            UpdateCardInfo(InventorySelectedCard.Card, InventorySelectedCard.Set);
        }
        private async void UpdatepictureBox(DataModel.YGOCardOBJ card, int ImageIndex)
        {
            await Task.Run(() => pictureBox1.Image = YGODataManagement.GetImage(card, ImageIndex, YGODataManagement.ImageType.standard));
        }

        private void UpdateCardInfo(YGOCardOBJ CurrentCard, YGOSetData? set)
        {
            listBox1.Items.Clear();

            listBox1.Items.Add("Type: " + CurrentCard.type);
            if (CurrentCard.HasAttack()) { listBox1.Items.Add("Attack: " + CurrentCard.atk); }
            if (CurrentCard.HasDefence()) { listBox1.Items.Add("Defense: " + CurrentCard.def); }
            if (CurrentCard.HasLevel()) { listBox1.Items.Add("Level: " + CurrentCard.level); }
            if (CurrentCard.race is not null) { listBox1.Items.Add("Race: " + CurrentCard.race); }
            if (CurrentCard.attribute is not null) { listBox1.Items.Add("Attribute: " + CurrentCard.attribute); }
            listBox1.Items.Add(Utility.CreateDivider(listBox1, "Price"));
            listBox1.Items.Add("Average: " + CurrentCard.GetLowestAveragePrice());
            listBox1.Items.Add("TCGPlayer: " + CurrentCard.card_prices.First().tcgplayer_price);
            listBox1.Items.Add("Card Market: " + CurrentCard.card_prices.First().cardmarket_price);
            listBox1.Items.Add("Cool Stuff Inc: " + CurrentCard.card_prices.First().coolstuffinc_price);
        }
    }
}
