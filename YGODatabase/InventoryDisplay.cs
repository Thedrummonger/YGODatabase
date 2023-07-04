using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class InventoryDisplay : Form
    {
        public InventoryManager _Parent;
        public int CurrentDisplayCollectionInd;
        bool Initializing = false;
        public InventoryDisplay(InventoryManager Parent)
        {
            Initializing = true;
            InitializeComponent();
            _Parent = Parent;
            CurrentDisplayCollectionInd = Parent.CurrentCollectionInd;
            cmbOrderBy.SelectedIndex = 0;
            Initializing = false;
        }
        private void UpdateCollectionDetails(object sender, EventArgs e)
        {
            if (Initializing) { return; }
            CurrentDisplayCollectionInd = comboBox1.SelectedIndex;
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
            PrintInventoryData(listView1, groupBox1, _Parent.Collections, CurrentDisplayCollectionInd, txtInventoryFilter.Text, cmbOrderBy.SelectedIndex, chkInvDescending.Checked);
        }

        public static void PrintInventoryData(System.Windows.Forms.ListView LVTarget, dynamic Display, List<CardCollection> Collections, int CurrentCollectionInd, string Filter, int OrderInd, bool OrderDescending)
        {

            int topItemIndex = 0;
            try { topItemIndex = LVTarget.TopItem?.Index??0; }
            catch (Exception ex) { }

            bool MainInventory = Collections[CurrentCollectionInd].IsInventory();

            Debug.WriteLine($"Printing {Collections[CurrentCollectionInd].Name}");
            Dictionary<string, DuplicateCardContainer> UniqueEntries = new Dictionary<string, DuplicateCardContainer>();

            foreach (var i in Collections[CurrentCollectionInd].data)
            {
                string InventoryID = i.Value.CreateIDString();

                if (!UniqueEntries.ContainsKey(InventoryID))
                {
                    DuplicateCardContainer Container = new();
                    Container.Entries = new List<Guid>();
                    Container.InvData = i.Value.Clone();
                    UniqueEntries.Add(InventoryID, Container);
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
                LVTarget.Columns.Add("D", 25);
                LVTarget.Columns.Add("Card", 276);
                LVTarget.Columns.Add("Set", 145);
                LVTarget.Columns.Add("Rarity", 42);
                LVTarget.Columns.Add("Con", 45);
            }
            else
            {
                LVTarget.Columns.Add("#", 20);
                LVTarget.Columns.Add("D", 25);
                LVTarget.Columns.Add("I", 25);
                LVTarget.Columns.Add("Card", 267);
                LVTarget.Columns.Add("Set", 128);
                LVTarget.Columns.Add("Rarity", 42);
                LVTarget.Columns.Add("Con", 45);
            }

            Categories CurrentCategory = Categories.None;

            var OtherDeckSearchFilter = new CardMatchFilters().SetAll(true).Set(_FilterCategory: false);

            var CollectionIDCache = CollectionSearchUtils.CacheIdsInCollections(Collections, new HashSet<int> { CurrentCollectionInd }, OtherDeckSearchFilter);
            
            int PrintsDone = 0;
            foreach (var i in PrintList)
            {
                bool SearchValid = SearchParser.CardMatchesFilter($"", i.CardData(), i.SetData(), Filter, true, true);
                if (!SearchValid) { continue; }

                var OtherDecks = CollectionSearchUtils.GetAmountOfCardInNonInventoryCollections(CollectionIDCache, Collections, i.InvData, new HashSet<int> { CurrentCollectionInd }, OtherDeckSearchFilter, true);

                CollectionShownCount += i.CardCount();
                int ImageInd = i.InvData.ImageIndex;
                List<string> DisplayData = new List<string> { i.CardCount().ToString(), OtherDecks.ToString(), i.CardData().name + (ImageInd > 0 ? $" ({ImageInd +1 })" : ""), i.SetData().set_name, i.SetData().GetRarityCode(), BulkData.Conditions[i.InvData.Condition] };
                
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
                    var InInventory = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], i.InvData, new CardMatchFilters().SetAll(true).Set(_FilterCategory: false));
                    var InInventorySimilar = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], i.InvData, new CardMatchFilters().SetAll(false));
                    DisplayData.Insert(2, InInventory.Count().ToString());
                    if (i.CardCount() > InInventorySimilar.Count()) { BackColor = Color.LightCoral; }                //No cards available including other printings
                    else if (i.CardCount() > InInventory.Count()) { BackColor = Color.LightPink; }                   //No cards of the exact printing available
                    else if ((i.CardCount() + OtherDecks) > InInventory.Count()) { BackColor = Color.LightYellow; }  //Cards are available but must be shared between decks
                }
                LVTarget.Items.Add(Utility.CreateListViewItem(i, DisplayData.ToArray(), BackColor));
                if (CategoryHeaderEdits.ContainsKey(CurrentCategory)) { CategoryHeaderEdits[CurrentCategory] = new(CategoryHeaderEdits[CurrentCategory].Item1, CategoryHeaderEdits[CurrentCategory].Item2+i.CardCount()); }
                InvListPos++;
                PrintsDone++;
            }

            Debug.WriteLine($"Printed {PrintsDone} Cards");

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
        private async void UpdatepictureBox(YGOCardOBJ card, int ImageIndex)
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

        private void InventoryDisplay_ResizeEnd(object sender, EventArgs e)
        {
            Utility.ResizeLowerListBox(listView1, this);
        }

        public static void SortColumnByClick(int ColumnInd, CheckBox DecensingCheckBox, ComboBox OrderByCMB, List<CardCollection> Collections, int CurrentCollectionInd)
        {
            int MainInvOffset = Collections[CurrentCollectionInd].IsInventory() ? 0 : 1;
            Dictionary<int, int> SortKeyInd = new Dictionary<int, int>()
            {
                { 0, 7 },                   //Sort By Count
                { 2 + MainInvOffset, 0 },   //Sort By Name
                { 3 + MainInvOffset, 1 },   //Sort by set
                { 4 + MainInvOffset, 2 },   //Sort by rarity
                { 5 + MainInvOffset, 3 },   //Sort by Condition
            };
            if (SortKeyInd.ContainsKey(ColumnInd))
            {
                if (OrderByCMB.SelectedIndex == SortKeyInd[ColumnInd])
                {
                    DecensingCheckBox.Checked = !DecensingCheckBox.Checked;
                }
                else
                {
                    OrderByCMB.SelectedIndex = SortKeyInd[ColumnInd];
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortColumnByClick(e.Column, chkInvDescending, cmbOrderBy, _Parent.Collections, CurrentDisplayCollectionInd);
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

        private void ShowContextMenu(ListViewItem SelectedEntry)
        {
            if (SelectedEntry.Tag is null) { return; }

            ContextMenuStrip contextMenu = new();
            ToolStripItem RefreshContextItem = contextMenu.Items.Add("Refresh");
            RefreshContextItem.Click += (sender, e) => { UpdateData(false); };

            if (SelectedEntry.Tag is DuplicateCardContainer inventoryObject)
            {
                if (!_Parent.Collections[CurrentDisplayCollectionInd].IsInventory())
                {
                    ToolStripItem ShowAltPrintings = contextMenu.Items.Add("Show other available printings");
                    ShowAltPrintings.Click += (sender, e) => { Utility.ShowOtherAvailablePrinting(inventoryObject, _Parent.Collections, CurrentDisplayCollectionInd); };
                }
                ToolStripItem ShowOtherdecks = contextMenu.Items.Add("Show other decks using card");
                ShowOtherdecks.Click += (sender, e) => { Utility.ShowOtherDecksUsingCard(inventoryObject, _Parent.Collections, CurrentDisplayCollectionInd); };
            }
            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(Cursor.Position);
            }
        }
    }
}
