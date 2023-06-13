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
            this.label5 = new System.Windows.Forms.Label();
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.BGSearchAdd = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtInventoryFilter = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importYDKAsCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageCurrentCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importYDKContentToCurrentInventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCollectionToInventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isPaperCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gbSelectedCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.BGSearchAdd.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(6, 83);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(201, 23);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.UpdateSearchResults);
            this.txtSearch.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CaptureTabPress);
            // 
            // lbSearchResults
            // 
            this.lbSearchResults.CausesValidation = false;
            this.lbSearchResults.FormattingEnabled = true;
            this.lbSearchResults.ItemHeight = 15;
            this.lbSearchResults.Location = new System.Drawing.Point(12, 196);
            this.lbSearchResults.Name = "lbSearchResults";
            this.lbSearchResults.Size = new System.Drawing.Size(384, 184);
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
            this.cmbFilterBy.Size = new System.Drawing.Size(144, 23);
            this.cmbFilterBy.TabIndex = 3;
            this.cmbFilterBy.SelectedIndexChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search";
            // 
            // gbSelectedCard
            // 
            this.gbSelectedCard.Controls.Add(this.label5);
            this.gbSelectedCard.Controls.Add(this.BtnAddOneSelected);
            this.gbSelectedCard.Controls.Add(this.btnRemoveSelected);
            this.gbSelectedCard.Controls.Add(this.numericUpDown1);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardCondition);
            this.gbSelectedCard.Controls.Add(this.lblSelectedCard);
            this.gbSelectedCard.Controls.Add(this.cmbSelctedCardRarity);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardSet);
            this.gbSelectedCard.Location = new System.Drawing.Point(231, 27);
            this.gbSelectedCard.Name = "gbSelectedCard";
            this.gbSelectedCard.Size = new System.Drawing.Size(166, 163);
            this.gbSelectedCard.TabIndex = 6;
            this.gbSelectedCard.TabStop = false;
            this.gbSelectedCard.Text = "Last Card added";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(117, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 24);
            this.label5.TabIndex = 12;
            this.label5.Text = "Select\r\nAmount";
            // 
            // BtnAddOneSelected
            // 
            this.BtnAddOneSelected.Location = new System.Drawing.Point(6, 72);
            this.BtnAddOneSelected.Name = "BtnAddOneSelected";
            this.BtnAddOneSelected.Size = new System.Drawing.Size(109, 23);
            this.BtnAddOneSelected.TabIndex = 11;
            this.BtnAddOneSelected.Text = "Add One Copy";
            this.BtnAddOneSelected.UseVisualStyleBackColor = true;
            this.BtnAddOneSelected.Click += new System.EventHandler(this.btnAddOneSelected_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(6, 43);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(109, 23);
            this.btnRemoveSelected.TabIndex = 10;
            this.btnRemoveSelected.Text = "Remove X Copies";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(121, 72);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(37, 23);
            this.numericUpDown1.TabIndex = 9;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // cmbSelectedCardCondition
            // 
            this.cmbSelectedCardCondition.FormattingEnabled = true;
            this.cmbSelectedCardCondition.Location = new System.Drawing.Point(99, 134);
            this.cmbSelectedCardCondition.Name = "cmbSelectedCardCondition";
            this.cmbSelectedCardCondition.Size = new System.Drawing.Size(59, 23);
            this.cmbSelectedCardCondition.TabIndex = 3;
            this.cmbSelectedCardCondition.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelectedCardCondition.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // lblSelectedCard
            // 
            this.lblSelectedCard.AutoSize = true;
            this.lblSelectedCard.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.lblSelectedCard.Location = new System.Drawing.Point(6, 22);
            this.lblSelectedCard.Name = "lblSelectedCard";
            this.lblSelectedCard.Size = new System.Drawing.Size(40, 15);
            this.lblSelectedCard.TabIndex = 0;
            this.lblSelectedCard.Text = "Name";
            // 
            // cmbSelctedCardRarity
            // 
            this.cmbSelctedCardRarity.FormattingEnabled = true;
            this.cmbSelctedCardRarity.Location = new System.Drawing.Point(6, 134);
            this.cmbSelctedCardRarity.Name = "cmbSelctedCardRarity";
            this.cmbSelctedCardRarity.Size = new System.Drawing.Size(87, 23);
            this.cmbSelctedCardRarity.TabIndex = 1;
            this.cmbSelctedCardRarity.DropDown += new System.EventHandler(this.ComboBox_DropDown);
            this.cmbSelctedCardRarity.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // cmbSelectedCardSet
            // 
            this.cmbSelectedCardSet.FormattingEnabled = true;
            this.cmbSelectedCardSet.Location = new System.Drawing.Point(6, 105);
            this.cmbSelectedCardSet.Name = "cmbSelectedCardSet";
            this.cmbSelectedCardSet.Size = new System.Drawing.Size(152, 23);
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
            this.chkShowSet.Location = new System.Drawing.Point(104, 44);
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
            this.listView1.Location = new System.Drawing.Point(13, 390);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(574, 270);
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
            this.cmbOrderBy.Location = new System.Drawing.Point(66, 56);
            this.cmbOrderBy.Name = "cmbOrderBy";
            this.cmbOrderBy.Size = new System.Drawing.Size(101, 23);
            this.cmbOrderBy.TabIndex = 10;
            this.cmbOrderBy.SelectedIndexChanged += new System.EventHandler(this.cmbOrderBy_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Order By";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(403, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(184, 251);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 46);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Collection:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(201, 23);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // BGSearchAdd
            // 
            this.BGSearchAdd.Controls.Add(this.label4);
            this.BGSearchAdd.Controls.Add(this.cmbFilterBy);
            this.BGSearchAdd.Controls.Add(this.label1);
            this.BGSearchAdd.Controls.Add(this.chkShowRarity);
            this.BGSearchAdd.Controls.Add(this.chkShowSet);
            this.BGSearchAdd.Controls.Add(this.txtSearch);
            this.BGSearchAdd.Location = new System.Drawing.Point(12, 78);
            this.BGSearchAdd.Name = "BGSearchAdd";
            this.BGSearchAdd.Size = new System.Drawing.Size(213, 112);
            this.BGSearchAdd.TabIndex = 15;
            this.BGSearchAdd.TabStop = false;
            this.BGSearchAdd.Text = "Card Search";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Search";
            // 
            // txtInventoryFilter
            // 
            this.txtInventoryFilter.Location = new System.Drawing.Point(66, 25);
            this.txtInventoryFilter.Name = "txtInventoryFilter";
            this.txtInventoryFilter.Size = new System.Drawing.Size(101, 23);
            this.txtInventoryFilter.TabIndex = 17;
            this.txtInventoryFilter.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(598, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.importYDKAsCollectionToolStripMenuItem,
            this.manageCurrentCollectionToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.fileToolStripMenuItem.Text = "Collections";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.newToolStripMenuItem.Text = "Add New Collection";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.btnAddCollection_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.deleteToolStripMenuItem.Text = "Delete Current Collection";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.btnDeleteCollection_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.renameToolStripMenuItem.Text = "Rename Current Collection";
            // 
            // importYDKAsCollectionToolStripMenuItem
            // 
            this.importYDKAsCollectionToolStripMenuItem.Name = "importYDKAsCollectionToolStripMenuItem";
            this.importYDKAsCollectionToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importYDKAsCollectionToolStripMenuItem.Text = "Import YDK as New Collection";
            this.importYDKAsCollectionToolStripMenuItem.Click += new System.EventHandler(this.BTNImportCollection_Click);
            // 
            // manageCurrentCollectionToolStripMenuItem
            // 
            this.manageCurrentCollectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importYDKContentToCurrentInventoryToolStripMenuItem,
            this.addCollectionToInventoryToolStripMenuItem,
            this.isPaperCollectionToolStripMenuItem});
            this.manageCurrentCollectionToolStripMenuItem.Name = "manageCurrentCollectionToolStripMenuItem";
            this.manageCurrentCollectionToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.manageCurrentCollectionToolStripMenuItem.Text = "Manage Current Collection";
            // 
            // importYDKContentToCurrentInventoryToolStripMenuItem
            // 
            this.importYDKContentToCurrentInventoryToolStripMenuItem.Name = "importYDKContentToCurrentInventoryToolStripMenuItem";
            this.importYDKContentToCurrentInventoryToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.importYDKContentToCurrentInventoryToolStripMenuItem.Text = "Append YDK To Collection";
            this.importYDKContentToCurrentInventoryToolStripMenuItem.Click += new System.EventHandler(this.btnImportCards_Click);
            // 
            // addCollectionToInventoryToolStripMenuItem
            // 
            this.addCollectionToInventoryToolStripMenuItem.Name = "addCollectionToInventoryToolStripMenuItem";
            this.addCollectionToInventoryToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.addCollectionToInventoryToolStripMenuItem.Text = "Add Collection To Inventory";
            // 
            // isPaperCollectionToolStripMenuItem
            // 
            this.isPaperCollectionToolStripMenuItem.Name = "isPaperCollectionToolStripMenuItem";
            this.isPaperCollectionToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.isPaperCollectionToolStripMenuItem.Text = "Is Paper Collection";
            this.isPaperCollectionToolStripMenuItem.Click += new System.EventHandler(this.chkPaperCollection_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtInventoryFilter);
            this.groupBox2.Controls.Add(this.cmbOrderBy);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(403, 292);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(184, 92);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Collection";
            // 
            // InventoryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 672);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.BGSearchAdd);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.gbSelectedCard);
            this.Controls.Add(this.lbSearchResults);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private ComboBox comboBox1;
        private GroupBox BGSearchAdd;
        private TextBox txtInventoryFilter;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem importYDKAsCollectionToolStripMenuItem;
        private ToolStripMenuItem manageCurrentCollectionToolStripMenuItem;
        private ToolStripMenuItem importYDKContentToCurrentInventoryToolStripMenuItem;
        private ToolStripMenuItem addCollectionToInventoryToolStripMenuItem;
        private ToolStripMenuItem isPaperCollectionToolStripMenuItem;
        private GroupBox groupBox2;
        private Label label5;
        private Label label4;
    }
}