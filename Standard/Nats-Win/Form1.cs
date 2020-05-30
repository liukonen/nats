using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace Nats_Win
{
    public partial class Form1 : Form
    {
        #region Globals
        string path;
        NATS.ArgumentsObject.ArgumentsObject.eSearchType SearchType;
        NATS.Filters.FileExtentionFilter.filterType FilterType;
        int threadCount = 4;
        List<string> BlackListItems = new List<string>();
        List<string> WhiteListItems = new List<string>();
        List<string> responseItems = new List<string>();
        #endregion

        #region Properties
        private string FilterString
        {
            get
            {
                return (approvedExtensionsToolStripMenuItem.Checked) ? string.Join("|", WhiteListItems) : string.Join("|", BlackListItems);
            }
        }
        #endregion

        #region Form Load
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            SetFilterDropdowns(true);

     
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BlackListItems.AddRange(nats_standard.Nats.DefaultBlackList());
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded;
            FilterType = NATS.Filters.FileExtentionFilter.filterType.BlackList;
            path = OpenFolder();
            this.Text = "Nats - " + path;
        }
        #endregion

        #region ToolStrip Click Events

        #region File
        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = OpenFolder();
            if (!string.IsNullOrWhiteSpace(test)) { path = test; }
            this.Text = "Nats - " + path;
        }

        private void saveOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (responseItems.Count > 0)
            {
                using (SaveFileDialog dia = new SaveFileDialog())
                {
                    dia.FileName = string.Concat(textBox1.Text, " ", System.DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"), ".txt"); ;
                    dia.Filter = "text|*.txt";
                    if (responseItems.Count > 0)
                    {
                        if (dia.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                var filename = dia.FileName;
                                if (string.IsNullOrWhiteSpace(filename)) { MessageBox.Show("err", "did not save, no filename selecte"); }
                                else if (System.IO.File.Exists(filename)) { MessageBox.Show("err", "File already exists, please use a different name"); }
                                else { System.IO.File.WriteAllText(filename, string.Join(Environment.NewLine, responseItems)); }
                            }
                            catch (Exception x) { MessageBox.Show("err", x.Message, MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { }
                    }
                }
            }
            else
            {
                MessageBox.Show("err", "you need to perform a search first", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Engine
        private void multiThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are " + threadCount + " threads correct", "Thread Count", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                int I = 0;
                while (I == 0)
                {

                    int.TryParse(Interaction.InputBox("Enter Number of threads", "threads", "4"), out I);
                }
            }
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Threaded;
            singleThreadToolStripMenuItem.Checked = false;
            windowsIndexToolStripMenuItem.Checked = false;
            multiThreadToolStripMenuItem.Checked = true;
            localIndexToolStripMenuItem.Checked = false;
            basicToolStripMenuItem.Checked = false;
            populateAndSearchToolStripMenuItem.Checked = false;
        }

        private void singleThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Single;
            singleThreadToolStripMenuItem.Checked = true;
            windowsIndexToolStripMenuItem.Checked = false;
            multiThreadToolStripMenuItem.Checked = false;
            localIndexToolStripMenuItem.Checked = false;
            basicToolStripMenuItem.Checked = false;
            populateAndSearchToolStripMenuItem.Checked = false;
        }


        /// <summary>
        /// INDEXES NOT WORKING
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowsIndexToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.WindowsIndex;
            singleThreadToolStripMenuItem.Checked = false;
            windowsIndexToolStripMenuItem.Checked = true;
            multiThreadToolStripMenuItem.Checked = false;
            localIndexToolStripMenuItem.Checked = false;
            basicToolStripMenuItem.Checked = false;
            populateAndSearchToolStripMenuItem.Checked = false;
        }

        /// <summary>
        /// INDEXES NOT WORKING
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex;
            singleThreadToolStripMenuItem.Checked = false;
            windowsIndexToolStripMenuItem.Checked = false;
            multiThreadToolStripMenuItem.Checked = false;
            localIndexToolStripMenuItem.Checked = true;
            basicToolStripMenuItem.Checked = true;
            populateAndSearchToolStripMenuItem.Checked = false;
        }

        /// <summary>
        /// INDEXS NOT WORKING
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void populateAndSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch;
            singleThreadToolStripMenuItem.Checked = false;
            windowsIndexToolStripMenuItem.Checked = false;
            multiThreadToolStripMenuItem.Checked = false;
            localIndexToolStripMenuItem.Checked = true;
            basicToolStripMenuItem.Checked = false;
            populateAndSearchToolStripMenuItem.Checked = true;

        }
        
        /// <summary>
        /// NEED TO IMPLEMENT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Options
        private void smartSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            smartSearchToolStripMenuItem.Checked = !smartSearchToolStripMenuItem.Checked;
        }

        private void ramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ramToolStripMenuItem.Checked = !ramToolStripMenuItem.Checked;
        }

        private void multilineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multilineToolStripMenuItem.Checked = !multilineToolStripMenuItem.Checked;
        }

        private void disapppvedExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilterDropdowns(true);
        }

        private void approvedExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilterDropdowns(false);
        }
        #endregion

        #region Help
        private void helpToolStripMenuItem_Click(object sender, EventArgs e){}
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(nats_standard.Nats.GUIHelpFile(), "Help", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nats License MIT, Copyright 2020 Luke Liukonen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #endregion

        #region Custom Event Handles

        /// <summary>
        /// White List Black List Direct Call (if you hit any of the white list black list items, for removing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WBDC(object sender, EventArgs e)
        {
            string Ext = ((ToolStripItem)sender).Text;
            if (MessageBox.Show("Are you sure you want to remove " + Ext, "Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)


                if (disapppvedExtensionsToolStripMenuItem.Checked)
                {
                    BlackListItems.Remove(Ext);
                    disapppvedExtensionsToolStripMenuItem.DropDownItems.Remove((ToolStripItem)sender);
                }
                else
                {
                    WhiteListItems.Remove(Ext);
                    approvedExtensionsToolStripMenuItem.DropDownItems.Remove((ToolStripItem)sender);
                }
        }

        /// <summary>
        /// Add Item Control on both the Black and white list dropdowns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddItem(object sender, EventArgs e)
        {
            string item;
            string inputbox = Interaction.InputBox("File Extension", "Extension", "4");
            if (!string.IsNullOrWhiteSpace(inputbox))
            {

                item = inputbox.Trim();
                if (item.StartsWith("*")) { item = item.Substring(1); }
                if (item.StartsWith(".")) { item = item.Substring(1); }


                if (disapppvedExtensionsToolStripMenuItem.Checked)
                {
                    if (!BlackListItems.Contains(item))
                    {
                        BlackListItems.Add(item);
                        disapppvedExtensionsToolStripMenuItem.DropDownItems.Add(item, null, WBDC);
                    }
                }
                else
                {
                    if (!WhiteListItems.Contains(item))
                    {
                        WhiteListItems.Add(item);
                        approvedExtensionsToolStripMenuItem.DropDownItems.Add(item, null, WBDC);
                    }
                }
            }
            else { MessageBox.Show("unable to grab the value, please try again"); }

        }
        #endregion

        #region Form Object Events

        /// <summary>
        /// Search button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                MessageBox.Show("Search Terminated");
                button1.Text = "Search";
            }
            else
            {
                listBox1.Items.Clear();
                button1.Text = "Cancel";
                listBox1.Items.Clear();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                else { MessageBox.Show("your path is not defined, please select a path."); }
            }
        }

        #region Background Worker Calls

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var arg = new NATS.ArgumentsObject.ArgumentsObject(FilterType, FilterString, smartSearchToolStripMenuItem.Checked)
            {
                DirectoryPath = path,
                KeywordSearch = textBox1.Text,
                SearchType = SearchType,
                ThreadCount = threadCount,
                MultiLine = multilineToolStripMenuItem.Checked,
                MemoryLoad = ramToolStripMenuItem.Checked

            };

            nats_standard.Nats nats = new nats_standard.Nats();

            responseItems.AddRange(nats.OldSearch(arg).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.Items.AddRange(responseItems.ToArray());
            button1.Text = "Search";
            MessageBox.Show("Search Complete");
        }
        #endregion

        #endregion

        #region Helper functions
        public string OpenFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog())
            {

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    return folderBrowserDialog1.SelectedPath;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates the Dropdown custom items for Blacklist and whitelist
        /// </summary>
        /// <param name="isBlackList"></param>
        private void SetFilterDropdowns(bool isBlackList)
        {
            disapppvedExtensionsToolStripMenuItem.Checked = isBlackList;
            approvedExtensionsToolStripMenuItem.Checked = !isBlackList;
            disapppvedExtensionsToolStripMenuItem.DropDownItems.Clear();
            approvedExtensionsToolStripMenuItem.DropDownItems.Clear();
            if (isBlackList)
            {

                disapppvedExtensionsToolStripMenuItem.DropDownItems.Add("+Add", null, AddItem);
                foreach (string items in BlackListItems)
                {
                    disapppvedExtensionsToolStripMenuItem.DropDownItems.Add(items, null, WBDC);
                }
            }
            else
            {

                approvedExtensionsToolStripMenuItem.DropDownItems.Add("+Add", null, AddItem);
                foreach (string items in WhiteListItems)
                {
                    approvedExtensionsToolStripMenuItem.DropDownItems.Add(items, null, WBDC);
                }
            }

        }
        #endregion


    }
}
