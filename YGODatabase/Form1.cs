using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Policy;
using System.Windows.Forms;
using static YGODatabase.dataModel;

namespace YGODatabase
{
    public partial class MainInterface : Form
    {
        public YGOCardOBJ CurrentCard = null;
        public MainInterface()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            YGODataManagement.MasterDataBase = YGODataManagement.GetDataBase();

            System.Diagnostics.Debug.WriteLine($"DataBase Loaded\n{YGODataManagement.MasterDataBase.data.Length} Cards Found\nLast Updated {YGODataManagement.MasterDataBase.LastDownload}");

            UpdateListBox();
        }

        private void updateYGODatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Result = MessageBox.Show($"Are you sure you want to update your card database?\n\n" +
                $"Repeated updates in quick succession may result in either your IP address being blacklisted or the API being rolled back." +
                $"\n\nYour Database was last updated {YGODataManagement.MasterDataBase.LastDownload}", 
                "Confirm Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (Result == DialogResult.OK)
            {
                YGODataManagement.UpdateLocalData();
                YGODataManagement.MasterDataBase = YGODataManagement.GetDataBase();
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
            DisplayData = DisplayData.Where(x => x.name.ToLower().Contains(textBox1.Text.ToLower())).ToArray();
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

            pictureBox1.Load(CurrentCard.card_images.First().image_url);

            rtxtCardtext.Text = CurrentCard.desc;

            lbCardData.Items.Clear();

            lbCardData.Items.Add("Type: " + CurrentCard.type);
            if (CurrentCard.HasAttack()) { lbCardData.Items.Add("Attack: " + CurrentCard.atk); }
            if (CurrentCard.HasDefence()) { lbCardData.Items.Add("Defense: " + CurrentCard.def); }
            if (CurrentCard.HasLevel()) { lbCardData.Items.Add("Level: " + CurrentCard.level); }
            lbCardData.Items.Add("Average Price: " + CurrentCard.GetLowestAveragePrice());

        }
    }
}