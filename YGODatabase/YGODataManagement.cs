using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public static class YGODataManagement
    {
        public static YGOData MasterDataBase = null;
        public static Dictionary<string, int> SetCodeDict = new Dictionary<string, int>();
        public static Dictionary<int, Tuple<int, int>> IDLookup = new Dictionary<int, Tuple<int, int>>();

        private static bool UseTestingPaths = true;

        public static Dictionary<string, InventoryDatabaseEntry> Inventory = new Dictionary<string, InventoryDatabaseEntry>();

        public static int GlobalCardWidth = 421;
        public static int GlobalCardHeight = 614;

        public static string GetDatabaseFilePath()
        {
            return Path.Combine(GetAppDataPath(), "Database.json");
        }
        public static string GetImageDirectoryPath(ImageType imageType)
        {
            return imageType switch
            {
                ImageType.small => Path.Combine(GetAppDataPath(), "Images", "small"),
                ImageType.cropped => Path.Combine(GetAppDataPath(), "Images", "cropped"),
                _ => Path.Combine(GetAppDataPath(), "Images")
            };
        }
        public static string GetInventoryFilePath()
        {
            if (UseTestingPaths) { return Path.Combine(GetTestFolderPath(), "Inventory.json"); }
            return Path.Combine(GetAppDataPath(), "Inventory.json");
        }
        public static string GetDeckDirectoryPath()
        {
            if (UseTestingPaths) { return Path.Combine(GetTestFolderPath(), "Decks"); }
            return Path.Combine(GetAppDataPath(), "Decks");
        }
        public static string GetSettingPath()
        {
            return Path.Combine(GetAppDataPath(), "Settings.json");
        }
        public static DataModel.YGOData DownloadData()
        {
            Debug.WriteLine("Downloading YGOPro Data....");
            WebClient wc = new System.Net.WebClient();
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
        public static string GetTestFolderPath()
        {
            string TestingFolder = "D:\\Testing";
            string YGOData = Path.Combine(TestingFolder, "YGODataBase");
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

        public static List<string> GetAllSets(this YGOData Database)
        {
            List<string> AllSets = new List<string>();
            foreach (var i in Database.data)
            {
                foreach (var j in i.card_sets??Array.Empty<YGOSetData>())
                {
                    if (!AllSets.Contains(j.set_name)) { AllSets.Add(j.set_name); }
                }
            }
            return AllSets;
        }

        public enum ImageType
        {
            standard,
            small,
            cropped
        }

        public static Bitmap GetImage(YGOCardOBJ card, int ImageInd, ImageType ImageType = ImageType.standard)
        {
            if (ImageInd >= card.card_images.Length)
            {
                Debug.WriteLine($"{card.name} had no image at index {ImageInd}");
                ImageInd = 0; 
            }

            string ImagePath = ImageType switch
            {
                ImageType.standard => card.card_images[ImageInd].image_url,
                ImageType.small => card.card_images[ImageInd].image_url_small,
                ImageType.cropped => card.card_images[ImageInd].image_url_cropped,
                _ => card.card_images[ImageInd].image_url,
            };

            string ImageName = Path.GetFileName(ImagePath);

            string ImageDirectory = GetImageDirectoryPath(ImageType);

            if (!Directory.Exists(ImageDirectory)) { Directory.CreateDirectory(ImageDirectory); }

            var LocalImages = Directory.GetFiles(ImageDirectory).Select(x => Path.GetFileName(x));
            if (LocalImages.Contains(ImageName))
            {
                return new Bitmap(Path.Combine(ImageDirectory, ImageName));
            }
            Debug.WriteLine($"Local Image {ImageName} not found, Downloading...");
            using WebClient wc = new WebClient();
            using Stream s = wc.OpenRead(ImagePath);
            var newImage = new Bitmap(s);
            try { newImage.Save(Path.Combine(ImageDirectory, ImageName)); }
            catch (Exception e) { Debug.WriteLine("Could not save image:\n"+e); }
            return newImage;
        }

        public static void ApplyDataBase(YGOData data)
        {
            MasterDataBase = data;
            int DataBaseIndex = 0;
            foreach(var item in MasterDataBase.data)
            {
                int ImageIndex = 0;
                IDLookup[item.id] = new(DataBaseIndex, ImageIndex);
                foreach (var altID in item.card_images) 
                { 
                    if (!IDLookup.ContainsKey(altID.id)) { IDLookup[altID.id] = new(DataBaseIndex, ImageIndex); }
                    ImageIndex++;
                }
                DataBaseIndex++;
                if (item.card_sets == null || !item.card_sets.Any()) { continue; }
                CreateSearchTags(item);
            }
        }

        private static void CreateSearchTags(YGOCardOBJ item)
        {
            foreach (var set in item.card_sets)
            {
                SetCodeDict[set.set_code] = item.id;
                set.SearchTags.Add(set.set_code.ToLower());
                set.SearchTags.Add(set.set_code.CleanCardName());
                var SetcodeData = set.set_code.StringSplit("-");
                if (SetcodeData.Length > 1)
                {
                    set.SearchTags.Add(SetcodeData[0].ToLower() + SetcodeData[1].Replace("EN", "").TrimStart('0'));
                    set.SearchTags.Add(SetcodeData[0].ToLower() + SetcodeData[1].Replace("EN", ""));
                }
            }
            item.SearchTags.Add(item.name.ToLower());
            item.SearchTags.Add(item.name.CleanCardName());
            item.SearchTags.Add(item.name.CleanCardName(" "));
        }
    }
}
