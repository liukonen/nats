namespace Nats_Win
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.populateAndSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smartSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disapppvedExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.approvedExtensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multilineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchTypesToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(409, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pathToolStripMenuItem,
            this.saveOutputToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveOutputToolStripMenuItem
            // 
            this.saveOutputToolStripMenuItem.Name = "saveOutputToolStripMenuItem";
            this.saveOutputToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveOutputToolStripMenuItem.Text = "Save";
            this.saveOutputToolStripMenuItem.Click += new System.EventHandler(this.saveOutputToolStripMenuItem_Click);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pathToolStripMenuItem.Text = "&Path";
            this.pathToolStripMenuItem.Click += new System.EventHandler(this.pathToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // searchTypesToolStripMenuItem
            // 
            this.searchTypesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singleThreadToolStripMenuItem,
            this.multiThreadToolStripMenuItem,
            this.windowsIndexToolStripMenuItem,
            this.localIndexToolStripMenuItem});
            this.searchTypesToolStripMenuItem.Name = "searchTypesToolStripMenuItem";
            this.searchTypesToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.searchTypesToolStripMenuItem.Text = "S&earch Types";
            // 
            // singleThreadToolStripMenuItem
            // 
            this.singleThreadToolStripMenuItem.Name = "singleThreadToolStripMenuItem";
            this.singleThreadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.singleThreadToolStripMenuItem.Text = "Si&ngle Thread";
            this.singleThreadToolStripMenuItem.Click += new System.EventHandler(this.singleThreadToolStripMenuItem_Click);
            // 
            // multiThreadToolStripMenuItem
            // 
            this.multiThreadToolStripMenuItem.Checked = true;
            this.multiThreadToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.multiThreadToolStripMenuItem.Name = "multiThreadToolStripMenuItem";
            this.multiThreadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.multiThreadToolStripMenuItem.Text = "Multi &Thread (4)";
            this.multiThreadToolStripMenuItem.Click += new System.EventHandler(this.multiThreadToolStripMenuItem_Click);
            // 
            // windowsIndexToolStripMenuItem
            // 
            this.windowsIndexToolStripMenuItem.Name = "windowsIndexToolStripMenuItem";
            this.windowsIndexToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.windowsIndexToolStripMenuItem.Text = "&Windows Index";
            this.windowsIndexToolStripMenuItem.Visible = false;
            this.windowsIndexToolStripMenuItem.Click += new System.EventHandler(this.windowsIndexToolStripMenuItem_Click_1);
            // 
            // localIndexToolStripMenuItem
            // 
            this.localIndexToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicToolStripMenuItem,
            this.populateAndSearchToolStripMenuItem});
            this.localIndexToolStripMenuItem.Name = "localIndexToolStripMenuItem";
            this.localIndexToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.localIndexToolStripMenuItem.Text = "Local &Index";
            this.localIndexToolStripMenuItem.Visible = false;
            this.localIndexToolStripMenuItem.Click += new System.EventHandler(this.localIndexToolStripMenuItem_Click);
            // 
            // basicToolStripMenuItem
            // 
            this.basicToolStripMenuItem.Name = "basicToolStripMenuItem";
            this.basicToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.basicToolStripMenuItem.Text = "Search (&Limited)";
            this.basicToolStripMenuItem.Click += new System.EventHandler(this.basicToolStripMenuItem_Click);
            // 
            // populateAndSearchToolStripMenuItem
            // 
            this.populateAndSearchToolStripMenuItem.Name = "populateAndSearchToolStripMenuItem";
            this.populateAndSearchToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.populateAndSearchToolStripMenuItem.Text = "Populate and Search";
            this.populateAndSearchToolStripMenuItem.Click += new System.EventHandler(this.populateAndSearchToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smartSearchToolStripMenuItem,
            this.ramToolStripMenuItem,
            this.disapppvedExtensionsToolStripMenuItem,
            this.approvedExtensionsToolStripMenuItem,
            this.multilineToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // smartSearchToolStripMenuItem
            // 
            this.smartSearchToolStripMenuItem.Name = "smartSearchToolStripMenuItem";
            this.smartSearchToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.smartSearchToolStripMenuItem.Text = "&Smart Search";
            this.smartSearchToolStripMenuItem.Click += new System.EventHandler(this.smartSearchToolStripMenuItem_Click);
            // 
            // ramToolStripMenuItem
            // 
            this.ramToolStripMenuItem.Checked = true;
            this.ramToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ramToolStripMenuItem.Name = "ramToolStripMenuItem";
            this.ramToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.ramToolStripMenuItem.Text = "&Ram";
            this.ramToolStripMenuItem.Click += new System.EventHandler(this.ramToolStripMenuItem_Click);
            // 
            // disapppvedExtensionsToolStripMenuItem
            // 
            this.disapppvedExtensionsToolStripMenuItem.Checked = true;
            this.disapppvedExtensionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disapppvedExtensionsToolStripMenuItem.Name = "disapppvedExtensionsToolStripMenuItem";
            this.disapppvedExtensionsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.disapppvedExtensionsToolStripMenuItem.Text = "&Disapppved Extensions";
            this.disapppvedExtensionsToolStripMenuItem.Click += new System.EventHandler(this.disapppvedExtensionsToolStripMenuItem_Click);
            // 
            // approvedExtensionsToolStripMenuItem
            // 
            this.approvedExtensionsToolStripMenuItem.Name = "approvedExtensionsToolStripMenuItem";
            this.approvedExtensionsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.approvedExtensionsToolStripMenuItem.Text = "&Approved Extensions";
            this.approvedExtensionsToolStripMenuItem.Click += new System.EventHandler(this.approvedExtensionsToolStripMenuItem_Click);
            // 
            // multilineToolStripMenuItem
            // 
            this.multilineToolStripMenuItem.Name = "multilineToolStripMenuItem";
            this.multilineToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.multilineToolStripMenuItem.Text = "&Multiline";
            this.multilineToolStripMenuItem.Click += new System.EventHandler(this.multilineToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(409, 288);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(334, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(334, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(409, 259);
            this.listBox1.TabIndex = 0;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 312);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Nats - ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singleThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem populateAndSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smartSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disapppvedExtensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem approvedExtensionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multilineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

