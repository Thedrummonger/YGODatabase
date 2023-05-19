using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    internal class SearchParser {
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, string SearchTerm, bool MatchCardName, bool MatchSetCode)
        {
            return CardMatchesFilter(DisplayText, Card, Card.card_sets, SearchTerm, MatchCardName, MatchSetCode);
        }
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, YGOSetData Set, string SearchTerm, bool MatchCardName, bool MatchSetCode)
        {
            return CardMatchesFilter(DisplayText, Card, new YGOSetData[] { Set }, SearchTerm, MatchCardName, MatchSetCode);
        }
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, YGOSetData[] Sets, string SearchTerm, bool MatchCardName, bool MatchSetCode)
        {
            string[] Conditionals = SearchTerm.StringSplit("||");
            foreach (string Conditional in Conditionals)
            {
                string[] Requirments = Conditional.StringSplit("&&");
                if (Requirments.All(x => Compare(x.Trim())))
                {
                    return true;
                }
            }
            return false;

            bool Compare(string SubTerm)
            {
                char[] Modifiers = new char[] { '!', '=' };
                bool Perfect = false;
                bool inverse = false;

                bool HasModifier = Modifiers.Any(x => SubTerm.StartsWith(x));
                while (HasModifier)
                {
                    char Modifier = SubTerm[0];
                    SubTerm = SubTerm[1..];
                    HasModifier = Modifiers.Any(x => SubTerm.StartsWith(x));

                    if (Modifier == '=') { Perfect = true; }
                    else if (Modifier == '!') { inverse = true; }
                }

                if (SubTerm.StartsWith("!")) { inverse = true; SubTerm = SubTerm[1..]; }
                List<string> MatchNames = new List<string>();
                if (MatchCardName)
                {
                    MatchNames.AddRange(Card.SearchTags);
                }
                if (MatchSetCode)
                {
                    foreach (YGOSetData data in Sets??Array.Empty<YGOSetData>())
                    {
                        MatchNames.AddRange(data.SearchTags);
                    }
                }
                if (Perfect) { return MatchNames.Any(x => x == SubTerm.ToLower()) != inverse; }
                return MatchNames.Any(x => x.Contains(SubTerm.ToLower())) != inverse;
            }
        }

    }
}
