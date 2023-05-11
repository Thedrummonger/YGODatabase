using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static YGODatabase.dataModel;

namespace YGODatabase
{
    public static class Utility
    {

        public static bool HasAttack(this YGOCardOBJ card)
        {
            return card.atk > 0 || card.type.Contains("Monster");
        }
        public static bool HasDefence(this YGOCardOBJ card)
        {
            return card.def > 0 || (card.type.Contains("Monster") && !card.type.Contains("Link"));
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
                ParseAndAdd(i.amazon_price);
                ParseAndAdd(i.coolstuffinc_price);
                ParseAndAdd(i.cardmarket_price);
                ParseAndAdd(i.tcgplayer_price);
                ParseAndAdd(i.ebay_price);
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

        public static string GetLowestRarity(this YGOCardOBJ card, string SetName)
        {
            string LowestRarity = null;
            foreach(var i in card.card_sets.Where(x => x.set_name == SetName))
            {

            }
            return LowestRarity;
        }

        public static YGOCardOBJ GetCardByID(int ID)
        {
            return YGODataManagement.MasterDataBase.data[YGODataManagement.IDLookup[ID]];
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

        public static string CleanCardName(this string input)
        {
            string s1 = Regex.Replace(input, "[^A-Za-z0-9  ]", "");
            s1 = Regex.Replace(s1, @"\s+", " ");
            s1 = s1.ToLower();
            return s1;
        }

        public static string[] GetIdenticalInventory(string InventoryUUID)
        {
            var Target = YGODataManagement.Inventory[InventoryUUID];
            string[] IdenticalEntries = YGODataManagement.Inventory.Where(x => 
                x.Key != InventoryUUID &&
                x.Value.set_rarity == Target.set_rarity &&
                x.Value.set_code == Target.set_code &&
                x.Value.Condition == Target.Condition
            ).ToDictionary(x =>x.Key, x=>x.Value).Keys.ToArray();
            return IdenticalEntries??Array.Empty<string>();
        }

        public static ListViewItem CreateListViewItem(object Tag, string[] Columns)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems.AddRange(Columns);
            item.SubItems.RemoveAt(0);
            item.Tag = Tag;
            return item;
        }
    }
}
