using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    internal class SmartCardSetSelector
    {
        public static YGOSetData? GetBestSetPrinting(YGOCardOBJ Card, List<CardCollection> Collections, int CurrentCollection, string? SetCode = null, string? SetRarity = null)
        {
            IEnumerable<YGOSetData> ValidPrintings = Card.card_sets;

            if (SetCode is not null)
            {
                ValidPrintings = ValidPrintings.Where(x => x.set_code == SetCode);
            }
            if (SetRarity is not null)
            {
                ValidPrintings = ValidPrintings.Where(x => x.set_rarity == SetRarity);
            }

            List<YGOSetData> AvailableValidPrintings = new List<YGOSetData>();

            string FileterSetCode = SetCode??Card.card_sets.First().set_code;
            string FileterSetRarity = SetRarity??Card.card_sets.First().set_rarity;

            if (CurrentCollection > 0) //Is a deck and not inventory
            {
                foreach(var i in ValidPrintings)
                {
                    InventoryDatabaseEntry Target = new InventoryDatabaseEntry(Collections[CurrentCollection].UUID) { cardID = Card.id, set_code = FileterSetCode, set_rarity = FileterSetRarity };
                    CardMatchFilters filters = new CardMatchFilters().SetAll(false).Set(_FilterSet: SetCode is not null, _FilterRarity: SetRarity is not null);
                    int AmountAvailable = CollectionSearchUtils.GetAmountOfCardAvailable(Target, Collections, Array.Empty<int>(), filters, true);
                    if (AmountAvailable > 0) { AvailableValidPrintings.Add(i); }
                }
            }
            if (AvailableValidPrintings.Any()) { ValidPrintings = AvailableValidPrintings; }

            ValidPrintings = ValidPrintings.OrderBy(x => x.GetRarityIndex());

            return ValidPrintings.FirstOrDefault();
        }

    }
}
