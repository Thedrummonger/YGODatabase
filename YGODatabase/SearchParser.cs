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
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, string SearchTerm, bool MatchDisplay, bool MatchCardName, bool MatchSetCode)
        {
            return CardMatchesFilter(DisplayText, Card, Card.card_sets, SearchTerm, MatchDisplay, MatchCardName, MatchSetCode);
        }
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, YGOSetData Set, string SearchTerm, bool MatchDisplay, bool MatchCardName, bool MatchSetCode)
        {
            return CardMatchesFilter(DisplayText, Card, new YGOSetData[] { Set }, SearchTerm, MatchDisplay, MatchCardName, MatchSetCode);
        }
        public static bool CardMatchesFilter(string DisplayText, YGOCardOBJ Card, YGOSetData[] Sets, string SearchTerm, bool MatchDisplay, bool MatchCardName, bool MatchSetCode)
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
                if (MatchDisplay)
                {
                    MatchNames.Add(DisplayText.ToLower());
                    MatchNames.Add(DisplayText.CleanCardName());
                    MatchNames.Add(DisplayText.CleanCardName(" "));
                }
                if (MatchCardName)
                {
                    MatchNames.Add(Card.name.ToLower());
                    MatchNames.Add(Card.name.CleanCardName());
                    MatchNames.Add(Card.name.CleanCardName(" "));
                }
                if (MatchSetCode)
                {
                    foreach (YGOSetData data in Sets??Array.Empty<YGOSetData>())
                    {
                        MatchNames.Add(data.set_code.ToLower());
                        MatchNames.Add(data.set_code.CleanCardName());
                    }
                }
                if (Perfect) { return MatchNames.Any(x => x == SubTerm.ToLower()) != inverse; }
                return MatchNames.Any(x => x.Contains(SubTerm.ToLower())) != inverse;
            }
        }

    }
}
