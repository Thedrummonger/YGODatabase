using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    internal static class CollectionSearchUtils
    {
        public static Guid[] GetIdenticalCardsFromCollection(CardCollection TargetCollection, InventoryDatabaseEntry TargetCard, CardMatchFilters? filters = null)
        {
            string TargetID = TargetCard.CreateIDString(filters);
            List<Guid> result = new List<Guid>();
            foreach(var i in TargetCollection.data)
            {
                if (i.Value.CreateIDString(filters) == TargetID) { result.Add(i.Key); }
            }
            return result.ToArray();
        }

        public static int GetAmountOfCardInNonInventoryCollections(List<CardCollection> Collections, InventoryDatabaseEntry Target, HashSet<int> IgnoreCollection, CardMatchFilters? filters = null, bool PaperOnly = false)
        {
            int CardsInOtherDecks = 0;
            int CollectionIndex = -1;
            foreach (var collection in Collections)
            {
                CollectionIndex++;
                if (!collection.PaperCollection && PaperOnly) { continue; }
                if (CollectionIndex == 0 || IgnoreCollection.Contains(CollectionIndex)) { continue; }
                var OtherDeckCount = GetIdenticalCardsFromCollection(collection, Target, filters);
                CardsInOtherDecks += OtherDeckCount.Count();
            }
            return CardsInOtherDecks;
        }
        public static int GetAmountOfCardInNonInventoryCollections(Dictionary<int, Dictionary<string, int>> Cache, List<CardCollection> Collections, InventoryDatabaseEntry Target, HashSet<int> IgnoreCollection, CardMatchFilters? filters = null, bool PaperOnly = false)
        {
            int CardsInOtherDecks = 0;
            int CollectionIndex = -1;
            var TargetID = Target.CreateIDString(filters);
            foreach (var collection in Collections)
            {
                CollectionIndex++;
                if ((!collection.PaperCollection && PaperOnly) || 
                    CollectionIndex == 0 || 
                    IgnoreCollection.Contains(CollectionIndex) || 
                    !Cache.ContainsKey(CollectionIndex) || 
                    !Cache[CollectionIndex].ContainsKey(TargetID)) 
                { continue; }
                CardsInOtherDecks += Cache[CollectionIndex][TargetID];
            }
            return CardsInOtherDecks;
        }

        public static Dictionary<int, Dictionary<string, int>> CacheIdsInCollections(List<CardCollection> Collections, HashSet<int> IgnoreCollection, CardMatchFilters? filters = null)
        {
            Dictionary<int, Dictionary<string, int>> cache = new();
            int CollectionIndex = -1;
            foreach (var collection in Collections)
            {
                CollectionIndex++;
                if (IgnoreCollection.Contains(CollectionIndex)) { continue; }
                if (!cache.ContainsKey(CollectionIndex)) { cache[CollectionIndex] = new Dictionary<string, int>(); }
                foreach(var card in collection.data)
                {
                    string ID = card.Value.CreateIDString(filters);
                    if (!cache[CollectionIndex].ContainsKey(ID)) { cache[CollectionIndex][ID] = 0; }
                    cache[CollectionIndex][ID]++;
                }
            }
            return cache;
        }
    }
}
