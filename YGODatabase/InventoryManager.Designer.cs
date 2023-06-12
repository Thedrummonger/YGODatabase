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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRenameCollection = new System.Windows.Forms.Button();
            this.BTNImportCollection = new System.Windows.Forms.Button();
            this.btnDeleteCollection = new System.Windows.Forms.Button();
            this.btnAddCollection = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnImportCards = new System.Windows.Forms.Button();
            this.BGSearchAdd = new System.Windows.Forms.GroupBox();
            this.chkPaperCollection = new System.Windows.Forms.CheckBox();
            this.txtInventoryFilter = new System.Windows.Forms.TextBox();
            this.gbSelectedCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.BGSearchAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(6, 83);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(152, 23);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.UpdateSearchResults);
            this.txtSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CaptureTabPress);
            // 
            // lbSearchResults
            // 
            this.lbSearchResults.CausesValidation = false;
            this.lbSearchResults.FormattingEnabled = true;
            this.lbSearchResults.ItemHeight = 15;
            this.lbSearchResults.Location = new System.Drawing.Point(12, 176);
            this.lbSearchResults.Name = "lbSearchResults";
            this.lbSearchResults.Size = new System.Drawing.Size(384, 139);
            this.lbSearchResults.TabIndex = 1;
            this.lbSearchResults.SelectedIndexChanged += new System.EventHandler(this.HighlightCard);
            this.lbSearchResults.DoubleClick += new System.EventHandler(this.lbSearchResults_DoubleClick);
            this.lbSearchResults.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CaptureTabPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
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
            this.cmbFilterBy.Location = new System.Drawing.Point(63, 16);
            this.cmbFilterBy.Name = "cmbFilterBy";
            this.cmbFilterBy.Size = new System.Drawing.Size(93, 23);
            this.cmbFilterBy.TabIndex = 3;
            this.cmbFilterBy.SelectedIndexChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 328);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Collection";
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
            this.gbSelectedCard.Location = new System.Drawing.Point(180, 58);
            this.gbSelectedCard.Name = "gbSelectedCard";
            this.gbSelectedCard.Size = new System.Drawing.Size(216, 112);
            this.gbSelectedCard.TabIndex = 6;
            this.gbSelectedCard.TabStop = false;
            this.gbSelectedCard.Text = "Last Card added";
            // 
            // BtnAddOneSelected
            // 
            this.BtnAddOneSelected.Location = new System.Drawing.Point(6, 82);
            this.BtnAddOneSelected.Name = "BtnAddOneSelected";
            this.BtnAddOneSelected.Size = new System.Drawing.Size(64, 23);
            this.BtnAddOneSelected.TabIndex = 11;
            this.BtnAddOneSelected.Text = "Add One";
            this.BtnAddOneSelected.UseVisualStyleBackColor = true;
            this.BtnAddOneSelected.Click += new System.EventHandler(this.btnAddOneSelected_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(6, 48);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(64, 23);
            this.btnRemoveSelected.TabIndex = 10;
            this.btnRemoveSelected.Text = "Remove";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(6, 16);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(37, 23);
            this.numericUpDown1.TabIndex = 9;
            // 
            // cmbSelectedCardCondition
            // 
            this.cmbSelectedCardCondition.FormattingEnabled = true;
            this.cmbSelectedCardCondition.Location = new System.Drawing.Point(76, 48);
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
            this.lblSelectedCard.Location = new System.Drawing.Point(49, 20);
            this.lblSelectedCard.Name = "lblSelectedCard";
            this.lblSelectedCard.Size = new System.Drawing.Size(40, 15);
            this.lblSelectedCard.TabIndex = 0;
            this.lblSelectedCard.Text = "Name";
            // 
            // cmbSelctedCardRarity
            // 
            this.cmbSelctedCardRarity.FormattingEnabled = true;
            this.cmbSelctedCardRarity.Location = new System.Drawing.Point(143, 48);
            this.cmbSelctedCardRarity.Name = "cmbSelctedCardRarity";
            this.cmbSelctedCardRarity.Size = new System.Drawing.Size(58, 23);
            this.cmbSelctedCardRarity.TabIndex = 1;
            this.cmbSelctedCardRarity.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelctedCardRarity.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // cmbSelectedCardSet
            // 
            this.cmbSelectedCardSet.FormattingEnabled = true;
            this.cmbSelectedCardSet.Location = new System.Drawing.Point(76, 82);
            this.cmbSelectedCardSet.Name = "cmbSelectedCardSet";
            this.cmbSelectedCardSet.Size = new System.Drawing.Size(125, 23);
            this.cmbSelectedCardSet.TabIndex = 2;
            this.cmbSelectedCardSet.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelectedCardSet.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // chkShowSet
            // 
            this.chkShowSet.AutoSize = true;
            this.chkShowSet.Checked = true;
            this.chkShowSet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowSet.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkShowSet.Location = new System.Drawing.Point(6, 60);
            this.chkShowSet.Name = "chkShowSet";
            this.chkShowSet.Size = new System.Drawing.Size(88, 17);
            this.chkShowSet.TabIndex = 7;
            this.chkShowSet.Text = "Select Rarity";
            this.chkShowSet.UseVisualStyleBackColor = true;
            this.chkShowSet.CheckedChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // chkShowRarity
            // 
            this.chkShowRarity.AutoSize = true;
            this.chkShowRarity.Checked = true;
            this.chkShowRarity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowRarity.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkShowRarity.Location = new System.Drawing.Point(6, 44);
            this.chkShowRarity.Name = "chkShowRarity";
            this.chkShowRarity.Size = new System.Drawing.Size(75, 17);
            this.chkShowRarity.TabIndex = 8;
            this.chkShowRarity.Text = "Select Set";
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
            this.listView1.Location = new System.Drawing.Point(12, 351);
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
            this.cmbOrderBy.Location = new System.Drawing.Point(279, 324);
            this.cmbOrderBy.Name = "cmbOrderBy";
            this.cmbOrderBy.Size = new System.Drawing.Size(102, 23);
            this.cmbOrderBy.TabIndex = 10;
            this.cmbOrderBy.SelectedIndexChanged += new System.EventHandler(this.cmbOrderBy_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(225, 328);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Order By";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(402, 64);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(184, 251);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRenameCollection);
            this.groupBox1.Controls.Add(this.BTNImportCollection);
            this.groupBox1.Controls.Add(this.btnDeleteCollection);
            this.groupBox1.Controls.Add(this.btnAddCollection);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(574, 46);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Deck:";
            // 
            // btnRenameCollection
            // 
            this.btnRenameCollection.Location = new System.Drawing.Point(353, 16);
            this.btnRenameCollection.Name = "btnRenameCollection";
            this.btnRenameCollection.Size = new System.Drawing.Size(75, 23);
            this.btnRenameCollection.TabIndex = 4;
            this.btnRenameCollection.Text = "Rename";
            this.btnRenameCollection.UseVisualStyleBackColor = true;
            // 
            // BTNImportCollection
            // 
            this.BTNImportCollection.Location = new System.Drawing.Point(433, 16);
            this.BTNImportCollection.Name = "BTNImportCollection";
            this.BTNImportCollection.Size = new System.Drawing.Size(135, 23);
            this.BTNImportCollection.TabIndex = 3;
            this.BTNImportCollection.Text = "Import YDK Collection";
            this.BTNImportCollection.UseVisualStyleBackColor = true;
            this.BTNImportCollection.Click += new System.EventHandler(this.BTNImportCollection_Click);
            // 
            // btnDeleteCollection
            // 
            this.btnDeleteCollection.Location = new System.Drawing.Point(282, 16);
            this.btnDeleteCollection.Name = "btnDeleteCollection";
            this.btnDeleteCollection.Size = new System.Drawing.Size(65, 23);
            this.btnDeleteCollection.TabIndex = 2;
            this.btnDeleteCollection.Text = "Delete";
            this.btnDeleteCollection.UseVisualStyleBackColor = true;
            this.btnDeleteCollection.Click += new System.EventHandler(this.btnDeleteCollection_Click);
            // 
            // btnAddCollection
            // 
            this.btnAddCollection.Location = new System.Drawing.Point(168, 16);
            this.btnAddCollection.Name = "btnAddCollection";
            this.btnAddCollection.Size = new System.Drawing.Size(108, 23);
            this.btnAddCollection.TabIndex = 1;
            this.btnAddCollection.Text = "New Collection";
            this.btnAddCollection.UseVisualStyleBackColor = true;
            this.btnAddCollection.Click += new System.EventHandler(this.btnAddCollection_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(156, 23);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnImportCards
            // 
            this.btnImportCards.Location = new System.Drawing.Point(387, 324);
            this.btnImportCards.Name = "btnImportCards";
            this.btnImportCards.Size = new System.Drawing.Size(81, 23);
            this.btnImportCards.TabIndex = 14;
            this.btnImportCards.Text = "import YDK";
            this.btnImportCards.UseVisualStyleBackColor = true;
            this.btnImportCards.Click += new System.EventHandler(this.btnImportCards_Click);
            // 
            // BGSearchAdd
            // 
            this.BGSearchAdd.Controls.Add(this.cmbFilterBy);
            this.BGSearchAdd.Controls.Add(this.label1);
            this.BGSearchAdd.Controls.Add(this.chkShowRarity);
            this.BGSearchAdd.Controls.Add(this.chkShowSet);
            this.BGSearchAdd.Controls.Add(this.txtSearch);
            this.BGSearchAdd.Location = new System.Drawing.Point(12, 58);
            this.BGSearchAdd.Name = "BGSearchAdd";
            this.BGSearchAdd.Size = new System.Drawing.Size(162, 112);
            this.BGSearchAdd.TabIndex = 15;
            this.BGSearchAdd.TabStop = false;
            this.BGSearchAdd.Text = "Card Search";
            // 
            // chkPaperCollection
            // 
            this.chkPaperCollection.AutoSize = true;
            this.chkPaperCollection.Location = new System.Drawing.Point(474, 326);
            this.chkPaperCollection.Name = "chkPaperCollection";
            this.chkPaperCollection.Size = new System.Drawing.Size(113, 19);
            this.chkPaperCollection.TabIndex = 16;
            this.chkPaperCollection.Text = "Paper Collection";
            this.chkPaperCollection.UseVisualStyleBackColor = true;
            this.chkPaperCollection.CheckedChanged += new System.EventHandler(this.chkPaperCollection_CheckedChanged);
            // 
            // txtInventoryFilter
            // 
            this.txtInventoryFilter.Location = new System.Drawing.Point(75, 324);
            this.txtInventoryFilter.Name = "txtInventoryFilter";
            this.txtInventoryFilter.Size = new System.Drawing.Size(144, 23);
            this.txtInventoryFilter.TabIndex = 17;
            this.txtInventoryFilter.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // InventoryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 672);
            this.Controls.Add(this.txtInventoryFilter);
            this.Controls.Add(this.chkPaperCollection);
            this.Controls.Add(this.BGSearchAdd);
            this.Controls.Add(this.btnImportCards);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbOrderBy);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.gbSelectedCard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbSearchResults);
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
            this.groupBox1.ResumeLayout(false);
            this.BGSearchAdd.ResumeLayout(false);
            this.BGSearchAdd.PerformLayout();
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
        private GroupBox groupBox1;
        private Button btnDeleteCollection;
        private Button btnAddCollection;
        private ComboBox comboBox1;
        private Button BTNImportCollection;
        private Button btnRenameCollection;
        private Button btnImportCards;
        private GroupBox BGSearchAdd;
        private CheckBox chkPaperCollection;
        private TextBox txtInventoryFilter;
    }
}