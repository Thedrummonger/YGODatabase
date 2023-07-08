using Newtonsoft.Json;
using System.Diagnostics;
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

        private void MainInterface_Load(object sender, EventArgs e)
        {
            ResizeHeight();
            ResizeWidth();
            UpdateCardData();
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
            lbCardList.BeginUpdate();
            YGOCardOBJ[] DisplayData = YGODataManagement.MasterDataBase.data;
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                lbCardList.DataSource = DisplayData;
                lbCardList.EndUpdate();
                return;
            }
            DisplayData = DisplayData.Where(x => SearchParser.CardMatchesFilter(x.ToString(), x, textBox1.Text.ToLower(), true, true)).ToArray();
            lbCardList.DataSource = DisplayData;
            lbCardList.EndUpdate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateListBox();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCardData();

            if (lbCardList.SelectedIndex < 0 || lbCardList.SelectedItem is not YGOCardOBJ cardOBJ || !cardOBJ.isMonster()) { return; }
            var Domains = Utility.GetDomains(cardOBJ);
            Debug.WriteLine(JsonConvert.SerializeObject(Domains, Formatting.Indented));
        }

        private void UpdateCardData()
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
            if (inventoryManager.Visible) { inventoryManager.SaveFormDataOnClose(); }
            File.WriteAllText(YGODataManagement.GetSettingPath(), JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        private void MainInterface_ResizeEnd(object sender, EventArgs e)
        {
            ResizeHeight();
            ResizeWidth();
            UpdateCardData();
        }

        private void ResizeHeight()
        {
            int CardDataExtraheight = 80;

            //Stretch the Card List to the bottom of the form
            Utility.StretchListBoxHeightToFormBottom(lbCardList, this);

            //Move the Card Text box to the half way point of the CardList LB then move it down by the height of the label
            //and then again by an arbirtrary number so the Card Data LB is big enough to fit most data at it's default height
            int NewCardTextBoxLocation = ((lbCardList.Height - lbCardList.Location.Y)/2) + CardDataExtraheight + lblCardText.Height;
            rtxtCardtext.Location = new Point(rtxtCardtext.Location.X, NewCardTextBoxLocation);

            //The Card Text Text box can now be stretched to the bottom of the form 
            Utility.StretchListBoxHeightToFormBottom(rtxtCardtext, this);

            //Move the CardText Label into the correct position above the TextBox
            lblCardText.Location = new Point(lblCardText.Location.X, rtxtCardtext.Location.Y - lblCardText.Height);
            lbldata.Location = new Point(lbldata.Location.X, lbCardData.Location.Y - lbldata.Height);

            //Stretch card data to the position of the Card text label and then up by a small amount for some padding
            lbCardData.Height = lblCardText.Location.Y - lbCardData.Location.Y - 2;
        }

        private void ResizeWidth()
        {
            int Padding = 8;

            int StaticPBWidth = pictureBox1.Width + Padding;
            int FormWith = this.Width - StaticPBWidth - 30;

            //The CardList should be slightly wider that the card data
            int CardListWidth = (int)(FormWith * .60);
            int CardDataWidth = (int)(FormWith * .40);

            //Set the width of the list boxes
            lbCardList.Width = CardListWidth;
            lbCardData.Width = CardDataWidth;
            rtxtCardtext.Width= CardDataWidth;

            //Set the card data to the right of the card list and add some padding
            lbCardData.Location = new Point(lbCardList.Location.X + lbCardList.Width + Padding, lbCardData.Location.Y);
            //Set the reset of the data objects to the same X point as the the Card Data LB
            rtxtCardtext.Location = new Point(lbCardData.Location.X, rtxtCardtext.Location.Y);
            lbldata.Location = new Point(lbCardData.Location.X, lbldata.Location.Y);
            lblCardText.Location = new Point(lbCardData.Location.X, lblCardText.Location.Y);

            //Make the search box the same size as the Card data list
            textBox1.Width = lbCardList.Width;

            //Set the Picture box objects to the right of the Data objects. The picture box already has whitespace padding on each side
            pictureBox1.Location = new Point(lbCardData.Location.X + lbCardData.Width, pictureBox1.Location.Y);
            numericUpDown1.Location = new Point(pictureBox1.Location.X + pictureBox1.Width - numericUpDown1.Width - Padding, numericUpDown1.Location.Y);
            label1.Location = new Point(numericUpDown1.Location.X - label1.Width - 2, numericUpDown1.Location.Y);
        }

        bool Dragging = false;
        bool LastClickWasItem = false;
        private void lbCardList_MouseDown(object sender, MouseEventArgs e)
        {
            //Ensure the new card is visually updated on mouse down instead of mouse up so it updates before the drag drop freezes the form
            int index = lbCardList.IndexFromPoint(e.Location);
            if (index < 0 || index >= lbCardList.Items.Count) { LastClickWasItem = false; return; }
            LastClickWasItem = true;
            lbCardList.SetSelected(index, true);
        }

        private void lbCardList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !Dragging && LastClickWasItem && lbCardList.SelectedItem is YGOCardOBJ Card)
            {
                Dragging = true;
                Debug.WriteLine($"Dragging {Card.name}");
                inventoryManager.listView1.DoDragDrop(Card, DragDropEffects.Move);
            }
            else
            {
                Dragging = false;
            }
        }
    }
}