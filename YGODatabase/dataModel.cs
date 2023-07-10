using Newtonsoft.Json;

namespace YGODatabase
{
    public class DataModel
    {
        public enum Categories
        {
            MainDeck,
            ExtraDeck,
            SideDeck,
            MaybeDeck,
            None
        }
        public static Dictionary<Categories, string> CategoryNames = new Dictionary<Categories, string>()
        {
            { Categories.MainDeck, "Main" },
            { Categories.ExtraDeck, "Extra" },
            { Categories.SideDeck, "Side" },
            { Categories.MaybeDeck, "Maybe" },
        };
        public class YGOData
        {
            public YGOCardOBJ[] data { get; set; }
            public DateTime LastDownload { get; set; }
        }
        public class CardCollection
        {
            public string Name { get; set; }
            public bool PaperCollection { get; set; } = true;
            public Guid UUID { get; set; }
            public Dictionary<Guid, InventoryDatabaseEntry> data { get; set; }
            public DateTime LastEdited { get; set; }
            public bool IsInventory()
            {
                return UUID == Guid.Empty;
            }
            public InventoryDatabaseEntry Clone()
            {
                var serialized = fastJSON.JSON.ToJSON(this);
                return fastJSON.JSON.ToObject<InventoryDatabaseEntry>(serialized);
            }
        }
        public class YGOCardOBJ
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string frameType { get; set; }
            public string desc { get; set; }
            public int atk { get; set; }
            public int def { get; set; }
            public int level { get; set; }
            public string race { get; set; }
            public string attribute { get; set; }
            public YGOSetData[] card_sets { get; set; }
            public YGOCardImages[] card_images { get; set; }
            public YGOCardPrices[] card_prices { get; set; }
            public List<string> SearchTags { get; set; } = new List<string>();

            public override string ToString()
            {
                return name;
            }
        }

        public class YGOSetData
        {
            public string set_name { get; set; }
            public string set_code { get; set; }
            public string set_rarity { get; set; }
            public string set_rarity_code { get; set; }
            public string set_price { get; set; }
            public List<string> SearchTags { get; set; } = new List<string>();
        }
        public class YGOCardImages
        {
            public int id { get; set; }
            public string image_url { get; set; }
            public string image_url_small { get; set; }
            public string image_url_cropped { get; set; }
        }
        public class YGOCardPrices
        {
            public string cardmarket_price { get; set; }
            public string tcgplayer_price { get; set; }
            public string ebay_price { get; set; }
            public string amazon_price { get; set; }
            public string coolstuffinc_price { get; set; }
        }

        public class CardMatchFilters
        {
            public bool FilterSet { get; set; } = true;
            public bool FilterRarity { get; set; } = true;
            public bool FilterCondition { get; set; } = true;
            public bool FilterCategory { get; set; } = true;
            public bool FilterArt { get; set; } = true;
            public CardMatchFilters SetAll(bool val)
            {
                FilterSet = val;
                FilterRarity = val;
                FilterCondition = val;
                FilterCategory = val;
                FilterArt = val;
                return this;
            }
            public CardMatchFilters Set(bool? _FilterSet = null, bool? _FilterRarity = null, bool? _FilterCondition = null, bool? _FilterCategory = null, bool? _FilterArt = null)
            {
                FilterSet = _FilterSet??FilterSet;
                FilterRarity = _FilterRarity??FilterRarity;
                FilterCondition = _FilterCondition??FilterCondition;
                FilterCategory = _FilterCategory??FilterCategory;
                FilterArt = _FilterArt??FilterArt;
                return this;
            }
            public CardMatchFilters CreateIdenticalCardFindFilter()
            {
                return SetAll(true).Set(_FilterCategory: false);
            }

        }
        public class InventoryDatabaseEntry
        {
            public int cardID { get; set; }
            public string set_code { get; set; }
            public string set_rarity { get; set; }
            public string Condition { get; set; } = "Moderately Played";
            public int ImageIndex { get; set; } = 0;
            public Categories Category { get; set; }
            public DateTime DateAdded { get; set; }
            public DateTime LastUpdated { get; set; }
            public YGOCardOBJ CardData() { return Utility.GetCardByID(cardID); }
            public YGOSetData SetData() { return Utility.GetExactCard(cardID, set_code, set_rarity); }
            public InventoryDatabaseEntry Clone()
            {
                var serialized = fastJSON.JSON.ToJSON(this);
                return fastJSON.JSON.ToObject<InventoryDatabaseEntry>(serialized);
            }
        };
        public class DuplicateCardContainer //Contains all exact duplicates of a single card in a given collection
        {
            public List<Guid> Entries { get; set; }
            public InventoryDatabaseEntry InvData { get; set; }

            public int CardCount() { return Entries.Count; }
            public YGOCardOBJ CardData() { return InvData.CardData(); }
            public YGOSetData SetData() { return InvData.SetData(); }
        }
        public class CardSearchResult
        {
            public string DisplayName { get; set; }
            public YGOCardOBJ Card { get; set; }
            public YGOSetData Set { get; set; }
            public bool FilteringSet { get; set; }
            public bool FilteringRarity { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }
        public class Divider
        {
            public string Display { get; set; }
            public override string ToString()
            {
                return Display;
            }
        }

        public class ComboBoxItem
        {
            public string DisplayName { get; set; }
            public object tag { get; set; }
            public override string ToString()
            {
                return DisplayName;
            }
        }

        public class AppSettingsSettings
        {
            public int InventorySearchBy = 0;
            public bool InventorySearchShowSet = true;
            public bool InventorySearchShowRarity = true;
            public int InventoryShowCollectionOrderBy = 0;
            public bool InventoryShowCollectionDescending = false;
            public string DefaultCondition = "Moderately Played";
            public Dictionary<string, string> LGSSearchURLS = null;
        }

        public class DomainData
        {
            public List<string> NameDomains { get; set; } = new List<string>();
            public List<string> AdditionalNameDomains { get; set; } = new List<string>();
            public string AttributeDomain { get; set; } = string.Empty;
            public List<string> AdditionalAttributeDomains { get; set; } = new List<string>();
            public List<string> MonsterType { get; set; } = new List<string>();
        }
    }
}
