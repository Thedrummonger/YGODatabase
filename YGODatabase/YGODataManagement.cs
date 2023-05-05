using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static YGODatabase.dataModel;

namespace YGODatabase
{
    public class YGODataManagement
    {
        public static dataModel.YGOData MasterDataBase = null;

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
        public static dataModel.YGOData DownloadData()
        {
            System.Diagnostics.Debug.WriteLine("Downloading YGOPro Data....");
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = wc.DownloadString("https://db.ygoprodeck.com/api/v7/cardinfo.php");
            try
            {
                dataModel.YGOData DataObject = JsonConvert.DeserializeObject<dataModel.YGOData>(webData);
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
            try { _ = JsonConvert.DeserializeObject<dataModel.YGOData>(File.ReadAllText(GetDatabaseFilePath())); }
            catch { System.Diagnostics.Debug.WriteLine("Local DataBase File corrupted"); return false; }
            System.Diagnostics.Debug.WriteLine("Local DataBase File valid");
            return true;
        }

        public static bool UpdateLocalData()
        {
            dataModel.YGOData DataObject = DownloadData();
            DataObject.LastDownload = DateTime.Now;
            string DatabasePath = GetDatabaseFilePath();
            File.WriteAllText(DatabasePath, JsonConvert.SerializeObject(DataObject, Formatting.Indented));
            return CheckForLocalData();
        }

        public static dataModel.YGOData GetDataBase()
        {
            System.Diagnostics.Debug.WriteLine("Loading Database");
            if (!CheckForLocalData())
            {
                System.Diagnostics.Debug.WriteLine("Local Database missing or corrupted, downloading from YGOPro");
                var DownloadSuccess = UpdateLocalData();
                if (!DownloadSuccess) { throw new Exception("Could not get data from YGOPro"); }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Local Database Loaded");
            }

            return JsonConvert.DeserializeObject<dataModel.YGOData>(File.ReadAllText(GetDatabaseFilePath()));

        }

        public static Bitmap GetImage(YGOCardOBJ card, int ImageInd)
        {
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
            newImage.Save(Path.Combine(GetImageDirectoryPath(), ImageName));
            return newImage;
        }
    }
}
