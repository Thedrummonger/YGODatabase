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
                char[] Modifiers = new char[] { '!', '=', '#', '%', '@' };
                bool Perfect = false;
                bool inverse = false;

                SearchType searchType = SearchType.Identifier;

                bool HasModifier = Modifiers.Any(x => SubTerm.StartsWith(x));
                while (HasModifier)
                {
                    char Modifier = SubTerm[0];
                    SubTerm = SubTerm[1..];
                    HasModifier = Modifiers.Any(x => SubTerm.StartsWith(x));

                    if (Modifier == '=') { Perfect = true; }
                    else if (Modifier == '!') { inverse = true; }
                    else if (Modifier == '#') { searchType = SearchType.type; }
                    else if (Modifier == '%') { searchType = SearchType.attribute; }
                    else if (Modifier == '@') { searchType = SearchType.description; }
                }

                if (SubTerm.StartsWith("!")) { inverse = true; SubTerm = SubTerm[1..]; }
                List<string> MatchNames = new List<string>();

                switch (searchType)
                {
                    case SearchType.Identifier:
                        {
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
                    case SearchType.type:
                        if (Perfect) { return (Card.type.ToLower() == SubTerm.ToLower()|| Card.frameType.ToLower() == SubTerm.ToLower()) != inverse; }
                        return (Card.type.ToLower().Contains(SubTerm.ToLower()) || Card.frameType.ToLower().Contains(SubTerm.ToLower())) != inverse;
                    case SearchType.attribute:
                        if (string.IsNullOrWhiteSpace(Card.attribute)) { return false; }
                        if (Perfect) { return Card.attribute.ToLower() == SubTerm.ToLower() != inverse; }
                        return Card.attribute.ToLower().Contains(SubTerm.ToLower()) != inverse; ;
                    case SearchType.description:
                        if (Perfect) { return Card.desc.ToLower() == SubTerm.ToLower() != inverse; }
                        return Card.desc.ToLower().Contains(SubTerm.ToLower()) != inverse; ;
                }
                return true;
            }
        }

        enum SearchType
        {
            Identifier,
            type,
            attribute,
            description
        }

    }
}
