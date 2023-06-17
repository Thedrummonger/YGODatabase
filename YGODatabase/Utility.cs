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
            return string.IsNullOrWhiteSpace(setData.set_rarity_code) ? setData.set_rarity : setData.set_rarity_code;
        }
        public static int GetRarityIndex(this YGOSetData setData)
        {
            int Index = BulkData.Rarities.IndexOf(setData.set_rarity);
            return Index < 0 ? 99999 : Index;
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
            if (YGODataManagement.IDLookup.ContainsKey(ID)) { return YGODataManagement.MasterDataBase.data[YGODataManagement.IDLookup[ID]]; }
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

        public static List<string> GetAllSetsContainingCard(this YGOCardOBJ card)
        {
            List<string> sets = new List<string>();
            foreach(var i in card.card_sets??Array.Empty<YGOSetData>())
            {
                if (!sets.Contains(i.set_name)) { sets.Add(i.set_name);}
            }
            return sets;
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

        public static string CleanCardName(this string input, string SpecialCharReplace = "")
        {
            string s1 = Regex.Replace(input, "[^A-Za-z0-9  ]", SpecialCharReplace);
            s1 = Regex.Replace(s1, @"\s+", " ");
            s1 = s1.ToLower();
            return s1;
        }

        public static Guid[] GetIdenticalInventory(Guid InventoryUUID, CardCollection CurrentCollction)
        {
            var Target = CurrentCollction.data[InventoryUUID];
            Guid[] IdenticalEntries = CurrentCollction.data.Where(x => 
                x.Key != InventoryUUID &&
                x.Value.set_rarity == Target.set_rarity &&
                x.Value.set_code == Target.set_code &&
                x.Value.Condition == Target.Condition &&
                x.Value.Category == Target.Category
            ).ToDictionary(x =>x.Key, x=>x.Value).Keys.ToArray();
            return IdenticalEntries??Array.Empty<Guid>();
        }

        public static ListViewItem CreateListViewItem(object Tag, string[] Columns)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems.AddRange(Columns);
            item.SubItems.RemoveAt(0);
            item.Tag = Tag;
            return item;
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
        public static DataModel.Divider CreateDivider(object containerObject, string DividerText = "")
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
                else
                {
                    Debug.WriteLine($"Not all cards were printed in set {SetName}");
                    Debug.WriteLine($"Missing: {JsonConvert.SerializeObject(CardsNotInThisSet.Select(x => x.name).Count())}");
                }
            }
            return CommonSets.ToArray();
        }

        public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }
    }
}
