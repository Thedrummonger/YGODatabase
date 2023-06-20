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
            Dictionary<string, DuplicateCardContainer> UniqueEntries = new Dictionary<string, DataModel.DuplicateCardContainer>();

            foreach (var i in Collections[CurrentCollectionInd].data)
            {
                var Card = Utility.GetCardByID(i.Value.cardID);
                var Set = Utility.GetExactCard(Card, i.Value.set_code, i.Value.set_rarity);
                string InventoryID = i.Value.CreateIDString();

                if (!UniqueEntries.ContainsKey(InventoryID))
                {
                    UniqueEntries.Add(InventoryID, new DuplicateCardContainer(Collections[CurrentCollectionInd].UUID) { Entries = new List<Guid>()});
                    UniqueEntries[InventoryID].InvData = new InventoryDatabaseEntry(Collections[CurrentCollectionInd].UUID)
                    {
                        DateAdded = i.Value.DateAdded,
                        LastUpdated = i.Value.LastUpdated,
                        cardID = i.Value.cardID,
                        Category = i.Value.Category,
                        Condition = i.Value.Condition,
                        ImageIndex = i.Value.ImageIndex,
                        Language = i.Value.Language,
                        ParentCollectionID = i.Value.ParentCollectionID,
                        set_code = i.Value.set_code,
                        set_rarity = i.Value.set_rarity
                    };
                }
                UniqueEntries[InventoryID].Entries.Add(i.Key);
                UniqueEntries[InventoryID].InvData.DateAdded = UniqueEntries[InventoryID].InvData.DateAdded >= i.Value.DateAdded ? UniqueEntries[InventoryID].InvData.DateAdded : i.Value.DateAdded;
                UniqueEntries[InventoryID].InvData.LastUpdated = UniqueEntries[InventoryID].InvData.LastUpdated >= i.Value.LastUpdated ? UniqueEntries[InventoryID].InvData.LastUpdated : i.Value.LastUpdated;
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

            IOrderedEnumerable<DuplicateCardContainer> PrintList = UniqueEntries.Values.OrderBy(x => x.InvData.Category);

            foreach (var i in SortPriority)
            {
                switch (i)
                {
                    case 0:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.CardData().name) : PrintList.ThenBy(x => x.CardData().name);
                        break;
                    case 1:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.SetData().set_name) : PrintList.ThenBy(x => x.SetData().set_name);
                        break;
                    case 2:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.SetData().GetRarityIndex()) : PrintList.ThenBy(x => x.SetData().GetRarityIndex());
                        break;
                    case 3:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => BulkData.Conditions[x.InvData.Condition]) : PrintList.ThenBy(x => BulkData.Conditions[x.InvData.Condition]);
                        break;
                    case 4:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.InvData.DateAdded) : PrintList.ThenBy(x => x.InvData.DateAdded);
                        break;
                    case 5:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.InvData.LastUpdated) : PrintList.ThenBy(x => x.InvData.LastUpdated);
                        break;
                    case 6:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.CardData().type) : PrintList.ThenBy(x => x.CardData().type);
                        break;
                    case 7:
                        PrintList = OrderDescending ? PrintList.ThenByDescending(x => x.CardCount()) : PrintList.ThenBy(x => x.CardCount());
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
                bool SearchValid = SearchParser.CardMatchesFilter($"", i.CardData(), i.SetData(), Filter, true, true);
                if (!SearchValid) { continue; }

                CollectionShownCount += i.CardCount();
                int ImageInd = i.InvData.ImageIndex;
                List<string> DisplayData = new List<string> { i.CardCount().ToString(), i.CardData().name + (ImageInd > 0 ? $" ({ImageInd +1 })" : ""), i.SetData().set_name, i.SetData().GetRarityCode(), BulkData.Conditions[i.InvData.Condition] };
                Color? BackColor = null;
                if (!MainInventory)
                {
                    if (i.InvData.Category != CurrentCategory)
                    {
                        CurrentCategory = i.InvData.Category;
                        List<string> CategoryHeader = new List<string> { "#", "", "", CategoryNames[CurrentCategory].ToUpper() + " DECK:", "", "", "" };
                        LVTarget.Items.Add(Utility.CreateListViewItem(null, CategoryHeader.ToArray()));
                        CategoryHeaderEdits[CurrentCategory] = new(InvListPos, 0);
                        InvListPos++;
                    }
                    var OtherDecks = SmartCardSetSelector.GetAmountOfCardInOtherDecks(i.CardData(), Collections, CurrentCollectionInd, i.SetData().set_code, i.SetData().set_rarity, true);
                    var InInventory = SmartCardSetSelector.GetCardsFromInventory(i.CardData(), Collections[0], i.SetData().set_code, i.SetData().set_rarity);
                    var InInventorySimilar = SmartCardSetSelector.GetCardsFromInventory(i.CardData(), Collections[0]);
                    DisplayData.Insert(1, InInventory.Count().ToString());
                    DisplayData.Insert(1, OtherDecks.ToString());
                    if (i.CardCount() > InInventorySimilar.Count()) { BackColor = Color.LightCoral; }                //No cards available including other printings
                    else if (i.CardCount() > InInventory.Count()) { BackColor = Color.LightPink; }                   //No cards of the exact printing available
                    else if ((i.CardCount() + OtherDecks) > InInventory.Count()) { BackColor = Color.LightYellow; }  //Cards are available but must be shared between decks
                }
                LVTarget.Items.Add(Utility.CreateListViewItem(i, DisplayData.ToArray(), BackColor));
                if (CategoryHeaderEdits.ContainsKey(CurrentCategory)) { CategoryHeaderEdits[CurrentCategory] = new(CategoryHeaderEdits[CurrentCategory].Item1, CategoryHeaderEdits[CurrentCategory].Item2+i.CardCount()); }
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
            if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is DataModel.DuplicateCardContainer InventorySelectedCard)
            {
                richTextBox1.Text = InventorySelectedCard.CardData().desc;
                ImageIndex = InventorySelectedCard.InvData.ImageIndex; 
            }
            else { return; }
            UpdatepictureBox(InventorySelectedCard.CardData(), ImageIndex);
            UpdateCardInfo(InventorySelectedCard.CardData(), InventorySelectedCard.SetData());
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
