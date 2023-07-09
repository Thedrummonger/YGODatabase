using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public static class Utility
    {
        public static string[] StringSplit(this string input, string Split, StringSplitOptions options = StringSplitOptions.None)
        {
            return input.Split(new string[] { Split }, options);
        }

        public static bool HasAttack(this YGOCardOBJ card)
        {
            return card.atk > 0 || card.isMonster();
        }
        public static bool HasDefence(this YGOCardOBJ card)
        {
            return card.def > 0 || (card.isMonster() && !card.isLinkMonster());
        }
        public static bool isMonster(this YGOCardOBJ card)
        {
            return card.type.Contains("Monster");
        }
        public static bool isLinkMonster(this YGOCardOBJ card)
        {
            return card.type.Contains("Monster") && card.type.Contains("Link");
        }
        public static bool HasLevel(this YGOCardOBJ card)
        {
            return card.level > 0 || (card.type.Contains("Monster"));
        }
        public static string GetRarityCode(this YGOSetData setData)
        {
            return string.IsNullOrWhiteSpace(setData.set_rarity_code) ? $"({setData.set_rarity})" : setData.set_rarity_code;
        }
        public static int GetRarityIndex(this YGOSetData setData)
        {
            return GetRarityIndex(setData.set_rarity);
        }
        public static int GetRarityIndex(string set_rarity)
        {
            int Index = BulkData.Rarities.IndexOf(set_rarity);
            return Index < 0 ? BulkData.Rarities.Count : Index;
        }
        public static string GetLowestAveragePrice(this YGOCardOBJ card)
        {
            List<decimal> prices = new List<decimal>();
            foreach(var i in card.card_prices)
            {
                ParseAndAdd(i.coolstuffinc_price);
                ParseAndAdd(i.cardmarket_price);
                ParseAndAdd(i.tcgplayer_price);
            }

            void ParseAndAdd(string Price)
            {
                bool Valid = decimal.TryParse(Price, out decimal DecPrice);
                if (!Valid || DecPrice <= 0) { return; }
                prices.Add(DecPrice);
            }
            if (!prices.Any()) { return @"N\A"; }
            return Math.Round(Queryable.Average(prices.AsQueryable()), 2).ToString();
        }

        public static YGOCardOBJ GetCardByID(int ID)
        {
            return GetCardByID(ID, out _);
        }
        public static YGOCardOBJ GetCardByID(int ID, out int ArtID)
        {
            if (YGODataManagement.IDLookup.ContainsKey(ID)) 
            {
                ArtID = YGODataManagement.IDLookup[ID].Item2;
                return YGODataManagement.MasterDataBase.data[YGODataManagement.IDLookup[ID].Item1]; 
            }
            ArtID = 0;
            return null;
        }
        public static YGOSetData GetExactCard(int CardID, string SetCode, string Rarity)
        {
            return GetExactCard(GetCardByID(CardID), SetCode, Rarity);
        }
        public static YGOSetData GetExactCard(YGOCardOBJ Card, string SetCode, string Rarity)
        {
            var Result = Card.card_sets.First(x => x.set_code == SetCode && x.set_rarity == Rarity);
            if (Result is not null) { return Result; }
            Result = Card.card_sets.First(x => x.set_name == SetCode && x.set_rarity == Rarity);
            if (Result is not null) { return Result; }
            Result = Card.card_sets.First(x => x.set_code == SetCode && x.set_rarity_code == Rarity);
            if (Result is not null) { return Result; }
            Result = Card.card_sets.First(x => x.set_name == SetCode && x.set_rarity_code == Rarity);
            if (Result is not null) { return Result; }
            return null;
        }

        public static string[] GetAllSetsContainingCard(this YGOCardOBJ card)
        {
            if (card.card_sets is null || !card.card_sets.Any()) { return Array.Empty<string>(); }
            return card.card_sets.Select(x => x.set_name).Distinct().ToArray();
        }
        public static List<string> GetAllRaritiesInSet(this YGOCardOBJ card, string Setname)
        {
            List<string> Rarities = new List<string>();
            foreach (var i in card.card_sets??Array.Empty<YGOSetData>())
            {
                if (i.set_name != Setname) { continue; }
                if (!Rarities.Contains(i.set_rarity)) { Rarities.Add(i.set_rarity); }
            }
            return Rarities;
        }
        public static string[] GetCommonSets(IEnumerable<YGOCardOBJ> Cards)
        {
            List<string> AllSetNames = new List<string>();
            foreach (var Card in Cards)
            {
                foreach (var set in Card.card_sets)
                {
                    if (!AllSetNames.Contains(set.set_name)) { AllSetNames.Add(set.set_name); }
                }
            }

            List<string> CommonSets = new List<string>();
            foreach (var SetName in AllSetNames)
            {
                var CardsInThisSet = Cards.Where(x => x.card_sets.Any(y => SetName == y.set_name));
                var CardsNotInThisSet = Cards.Where(x => !x.card_sets.Any(y => SetName == y.set_name));

                if (!CardsNotInThisSet.Any())
                {
                    CommonSets.Add(SetName);
                    Debug.WriteLine($"All cards were printed in set {SetName}");
                }
            }
            return CommonSets.ToArray();
        }

        public static string CleanCardName(this string input, string SpecialCharReplace = "")
        {
            string s1 = Regex.Replace(input, "[^A-Za-z0-9  ]", SpecialCharReplace);
            s1 = Regex.Replace(s1, @"\s+", " ");
            s1 = s1.ToLower();
            return s1;
        }
        public static string CreateIDString(this InventoryDatabaseEntry Inventory, CardMatchFilters? filters = null)
        {
            var Filter = filters?? new CardMatchFilters();
            var Card = GetCardByID(Inventory.cardID);
            var Set = GetExactCard(Inventory.cardID, Inventory.set_code, Inventory.set_rarity);
            string ID = $"{Card.name}";
            if (Filter.FilterSet) { ID += $" {Set.set_name}"; }
            if (Filter.FilterRarity) { ID += $" {Set.set_rarity}"; }
            if (Filter.FilterCondition) { ID += $" {Inventory.Condition}"; }
            if (Filter.FilterCategory) { ID += $" {Inventory.Category}"; }
            if (Filter.FilterArt) { ID += $" {Inventory.ImageIndex}"; }
            return ID;
        }
        public static string BuildFileName(string File, int ID, string Dir)
        {
            string Name = File + (ID < 1 ? "" : ID.ToString()) + ".json";
            return Path.Combine(Dir, Name);
        }
        public static string CreateUniqueFilename(string File, string Dir)
        {
            int UniqueID = 0;
            while (Directory.GetFiles(Dir).Contains(BuildFileName(File, UniqueID, Dir)))
            {
                UniqueID++;
            }
            return BuildFileName(File, UniqueID, Dir);
        }

        public static ListViewItem CreateListViewItem(object Tag, string[] Columns, Color? color = null)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems.AddRange(Columns);
            item.SubItems.RemoveAt(0);
            item.Tag = Tag;
            if (color is not null) { item.BackColor = (Color)color; }
            return item;
        }
        public static Divider CreateDivider(object containerObject, string DividerText = "")
        {
            Font font;
            Graphics g;
            int width;
            switch (containerObject)
            {
                case ListView LVcontainer:
                    font = LVcontainer.Font;
                    width = LVcontainer.Width - (LVcontainer.CheckBoxes ? 45 : 0);
                    g = LVcontainer.CreateGraphics();
                    break;
                case ListBox LBcontainer:
                    font = LBcontainer.Font;
                    width = LBcontainer.Width;
                    g = LBcontainer.CreateGraphics();
                    break;
                case ComboBox cmb:
                    font = cmb.Font;
                    width = cmb.Width;
                    g = cmb.CreateGraphics();
                    break;
                default:
                    return new Divider { Display = DividerText };
            }

            string Divider = DividerText;
            while (true)
            {
                string newDivider = Divider;
                if (string.IsNullOrWhiteSpace(DividerText)) { newDivider += "="; }
                else { newDivider = $"={newDivider}="; }
                if ((int)g.MeasureString(newDivider, font).Width < width) { Divider = newDivider; }
                else { break; }
            }
            return new DataModel.Divider { Display = Divider };
        }
        public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }
        public static void ManageNUD(NumericUpDown NUD, int min, int max, int cur)
        {
            NUD.Minimum = min;
            NUD.Maximum = max;
            NUD.Value = cur;
        }

        public static DuplicateCardContainer CreateSelectedCardEntry(CardCollection SelectedCardCollection, Guid SelectedCardID)
        {
            DuplicateCardContainer Container = new();
            Container.InvData = SelectedCardCollection.data[SelectedCardID].Clone();
            Container.Entries = CollectionSearchUtils.GetIdenticalCardsFromCollection(SelectedCardCollection, Container.InvData).ToList();
            return Container;
        }

        public static void ResizeLowerListBox(dynamic listBox, dynamic Form)
        {
            int PaddingSpace = 40;
            listBox.Width = Form.Width - PaddingSpace;
            StretchListBoxHeightToFormBottom(listBox, Form);
        }
        public static void StretchListBoxHeightToFormBottom(dynamic listBox, dynamic Form)
        {
            int PaddingSpace = 40;
            int TitleBarHeight = 10;
            int ListViewHeight = Form.Height - TitleBarHeight;
            listBox.Height = ListViewHeight - listBox.Location.Y - PaddingSpace;
        }

        internal static DomainData GetDomains(YGOCardOBJ cardOBJ)
        {
            DomainData domainData = new DomainData();
            domainData.NameDomains = cardOBJ.name.Split(' ').Where(x => char.IsUpper(x.Trim()[0])).ToList();
            domainData.AdditionalNameDomains = new List<string>();
            domainData.AttributeDomain = cardOBJ.attribute;
            domainData.AdditionalAttributeDomains = new List<string>();
            domainData.MonsterType = new List<string> { cardOBJ.race };

            int ParentheseLevel = 0;
            string CurrentData = string.Empty;
            string SnippetData = string.Empty;
            string QuoteSnippet = string.Empty;
            bool ThisCardIsAlso = false;
            bool InQuote = false;
            foreach (var letter in cardOBJ.desc)
            {
                if (letter == '(') { ParentheseLevel++; }
                if (letter == ')') { ParentheseLevel--; ThisCardIsAlso = false; SnippetData = string.Empty; }
                if (letter == '"') 
                { 
                    InQuote = !InQuote; 
                    if (!InQuote)
                    {
                        domainData.AdditionalNameDomains.AddRange(QuoteSnippet.Trim('"').Split(' ').Where(x => char.IsUpper(x.Trim()[0])).ToList());
                        QuoteSnippet = string.Empty;
                    }
                }
                if (InQuote)
                {
                    QuoteSnippet += letter;
                }

                CurrentData += letter;
                if (CurrentData.EndsWith("card is also"))
                {
                    ThisCardIsAlso = true;
                    SnippetData = string.Empty;
                    continue;
                }

                if (ThisCardIsAlso)
                {
                    SnippetData += letter;
                    if (CurrentData.EndsWith("-Attribute"))
                    {
                        ThisCardIsAlso = false;
                        foreach(var i in string.Concat(SnippetData.Split('-')[0].Where(c => (c >= 'A' && c <= 'Z') || c == ',')).Split(','))
                        {
                            domainData.AdditionalAttributeDomains.Add(i.Trim());
                        }
                        SnippetData = string.Empty;
                    }
                    else if (CurrentData.EndsWith("still a"))
                    {
                        ThisCardIsAlso = false;
                        SnippetData = string.Empty;
                    }
                    else if (CurrentData.EndsWith("\" card"))
                    {
                        ThisCardIsAlso = false;
                        domainData.AdditionalNameDomains.Add(SnippetData.Split('"')[1]);
                        SnippetData = string.Empty;
                    }
                }
            }
            domainData.AdditionalNameDomains = domainData.AdditionalNameDomains.Distinct().ToList();
            domainData.AdditionalAttributeDomains = domainData.AdditionalAttributeDomains.Distinct().ToList();
            return domainData;
        }

        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(pos));
            return null;
        }
        public static void ShowOtherDecksUsingCard(DuplicateCardContainer inventoryObject, List<CardCollection> Collections, int CurrentCollectionInd)
        {
            Dictionary<Guid, Tuple<string, int, int>> DeckCounts = new Dictionary<Guid, Tuple<string, int, int>>();
            foreach (var i in Collections.Where(x => x.UUID != Guid.Empty && x.UUID != Collections[CurrentCollectionInd].UUID))
            {
                var AmountinDeck = CollectionSearchUtils.GetIdenticalCardsFromCollection(i, inventoryObject.InvData, new CardMatchFilters().SetAll(true).Set(_FilterCategory: false));
                var SimilarAmountinDeck = CollectionSearchUtils.GetIdenticalCardsFromCollection(i, inventoryObject.InvData, new CardMatchFilters().SetAll(false));
                DeckCounts[i.UUID] = new(i.Name, AmountinDeck.Count(), SimilarAmountinDeck.Count());
            }
            string Message = $"Decks containing: {inventoryObject.CardData().name} {inventoryObject.SetData().GetRarityCode()} {inventoryObject.SetData().set_name}\n\n";
            foreach (var i in DeckCounts.Values)
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

        public static void ShowOtherAvailablePrinting(DuplicateCardContainer inventoryContainer, List<CardCollection> Collections, int CurrentCollectionInd)
        {
            string TargetCardID = inventoryContainer.InvData.CreateIDString(new CardMatchFilters().SetAll(true).Set(_FilterCategory: false));
            var OtherPrintings = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], inventoryContainer.InvData, new CardMatchFilters().SetAll(false)); ;
            Dictionary<string, Tuple<string, int, int>> AltPrintings = new();
            foreach (var i in OtherPrintings)
            {
                var Entry = Collections[0].data[i];
                string CardID = Entry.CreateIDString(new CardMatchFilters().SetAll(true).Set(_FilterCategory: false));
                if (CardID == TargetCardID) { continue; }
                var IDString = Entry.CreateIDString();
                var Card = Utility.GetCardByID(Entry.cardID);
                var Set = Utility.GetExactCard(Card, Entry.set_code, Entry.set_rarity);
                var OtherDecks = CollectionSearchUtils.GetAmountOfCardInNonInventoryCollections(Collections, inventoryContainer.InvData, new HashSet<int> { CurrentCollectionInd }, new CardMatchFilters(), true);
                if (!AltPrintings.ContainsKey(IDString)) { AltPrintings[IDString] = new($"{Set.set_name} {Set.GetRarityCode()} ({Entry.Condition}) (Art{Entry.ImageIndex+1})", 0, OtherDecks); }
                AltPrintings[IDString] = new(AltPrintings[IDString].Item1, AltPrintings[IDString].Item2 + 1, AltPrintings[IDString].Item3);
            }
            var Available = AltPrintings.Where(x => x.Value.Item3 < 1).Select(x => $"{x.Value.Item2 - x.Value.Item3}X {x.Value.Item1}").ToArray();
            var Shareable = AltPrintings.Where(x => x.Value.Item3 > 0).Select(x => $"{x.Value.Item2}X {x.Value.Item1}");

            string Message = $"Printings available for use in this deck:\n{string.Join('\n', Available)}";
            if (Shareable.Any()) { Message += $"\n\nPrintings available for sharing with other decks\n{string.Join('\n', Shareable)}"; }
            MessageBox.Show(Message);
        }
    }
}
