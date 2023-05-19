namespace YGODatabase
{
    partial class InventoryManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lbSearchResults = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFilterBy = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbSelectedCard = new System.Windows.Forms.GroupBox();
            this.BtnAddOneSelected = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.cmbSelectedCardCondition = new System.Windows.Forms.ComboBox();
            this.lblSelectedCard = new System.Windows.Forms.Label();
            this.cmbSelctedCardRarity = new System.Windows.Forms.ComboBox();
            this.cmbSelectedCardSet = new System.Windows.Forms.ComboBox();
            this.chkShowSet = new System.Windows.Forms.CheckBox();
            this.chkShowRarity = new System.Windows.Forms.CheckBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Count = new System.Windows.Forms.ColumnHeader();
            this.CardName = new System.Windows.Forms.ColumnHeader();
            this.Set = new System.Windows.Forms.ColumnHeader();
            this.Rarity = new System.Windows.Forms.ColumnHeader();
            this.Condition = new System.Windows.Forms.ColumnHeader();
            this.cmbOrderBy = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbSelectedCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 78);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(162, 23);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.UpdateSearchResults);
            this.txtSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CaptureTabPress);
            // 
            // lbSearchResults
            // 
            this.lbSearchResults.FormattingEnabled = true;
            this.lbSearchResults.ItemHeight = 15;
            this.lbSearchResults.Location = new System.Drawing.Point(12, 114);
            this.lbSearchResults.Name = "lbSearchResults";
            this.lbSearchResults.Size = new System.Drawing.Size(384, 124);
            this.lbSearchResults.TabIndex = 1;
            this.lbSearchResults.SelectedIndexChanged += new System.EventHandler(this.HighlightCard);
            this.lbSearchResults.DoubleClick += new System.EventHandler(this.lbSearchResults_DoubleClick);
            this.lbSearchResults.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CaptureTabPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Search By";
            // 
            // cmbFilterBy
            // 
            this.cmbFilterBy.FormattingEnabled = true;
            this.cmbFilterBy.Items.AddRange(new object[] {
            "Card Name",
            "Set Code",
            "Both"});
            this.cmbFilterBy.Location = new System.Drawing.Point(76, 12);
            this.cmbFilterBy.Name = "cmbFilterBy";
            this.cmbFilterBy.Size = new System.Drawing.Size(98, 23);
            this.cmbFilterBy.TabIndex = 3;
            this.cmbFilterBy.SelectedIndexChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 251);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Inventory";
            // 
            // gbSelectedCard
            // 
            this.gbSelectedCard.Controls.Add(this.BtnAddOneSelected);
            this.gbSelectedCard.Controls.Add(this.btnRemoveSelected);
            this.gbSelectedCard.Controls.Add(this.numericUpDown1);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardCondition);
            this.gbSelectedCard.Controls.Add(this.lblSelectedCard);
            this.gbSelectedCard.Controls.Add(this.cmbSelctedCardRarity);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardSet);
            this.gbSelectedCard.Location = new System.Drawing.Point(180, 6);
            this.gbSelectedCard.Name = "gbSelectedCard";
            this.gbSelectedCard.Size = new System.Drawing.Size(216, 102);
            this.gbSelectedCard.TabIndex = 6;
            this.gbSelectedCard.TabStop = false;
            this.gbSelectedCard.Text = "Last Card added";
            // 
            // BtnAddOneSelected
            // 
            this.BtnAddOneSelected.Location = new System.Drawing.Point(6, 71);
            this.BtnAddOneSelected.Name = "BtnAddOneSelected";
            this.BtnAddOneSelected.Size = new System.Drawing.Size(64, 23);
            this.BtnAddOneSelected.TabIndex = 11;
            this.BtnAddOneSelected.Text = "Add One";
            this.BtnAddOneSelected.UseVisualStyleBackColor = true;
            this.BtnAddOneSelected.Click += new System.EventHandler(this.btnAddOneSelected_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(6, 44);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(64, 23);
            this.btnRemoveSelected.TabIndex = 10;
            this.btnRemoveSelected.Text = "Remove";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(6, 15);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(37, 23);
            this.numericUpDown1.TabIndex = 9;
            // 
            // cmbSelectedCardCondition
            // 
            this.cmbSelectedCardCondition.FormattingEnabled = true;
            this.cmbSelectedCardCondition.Location = new System.Drawing.Point(76, 41);
            this.cmbSelectedCardCondition.Name = "cmbSelectedCardCondition";
            this.cmbSelectedCardCondition.Size = new System.Drawing.Size(58, 23);
            this.cmbSelectedCardCondition.TabIndex = 3;
            this.cmbSelectedCardCondition.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelectedCardCondition.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // lblSelectedCard
            // 
            this.lblSelectedCard.AutoSize = true;
            this.lblSelectedCard.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.lblSelectedCard.Location = new System.Drawing.Point(49, 19);
            this.lblSelectedCard.Name = "lblSelectedCard";
            this.lblSelectedCard.Size = new System.Drawing.Size(40, 15);
            this.lblSelectedCard.TabIndex = 0;
            this.lblSelectedCard.Text = "Name";
            // 
            // cmbSelctedCardRarity
            // 
            this.cmbSelctedCardRarity.FormattingEnabled = true;
            this.cmbSelctedCardRarity.Location = new System.Drawing.Point(143, 41);
            this.cmbSelctedCardRarity.Name = "cmbSelctedCardRarity";
            this.cmbSelctedCardRarity.Size = new System.Drawing.Size(58, 23);
            this.cmbSelctedCardRarity.TabIndex = 1;
            this.cmbSelctedCardRarity.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelctedCardRarity.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // cmbSelectedCardSet
            // 
            this.cmbSelectedCardSet.FormattingEnabled = true;
            this.cmbSelectedCardSet.Location = new System.Drawing.Point(76, 70);
            this.cmbSelectedCardSet.Name = "cmbSelectedCardSet";
            this.cmbSelectedCardSet.Size = new System.Drawing.Size(125, 23);
            this.cmbSelectedCardSet.TabIndex = 2;
            this.cmbSelectedCardSet.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelectedCardSet.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // chkShowSet
            // 
            this.chkShowSet.AutoSize = true;
            this.chkShowSet.Location = new System.Drawing.Point(12, 49);
            this.chkShowSet.Name = "chkShowSet";
            this.chkShowSet.Size = new System.Drawing.Size(74, 19);
            this.chkShowSet.TabIndex = 7;
            this.chkShowSet.Text = "Show Set";
            this.chkShowSet.UseVisualStyleBackColor = true;
            this.chkShowSet.CheckedChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // chkShowRarity
            // 
            this.chkShowRarity.AutoSize = true;
            this.chkShowRarity.Location = new System.Drawing.Point(86, 49);
            this.chkShowRarity.Name = "chkShowRarity";
            this.chkShowRarity.Size = new System.Drawing.Size(88, 19);
            this.chkShowRarity.TabIndex = 8;
            this.chkShowRarity.Text = "Show Rarity";
            this.chkShowRarity.UseVisualStyleBackColor = true;
            this.chkShowRarity.CheckedChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Count,
            this.CardName,
            this.Set,
            this.Rarity,
            this.Condition});
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(12, 269);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(574, 273);
            this.listView1.TabIndex = 9;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.HighlightCard);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // Count
            // 
            this.Count.Text = "#";
            this.Count.Width = 23;
            // 
            // CardName
            // 
            this.CardName.Text = "Card";
            this.CardName.Width = 300;
            // 
            // Set
            // 
            this.Set.Text = "Set";
            this.Set.Width = 150;
            // 
            // Rarity
            // 
            this.Rarity.Text = "Rarity";
            this.Rarity.Width = 48;
            // 
            // Condition
            // 
            this.Condition.Text = "Cond";
            this.Condition.Width = 45;
            // 
            // cmbOrderBy
            // 
            this.cmbOrderBy.FormattingEnabled = true;
            this.cmbOrderBy.Items.AddRange(new object[] {
            "Card Name",
            "Set Name",
            "Rarity",
            "Condition",
            "Date Added",
            "Date Modified"});
            this.cmbOrderBy.Location = new System.Drawing.Point(294, 244);
            this.cmbOrderBy.Name = "cmbOrderBy";
            this.cmbOrderBy.Size = new System.Drawing.Size(102, 23);
            this.cmbOrderBy.TabIndex = 10;
            this.cmbOrderBy.SelectedIndexChanged += new System.EventHandler(this.cmbOrderBy_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Order By";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(402, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(184, 251);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // InventoryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 554);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbOrderBy);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.chkShowRarity);
            this.Controls.Add(this.chkShowSet);
            this.Controls.Add(this.gbSelectedCard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbFilterBy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbSearchResults);
            this.Controls.Add(this.txtSearch);
            this.KeyPreview = true;
            this.Name = "InventoryManager";
            this.Text = "InventoryManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InventoryManager_FormClosing);
            this.Load += new System.EventHandler(this.InventoryManager_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CaptureKeyPress);
            this.gbSelectedCard.ResumeLayout(false);
            this.gbSelectedCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtSearch;
        private ListBox lbSearchResults;
        private Label label1;
        private ComboBox cmbFilterBy;
        private Label label2;
        private GroupBox gbSelectedCard;
        private CheckBox chkShowSet;
        private CheckBox chkShowRarity;
        private ComboBox cmbSelectedCardCondition;
        private ComboBox cmbSelectedCardSet;
        private ComboBox cmbSelctedCardRarity;
        private Label lblSelectedCard;
        private ListView listView1;
        private ColumnHeader Count;
        private ColumnHeader CardName;
        private ColumnHeader Set;
        private ColumnHeader Rarity;
        private NumericUpDown numericUpDown1;
        private ColumnHeader Condition;
        private Button BtnAddOneSelected;
        private Button btnRemoveSelected;
        private ComboBox cmbOrderBy;
        private Label label3;
        private PictureBox pictureBox1;
    }
}