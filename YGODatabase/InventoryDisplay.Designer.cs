namespace YGODatabase
{
    partial class InventoryDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventoryDisplay));
            this.listView1 = new System.Windows.Forms.ListView();
            this.Count = new System.Windows.Forms.ColumnHeader();
            this.CardName = new System.Windows.Forms.ColumnHeader();
            this.Set = new System.Windows.Forms.ColumnHeader();
            this.Rarity = new System.Windows.Forms.ColumnHeader();
            this.Condition = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkInvDescending = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInventoryFilter = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbOrderBy = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            this.listView1.Location = new System.Drawing.Point(12, 127);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(574, 478);
            this.listView1.TabIndex = 10;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // Count
            // 
            this.Count.Text = "#";
            this.Count.Width = 23;
            // 
            // CardName
            // 
            this.CardName.Text = "Card";
            this.CardName.Width = 290;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkInvDescending);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtInventoryFilter);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbOrderBy);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 109);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Collection:";
            // 
            // chkInvDescending
            // 
            this.chkInvDescending.AutoSize = true;
            this.chkInvDescending.Location = new System.Drawing.Point(137, 74);
            this.chkInvDescending.Name = "chkInvDescending";
            this.chkInvDescending.Size = new System.Drawing.Size(51, 19);
            this.chkInvDescending.TabIndex = 18;
            this.chkInvDescending.Text = "Desc";
            this.chkInvDescending.UseVisualStyleBackColor = true;
            this.chkInvDescending.CheckedChanged += new System.EventHandler(this.UpdateCollectionDetails);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 17);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(182, 23);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.UpdateCollectionDetails);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search";
            // 
            // txtInventoryFilter
            // 
            this.txtInventoryFilter.Location = new System.Drawing.Point(65, 46);
            this.txtInventoryFilter.Name = "txtInventoryFilter";
            this.txtInventoryFilter.Size = new System.Drawing.Size(123, 23);
            this.txtInventoryFilter.TabIndex = 17;
            this.txtInventoryFilter.TextChanged += new System.EventHandler(this.UpdateCollectionDetails);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Order By";
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
            "Date Modified",
            "Card Type",
            "Count"});
            this.cmbOrderBy.Location = new System.Drawing.Point(65, 72);
            this.cmbOrderBy.Name = "cmbOrderBy";
            this.cmbOrderBy.Size = new System.Drawing.Size(66, 23);
            this.cmbOrderBy.TabIndex = 10;
            this.cmbOrderBy.SelectedIndexChanged += new System.EventHandler(this.UpdateCollectionDetails);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(508, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(78, 108);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(350, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(152, 109);
            this.richTextBox1.TabIndex = 16;
            this.richTextBox1.Text = "";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(213, 11);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(131, 109);
            this.listBox1.TabIndex = 17;
            // 
            // InventoryDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 617);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listView1);
            this.Name = "InventoryDisplay";
            this.Text = "InventoryDisplay";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ListView listView1;
        private ColumnHeader Count;
        private ColumnHeader CardName;
        private ColumnHeader Set;
        private ColumnHeader Rarity;
        private ColumnHeader Condition;
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private CheckBox chkInvDescending;
        private Label label2;
        private TextBox txtInventoryFilter;
        private Label label3;
        private ComboBox cmbOrderBy;
        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private ListBox listBox1;
    }
}