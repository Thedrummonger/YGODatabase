namespace YGODatabase
{
    partial class MainInterface
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainInterface));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inventoryManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lbCardList = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rtxtCardtext = new System.Windows.Forms.RichTextBox();
            this.lblCardText = new System.Windows.Forms.Label();
            this.lbldata = new System.Windows.Forms.Label();
            this.lbCardData = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(660, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inventoryManagerToolStripMenuItem,
            this.updateDatabaseToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // inventoryManagerToolStripMenuItem
            // 
            this.inventoryManagerToolStripMenuItem.Name = "inventoryManagerToolStripMenuItem";
            this.inventoryManagerToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.inventoryManagerToolStripMenuItem.Text = "Inventory Manager";
            this.inventoryManagerToolStripMenuItem.Click += new System.EventHandler(this.inventoryManagerToolStripMenuItem_Click);
            // 
            // updateDatabaseToolStripMenuItem
            // 
            this.updateDatabaseToolStripMenuItem.Name = "updateDatabaseToolStripMenuItem";
            this.updateDatabaseToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.updateDatabaseToolStripMenuItem.Text = "Update Database";
            this.updateDatabaseToolStripMenuItem.Click += new System.EventHandler(this.updateDatabaseToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(218, 23);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lbCardList
            // 
            this.lbCardList.FormattingEnabled = true;
            this.lbCardList.IntegralHeight = false;
            this.lbCardList.ItemHeight = 15;
            this.lbCardList.Location = new System.Drawing.Point(12, 56);
            this.lbCardList.Name = "lbCardList";
            this.lbCardList.Size = new System.Drawing.Size(218, 379);
            this.lbCardList.TabIndex = 2;
            this.lbCardList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(236, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(275, 379);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // rtxtCardtext
            // 
            this.rtxtCardtext.Location = new System.Drawing.Point(517, 255);
            this.rtxtCardtext.Name = "rtxtCardtext";
            this.rtxtCardtext.Size = new System.Drawing.Size(131, 180);
            this.rtxtCardtext.TabIndex = 4;
            this.rtxtCardtext.Text = "";
            // 
            // lblCardText
            // 
            this.lblCardText.AutoSize = true;
            this.lblCardText.Location = new System.Drawing.Point(517, 239);
            this.lblCardText.Name = "lblCardText";
            this.lblCardText.Size = new System.Drawing.Size(56, 15);
            this.lblCardText.TabIndex = 5;
            this.lblCardText.Text = "Card Text";
            // 
            // lbldata
            // 
            this.lbldata.AutoSize = true;
            this.lbldata.Location = new System.Drawing.Point(517, 38);
            this.lbldata.Name = "lbldata";
            this.lbldata.Size = new System.Drawing.Size(56, 15);
            this.lbldata.TabIndex = 7;
            this.lbldata.Text = "Card Info";
            // 
            // lbCardData
            // 
            this.lbCardData.FormattingEnabled = true;
            this.lbCardData.IntegralHeight = false;
            this.lbCardData.ItemHeight = 15;
            this.lbCardData.Location = new System.Drawing.Point(517, 56);
            this.lbCardData.Name = "lbCardData";
            this.lbCardData.Size = new System.Drawing.Size(131, 180);
            this.lbCardData.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(442, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Art:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(476, 30);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(35, 23);
            this.numericUpDown1.TabIndex = 10;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // MainInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 447);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbCardData);
            this.Controls.Add(this.lbldata);
            this.Controls.Add(this.lblCardText);
            this.Controls.Add(this.rtxtCardtext);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbCardList);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(676, 486);
            this.Name = "MainInterface";
            this.Text = "YGO Database";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainInterface_FormClosing);
            this.Load += new System.EventHandler(this.MainInterface_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResizeEnd += new System.EventHandler(this.MainInterface_ResizeEnd);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private TextBox textBox1;
        private ListBox lbCardList;
        private PictureBox pictureBox1;
        private RichTextBox rtxtCardtext;
        private Label lblCardText;
        private Label lbldata;
        private ListBox lbCardData;
        private ToolStripMenuItem inventoryManagerToolStripMenuItem;
        private ToolStripMenuItem updateDatabaseToolStripMenuItem;
        private Label label1;
        private NumericUpDown numericUpDown1;
    }
}