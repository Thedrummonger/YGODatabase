using Microsoft.VisualBasic;
using Newtonsoft.Json;
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
        public static YGOSetData GetBestSetPrinting(InventoryDatabaseEntry template, List<CardCollection> Collections, CardCollection CurrentCollection, out int ImageIndex)
        {
            ImageIndex = -1;
            CardMatchFilters filters = new();
            filters.SetAll(false);
            filters.Set(_FilterSet: template.set_code is not null, _FilterRarity: template.set_rarity is not null);
            var AllSets = template.CardData().card_sets.OrderBy(x => x.GetRarityIndex());
            template.set_rarity ??= AllSets.First().set_rarity;
            template.set_code ??= AllSets.Where(x => x.set_rarity == template.set_rarity).First().set_code;
            var CardsInInventory = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], template, filters);
            foreach(var i in CardsInInventory)
            {
                var InventoryEntry = Collections[0].data[i];
                var SpecificCardInInevntory = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], InventoryEntry, new CardMatchFilters().SetAll(true));
                var AmountInDecks = CollectionSearchUtils.GetAmountOfCardInNonInventoryCollections(Collections, InventoryEntry, new HashSet<int>(), new CardMatchFilters().SetAll(true), true);
                if (SpecificCardInInevntory.Count() > AmountInDecks)
                {
                    ImageIndex = InventoryEntry.ImageIndex;
                    return InventoryEntry.SetData();
                }
            }
            foreach (var i in CardsInInventory)
            {
                var InventoryEntry = Collections[0].data[i];
                var SpecificCardInInevntory = CollectionSearchUtils.GetIdenticalCardsFromCollection(Collections[0], InventoryEntry, new CardMatchFilters().SetAll(true));
                var AmountInThisDecks = CollectionSearchUtils.GetIdenticalCardsFromCollection(CurrentCollection, InventoryEntry, new CardMatchFilters().SetAll(true)).Count();
                if (SpecificCardInInevntory.Count() > AmountInThisDecks)
                {
                    ImageIndex = InventoryEntry.ImageIndex;
                    return InventoryEntry.SetData();
                }
            }
            return Utility.GetExactCard(template.cardID, template.set_code, template.set_rarity);
        }
    }
}
