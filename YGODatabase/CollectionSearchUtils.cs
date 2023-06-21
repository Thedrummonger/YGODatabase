using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    internal static class CollectionSearchUtils
    {
        public static Guid[] GetIdenticalCardsFromCollection(CardCollection TargetCollection, InventoryDatabaseEntry TargetCard, CardMatchFilters? filters = null)
        {
            Guid[] IdenticalEntries = TargetCollection.data.Where(x => x.Value.CreateIDString(filters) == TargetCard.CreateIDString(filters)).Select(x => x.Key).ToArray();
            return IdenticalEntries??Array.Empty<Guid>();
        }

        public static int GetAmountOfCardAvailable(InventoryDatabaseEntry Target, List<CardCollection> Collections, int[] IgnoreCollection, CardMatchFilters? filters = null, bool PaperOnly = false)
        {
            int InInventory = IgnoreCollection.Contains(0) ? 0 : GetIdenticalCardsFromCollection(Collections[0], Target, filters).Length;
            int InOtherDecks = GetAmountOfCardInNonInventoryCollections(Collections, Target, IgnoreCollection, filters, PaperOnly);
            int AmountAvailable = InInventory - InOtherDecks;
            return AmountAvailable;
        }

        public static int GetAmountOfCardInNonInventoryCollections(List<CardCollection> Collections, InventoryDatabaseEntry Target, int[] IgnoreCollection, CardMatchFilters? filters = null, bool PaperOnly = false)
        {
            int CardsInOtherDecks = 0;
            int CollectionIndex = -1;
            foreach (var collection in Collections.Where(x => x.PaperCollection || !PaperOnly))
            {
                CollectionIndex++;
                if (CollectionIndex == 0 || IgnoreCollection.Contains(CollectionIndex)) { continue; }
                var OtherDeckCount = GetIdenticalCardsFromCollection(collection, Target, filters);
                CardsInOtherDecks += OtherDeckCount.Count();
            }
            return CardsInOtherDecks;
        }
    }
}
