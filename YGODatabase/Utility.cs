using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YGODatabase.dataModel;

namespace YGODatabase
{
    public static class Utility
    {
        public static class LevenshteinDistance
        {
            public static int Compute(string s, string t)
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (string.IsNullOrEmpty(t))
                        return 0;
                    return t.Length;
                }

                if (string.IsNullOrEmpty(t))
                {
                    return s.Length;
                }

                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                for (int i = 0; i <= n; d[i, 0] = i++) ;
                for (int j = 1; j <= m; d[0, j] = j++) ;

                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= m; j++)
                    {
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                        int min1 = d[i - 1, j] + 1;
                        int min2 = d[i, j - 1] + 1;
                        int min3 = d[i - 1, j - 1] + cost;
                        d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                    }
                }
                return d[n, m];
            }
        }

        public static bool ValidSearchCard(YGOCardOBJ card, string Search, int Similarity = 10)
        {
            int LasDist = LevenshteinDistance.Compute(card.name, Search);
            return LasDist >= Similarity;
        }

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

        public static List<string> CardTypes = new List<string> {
          "Spell Card",
          "Effect Monster",
          "Normal Monster",
          "Flip Effect Monster",
          "Trap Card",
          "Union Effect Monster",
          "Fusion Monster",
          "Pendulum Effect Monster",
          "Link Monster",
          "XYZ Monster",
          "Synchro Monster",
          "Synchro Tuner Monster",
          "Tuner Monster",
          "Gemini Monster",
          "Normal Tuner Monster",
          "Spirit Monster",
          "Ritual Effect Monster",
          "Skill Card",
          "Token",
          "Pendulum Effect Fusion Monster",
          "Ritual Monster",
          "Toon Monster",
          "Pendulum Normal Monster",
          "Synchro Pendulum Effect Monster",
          "Pendulum Tuner Effect Monster",
          "XYZ Pendulum Effect Monster",
          "Pendulum Effect Ritual Monster",
          "Pendulum Flip Effect Monster"
        };
     }
}
