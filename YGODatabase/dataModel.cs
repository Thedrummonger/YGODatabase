using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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


        public class InventoryDatabaseEntry
        {
            public int cardID { get; set; }
            public string set_code { get; set; }
            public string set_rarity { get; set; }
            public string Condition { get; set; } = "Moderately Played";
            public string Language { get; set; } = "En";
            public int ImageIndex { get; set; } = 0;
            public Categories Category { get; set; }
            public DateTime DateAdded { get; set; }
            public DateTime LastUpdated { get; set; }
        };
        public class InventoryObject
        {
            public int Amount { get; set; }
            public YGOCardOBJ Card { get; set; }
            public YGOSetData Set { get; set; }
            public Guid InventoryID { get; set; }
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
        }
    }
}
