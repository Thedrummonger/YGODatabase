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
            this.lbInventory = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbSelectedCard = new System.Windows.Forms.GroupBox();
            this.lblSelectedCardModified = new System.Windows.Forms.Label();
            this.lblSelectedCardAdded = new System.Windows.Forms.Label();
            this.cmbSelectedCardCondition = new System.Windows.Forms.ComboBox();
            this.cmbSelectedCardSet = new System.Windows.Forms.ComboBox();
            this.cmbSelctedCardRarity = new System.Windows.Forms.ComboBox();
            this.lblSelectedCard = new System.Windows.Forms.Label();
            this.chkShowSet = new System.Windows.Forms.CheckBox();
            this.chkShowRarity = new System.Windows.Forms.CheckBox();
            this.gbSelectedCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 68);
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
            this.lbSearchResults.Location = new System.Drawing.Point(12, 103);
            this.lbSearchResults.Name = "lbSearchResults";
            this.lbSearchResults.Size = new System.Drawing.Size(479, 124);
            this.lbSearchResults.TabIndex = 1;
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
            // lbInventory
            // 
            this.lbInventory.FormattingEnabled = true;
            this.lbInventory.ItemHeight = 15;
            this.lbInventory.Location = new System.Drawing.Point(12, 253);
            this.lbInventory.Name = "lbInventory";
            this.lbInventory.Size = new System.Drawing.Size(479, 274);
            this.lbInventory.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Inventory";
            // 
            // gbSelectedCard
            // 
            this.gbSelectedCard.Controls.Add(this.lblSelectedCardModified);
            this.gbSelectedCard.Controls.Add(this.lblSelectedCardAdded);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardCondition);
            this.gbSelectedCard.Controls.Add(this.cmbSelectedCardSet);
            this.gbSelectedCard.Controls.Add(this.cmbSelctedCardRarity);
            this.gbSelectedCard.Controls.Add(this.lblSelectedCard);
            this.gbSelectedCard.Location = new System.Drawing.Point(180, 6);
            this.gbSelectedCard.Name = "gbSelectedCard";
            this.gbSelectedCard.Size = new System.Drawing.Size(311, 91);
            this.gbSelectedCard.TabIndex = 6;
            this.gbSelectedCard.TabStop = false;
            this.gbSelectedCard.Text = "Last Card added";
            // 
            // lblSelectedCardModified
            // 
            this.lblSelectedCardModified.AutoSize = true;
            this.lblSelectedCardModified.Location = new System.Drawing.Point(133, 29);
            this.lblSelectedCardModified.Name = "lblSelectedCardModified";
            this.lblSelectedCardModified.Size = new System.Drawing.Size(79, 15);
            this.lblSelectedCardModified.TabIndex = 8;
            this.lblSelectedCardModified.Text = "Last modified";
            // 
            // lblSelectedCardAdded
            // 
            this.lblSelectedCardAdded.AutoSize = true;
            this.lblSelectedCardAdded.Location = new System.Drawing.Point(7, 29);
            this.lblSelectedCardAdded.Name = "lblSelectedCardAdded";
            this.lblSelectedCardAdded.Size = new System.Drawing.Size(69, 15);
            this.lblSelectedCardAdded.TabIndex = 7;
            this.lblSelectedCardAdded.Text = "Date Added";
            // 
            // cmbSelectedCardCondition
            // 
            this.cmbSelectedCardCondition.FormattingEnabled = true;
            this.cmbSelectedCardCondition.Location = new System.Drawing.Point(207, 62);
            this.cmbSelectedCardCondition.Name = "cmbSelectedCardCondition";
            this.cmbSelectedCardCondition.Size = new System.Drawing.Size(94, 23);
            this.cmbSelectedCardCondition.TabIndex = 3;
            this.cmbSelectedCardCondition.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // cmbSelectedCardSet
            // 
            this.cmbSelectedCardSet.FormattingEnabled = true;
            this.cmbSelectedCardSet.Location = new System.Drawing.Point(7, 62);
            this.cmbSelectedCardSet.Name = "cmbSelectedCardSet";
            this.cmbSelectedCardSet.Size = new System.Drawing.Size(94, 23);
            this.cmbSelectedCardSet.TabIndex = 2;
            this.cmbSelectedCardSet.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // cmbSelctedCardRarity
            // 
            this.cmbSelctedCardRarity.FormattingEnabled = true;
            this.cmbSelctedCardRarity.Location = new System.Drawing.Point(107, 62);
            this.cmbSelctedCardRarity.Name = "cmbSelctedCardRarity";
            this.cmbSelctedCardRarity.Size = new System.Drawing.Size(94, 23);
            this.cmbSelctedCardRarity.TabIndex = 1;
            this.cmbSelctedCardRarity.SelectedIndexChanged += new System.EventHandler(this.SelectedCardValueEdited);
            // 
            // lblSelectedCard
            // 
            this.lblSelectedCard.AutoSize = true;
            this.lblSelectedCard.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
            this.lblSelectedCard.Location = new System.Drawing.Point(7, 14);
            this.lblSelectedCard.Name = "lblSelectedCard";
            this.lblSelectedCard.Size = new System.Drawing.Size(40, 15);
            this.lblSelectedCard.TabIndex = 0;
            this.lblSelectedCard.Text = "Name";
            // 
            // chkShowSet
            // 
            this.chkShowSet.AutoSize = true;
            this.chkShowSet.Location = new System.Drawing.Point(12, 39);
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
            this.chkShowRarity.Location = new System.Drawing.Point(86, 39);
            this.chkShowRarity.Name = "chkShowRarity";
            this.chkShowRarity.Size = new System.Drawing.Size(88, 19);
            this.chkShowRarity.TabIndex = 8;
            this.chkShowRarity.Text = "Show Rarity";
            this.chkShowRarity.UseVisualStyleBackColor = true;
            this.chkShowRarity.CheckedChanged += new System.EventHandler(this.UpdateSearchResults);
            // 
            // InventoryManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 539);
            this.Controls.Add(this.chkShowRarity);
            this.Controls.Add(this.chkShowSet);
            this.Controls.Add(this.gbSelectedCard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbInventory);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtSearch;
        private ListBox lbSearchResults;
        private Label label1;
        private ComboBox cmbFilterBy;
        private ListBox lbInventory;
        private Label label2;
        private GroupBox gbSelectedCard;
        private CheckBox chkShowSet;
        private CheckBox chkShowRarity;
        private ComboBox cmbSelectedCardCondition;
        private ComboBox cmbSelectedCardSet;
        private ComboBox cmbSelctedCardRarity;
        private Label lblSelectedCard;
        private Label lblSelectedCardModified;
        private Label lblSelectedCardAdded;
    }
}