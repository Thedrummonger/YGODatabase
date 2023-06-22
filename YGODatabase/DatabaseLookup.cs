using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Policy;
using System.Windows.Forms;
using static YGODatabase.DataModel;

namespace YGODatabase
{
    public partial class MainInterface : Form
    {
        public YGOCardOBJ CurrentCard = null;
        public InventoryManager inventoryManager;
        public AppSettingsSettings Settings;
        public MainInterface()
        {
            InitializeComponent();
            Settings = new AppSettingsSettings();
            if (File.Exists(YGODataManagement.GetSettingPath()))
            {
                try { Settings = JsonConvert.DeserializeObject<AppSettingsSettings>(File.ReadAllText(YGODataManagement.GetSettingPath())); }
                catch { Settings = new AppSettingsSettings(); }
            }
            inventoryManager = new InventoryManager(this);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = 0;
            numericUpDown1.Minimum = 0;
            numericUpDown1.Value = 0;
            YGODataManagement.ApplyDataBase(YGODataManagement.GetDataBase());

            Debug.WriteLine($"DataBase Loaded\n{YGODataManagement.MasterDataBase.data.Length} Cards Found\nLast Updated {YGODataManagement.MasterDataBase.LastDownload}");

            UpdateListBox();
        }

        private void updateDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Result = MessageBox.Show($"Are you sure you want to update your card database?\n\n" +
                $"Repeated updates in quick succession may result in either your IP address being blacklisted or the API being rolled back." +
                $"\n\nYour Database was last updated {YGODataManagement.MasterDataBase.LastDownload}",
                "Confirm Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (Result == DialogResult.OK)
            {
                YGODataManagement.UpdateLocalData();
                YGODataManagement.ApplyDataBase(YGODataManagement.GetDataBase());
                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            YGOCardOBJ[] DisplayData = YGODataManagement.MasterDataBase.data;
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                lbCardList.DataSource = DisplayData;
                return;
            }
            DisplayData = DisplayData.Where(x => SearchParser.CardMatchesFilter(x.ToString(), x, textBox1.Text.ToLower(), true, true)).ToArray();
            lbCardList.DataSource = DisplayData;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbCardList.SelectedIndex < 0 || lbCardList.SelectedItem is not YGOCardOBJ) { return; }
            CurrentCard = lbCardList.SelectedItem as YGOCardOBJ;

            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = CurrentCard.card_images.Length;
            numericUpDown1.Value = 1;

            pictureBox1.Image = YGODataManagement.GetImage(CurrentCard, 0);

            rtxtCardtext.Text = CurrentCard.desc;

            lbCardData.Items.Clear();

            lbCardData.Items.Add("Type: " + CurrentCard.type);
            if (CurrentCard.HasAttack()) { lbCardData.Items.Add("Attack: " + CurrentCard.atk); }
            if (CurrentCard.HasDefence()) { lbCardData.Items.Add("Defense: " + CurrentCard.def); }
            if (CurrentCard.HasLevel()) { lbCardData.Items.Add("Level: " + CurrentCard.level); }
            if (CurrentCard.race is not null) { lbCardData.Items.Add("Race: " + CurrentCard.race); }
            if (CurrentCard.attribute is not null) { lbCardData.Items.Add("Attribute: " + CurrentCard.attribute); }
            lbCardData.Items.Add(Utility.CreateDivider(lbCardData, "Price"));
            lbCardData.Items.Add("Average: " + CurrentCard.GetLowestAveragePrice());
            lbCardData.Items.Add("TCGPlayer: " + CurrentCard.card_prices.First().tcgplayer_price);
            lbCardData.Items.Add("Card Market: " + CurrentCard.card_prices.First().cardmarket_price);
            lbCardData.Items.Add("Cool Stuff Inc: " + CurrentCard.card_prices.First().coolstuffinc_price);

        }

        private void inventoryManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                inventoryManager.Show();
                inventoryManager.Focus();
            }
            catch { }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = YGODataManagement.GetImage(CurrentCard, (int)(numericUpDown1.Value - 1));
        }

        private void MainInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(YGODataManagement.GetSettingPath(), JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }
    }
}