﻿namespace YGODatabase
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
            this.updateYGODatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lbCardList = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rtxtCardtext = new System.Windows.Forms.RichTextBox();
            this.lblCardText = new System.Windows.Forms.Label();
            this.lbldata = new System.Windows.Forms.Label();
            this.lbCardData = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.updateYGODatabaseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(660, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // updateYGODatabaseToolStripMenuItem
            // 
            this.updateYGODatabaseToolStripMenuItem.Name = "updateYGODatabaseToolStripMenuItem";
            this.updateYGODatabaseToolStripMenuItem.Size = new System.Drawing.Size(135, 20);
            this.updateYGODatabaseToolStripMenuItem.Text = "Update YGO Database";
            this.updateYGODatabaseToolStripMenuItem.Click += new System.EventHandler(this.updateYGODatabaseToolStripMenuItem_Click);
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
            this.lbCardList.ItemHeight = 15;
            this.lbCardList.Location = new System.Drawing.Point(12, 56);
            this.lbCardList.Name = "lbCardList";
            this.lbCardList.Size = new System.Drawing.Size(218, 379);
            this.lbCardList.TabIndex = 2;
            this.lbCardList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(236, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(275, 379);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // rtxtCardtext
            // 
            this.rtxtCardtext.Location = new System.Drawing.Point(517, 243);
            this.rtxtCardtext.Name = "rtxtCardtext";
            this.rtxtCardtext.Size = new System.Drawing.Size(131, 192);
            this.rtxtCardtext.TabIndex = 4;
            this.rtxtCardtext.Text = "";
            // 
            // lblCardText
            // 
            this.lblCardText.AutoSize = true;
            this.lblCardText.Location = new System.Drawing.Point(517, 225);
            this.lblCardText.Name = "lblCardText";
            this.lblCardText.Size = new System.Drawing.Size(53, 15);
            this.lblCardText.TabIndex = 5;
            this.lblCardText.Text = "CardText";
            // 
            // lbldata
            // 
            this.lbldata.AutoSize = true;
            this.lbldata.Location = new System.Drawing.Point(517, 38);
            this.lbldata.Name = "lbldata";
            this.lbldata.Size = new System.Drawing.Size(31, 15);
            this.lbldata.TabIndex = 7;
            this.lbldata.Text = "Data";
            // 
            // lbCardData
            // 
            this.lbCardData.FormattingEnabled = true;
            this.lbCardData.ItemHeight = 15;
            this.lbCardData.Location = new System.Drawing.Point(517, 56);
            this.lbCardData.Name = "lbCardData";
            this.lbCardData.Size = new System.Drawing.Size(131, 154);
            this.lbCardData.TabIndex = 8;
            // 
            // MainInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 450);
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
            this.Name = "MainInterface";
            this.Text = "YGO Database";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem updateYGODatabaseToolStripMenuItem;
        private TextBox textBox1;
        private ListBox lbCardList;
        private PictureBox pictureBox1;
        private RichTextBox rtxtCardtext;
        private Label lblCardText;
        private Label lbldata;
        private ListBox lbCardData;
    }
}