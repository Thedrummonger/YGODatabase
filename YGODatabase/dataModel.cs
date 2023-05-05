using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGODatabase
{
    public class dataModel
    {
        public class YGOData
        {
            public YGOCardOBJ[] data { get; set; }
            public DateTime LastDownload { get; set; }
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
    }
}
