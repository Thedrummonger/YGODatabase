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

            if (CurrentCollection > 0) //Is a deck and not inventory
            {
                foreach(var i in ValidPrintings)
                {
                    int AmountAvailable = GetAmountOfCardAvailable(Card, Collections, Array.Empty<int>(), i.set_code, i.set_rarity, true);
                    if (AmountAvailable > 0) { AvailableValidPrintings.Add(i); }
                }
            }
            if (AvailableValidPrintings.Any()) { ValidPrintings = AvailableValidPrintings; }

            ValidPrintings = ValidPrintings.OrderBy(x => x.GetRarityIndex());

            return ValidPrintings.FirstOrDefault();
        }

        public static IEnumerable<Guid> GetCardsFromInventory(YGOCardOBJ Card, CardCollection Inventory, string? FilerSet = null, string? FilerRarity = null)
        {
            return Inventory.data.Where(x => 
                x.Value.cardID == Card.id && 
                (FilerSet == null || x.Value.set_code == FilerSet) && 
                (FilerRarity == null || x.Value.set_rarity == FilerRarity)
            ).Select(x => x.Key);
        }

        public static int GetAmountOfCardInOtherDecks(YGOCardOBJ Card, List<CardCollection> Collections, int IgnoreCollection, string? FilerSet = null, string? FilerRarity = null, bool PaperOnly = false)
        {
            return GetAmountOfCardInOtherDecks(Card, Collections, new int[] { IgnoreCollection }, FilerSet, FilerRarity, PaperOnly);
        }
        public static int GetAmountOfCardInOtherDecks(YGOCardOBJ Card, List<CardCollection> Collections, int[] IgnoreCollection, string? FilerSet = null, string? FilerRarity = null, bool PaperOnly = false)
        {
            int CardsInOtherDecks = 0;
            int CollectionIndex = -1;
            foreach (var collection in Collections.Where(x => x.PaperCollection || !PaperOnly))
            {
                CollectionIndex++;
                if (CollectionIndex == 0 ||  IgnoreCollection.Contains(CollectionIndex)) { continue; }
                var OtherDeckCount = GetCardsFromInventory(Card, collection, FilerSet, FilerRarity);
                CardsInOtherDecks += OtherDeckCount.Count();
            }
            return CardsInOtherDecks;
        }

        public static int GetAmountOfCardAvailable(YGOCardOBJ Card, List<CardCollection> Collections, int IgnoreCollection, string? FilerSet = null, string? FilerRarity = null, bool IgnoreNonPape = true)
        {
            return GetAmountOfCardAvailable(Card, Collections, new int[] { IgnoreCollection }, FilerSet, FilerRarity, IgnoreNonPape);
        }
        public static int GetAmountOfCardAvailable(YGOCardOBJ Card, List<CardCollection> Collections, int[] IgnoreCollection, string? FilerSet = null, string? FilerRarity = null, bool IgnoreNonPape = true)
        {
            int InInventory = GetCardsFromInventory(Card, Collections[0], FilerSet, FilerRarity).Count();
            int InOtherDecks = GetAmountOfCardInOtherDecks(Card, Collections, IgnoreCollection, FilerSet, FilerRarity, IgnoreNonPape);
            int AmountAvailable = InInventory - InOtherDecks;
            return AmountAvailable;
        }

    }
}
