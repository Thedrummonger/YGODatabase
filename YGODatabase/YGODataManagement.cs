using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public class YGODataManagement
    {
        public static YGOData MasterDataBase = null;
        public static Dictionary<string, int> SetCodeDict = new Dictionary<string, int>();
        public static Dictionary<int, int> IDLookup = new Dictionary<int, int>();

        public static Dictionary<string, InventoryDatabaseEntry> Inventory = new Dictionary<string, InventoryDatabaseEntry>();

        public int GlobalCardWidth = 421;
        public int GlobalCardHeight = 614;

        public static string GetDatabaseFilePath()
        {
            return Path.Combine(GetAppDataPath(), "Database.json");
        }
        public static string GetImageDirectoryPath()
        {
            return Path.Combine(GetAppDataPath(), "Images");
        }
        public static DataModel.YGOData DownloadData()
        {
            System.Diagnostics.Debug.WriteLine("Downloading YGOPro Data....");
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = wc.DownloadString("https://db.ygoprodeck.com/api/v7/cardinfo.php");
            try
            {
                DataModel.YGOData DataObject = JsonConvert.DeserializeObject<DataModel.YGOData>(webData);
                return DataObject;
            }
            catch
            {
                throw new Exception("Could not download or parse Data from YGOPro");
            }
        }

        public static string GetAppDataPath()
        {
            string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string YGOData = Path.Combine(AppDataPath, "YGODataBase");
            if (!Directory.Exists(YGOData)) { System.Diagnostics.Debug.WriteLine("Creating appdata Directory"); Directory.CreateDirectory(YGOData); }
            return YGOData;
        }

        public static bool CheckForLocalData()
        {
            if (!File.Exists(GetDatabaseFilePath())) { System.Diagnostics.Debug.WriteLine("Local DataBase File not found"); return false; }
            try { _ = JsonConvert.DeserializeObject<DataModel.YGOData>(File.ReadAllText(GetDatabaseFilePath())); }
            catch { System.Diagnostics.Debug.WriteLine("Local DataBase File corrupted"); return false; }
            System.Diagnostics.Debug.WriteLine("Local DataBase File valid");
            return true;
        }

        public static bool UpdateLocalData()
        {
            DataModel.YGOData DataObject = DownloadData();
            DataObject.LastDownload = DateTime.Now;
            string DatabasePath = GetDatabaseFilePath();
            File.WriteAllText(DatabasePath, JsonConvert.SerializeObject(DataObject, Formatting.Indented));
            return CheckForLocalData();
        }

        public static DataModel.YGOData GetDataBase()
        {
            Debug.WriteLine("Loading Database");
            if (!CheckForLocalData())
            {
                Debug.WriteLine("Local Database missing or corrupted, downloading from YGOPro");
                var DownloadSuccess = UpdateLocalData();
                if (!DownloadSuccess) { throw new Exception("Could not get data from YGOPro"); }
            }
            else
            {
                Debug.WriteLine("Local Database Loaded");
            }

            return JsonConvert.DeserializeObject<DataModel.YGOData>(File.ReadAllText(GetDatabaseFilePath()));

        }

        public static Bitmap GetImage(YGOCardOBJ card, int ImageInd)
        {
            if (ImageInd >= card.card_images.Length) 
            {
                var ExctCard = card.card_sets[ImageInd];
                ImageInd = 0; 
                Debug.WriteLine($"{card.name} had no image for {ExctCard.set_name} {ExctCard.set_rarity}");
            }
            string ImageName = Path.GetFileName(card.card_images[ImageInd].image_url);

            if (!Directory.Exists(GetImageDirectoryPath())) { Directory.CreateDirectory(GetImageDirectoryPath()); }

            var LocalImages = Directory.GetFiles(GetImageDirectoryPath()).Select(x => Path.GetFileName(x));
            if (LocalImages.Contains(ImageName))
            {
                return new Bitmap(Path.Combine(GetImageDirectoryPath(), ImageName));
            }
            Debug.WriteLine($"Local Image {ImageName} not found, Downloading...");
            using WebClient wc = new WebClient();
            using Stream s = wc.OpenRead(card.card_images[ImageInd].image_url);
            var newImage = new Bitmap(s);
            try { newImage.Save(Path.Combine(GetImageDirectoryPath(), ImageName)); }
            catch (Exception e) { Debug.WriteLine("Could not save image:\n"+e); }
            return newImage;
        }

        public static void ApplyDataBase(YGOData data)
        {
            MasterDataBase = data;
            int ind = 0;
            foreach(var item in MasterDataBase.data)
            {
                IDLookup[item.id] = ind;
                ind++;
                if (item.card_sets == null || !item.card_sets.Any()) { continue; }
                foreach(var set in item.card_sets)
                {
                    SetCodeDict[set.set_code] = item.id;
                }
            }
        }
    }
}
