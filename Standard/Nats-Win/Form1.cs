using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nats_Win
{
    public partial class Form1 : Form
    {

        string path;
        NATS.ArgumentsObject.ArgumentsObject.eSearchType SearchType;
        NATS.Filters.FileExtentionFilter.filterType FilterType;
        int threadCount = 4;
        public Form1()
        {

        //    using (FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog())
        //    {

        //        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        //        {
        //            path = folderBrowserDialog1.SelectedPath;
        //        }
        //    }
            BlackListItems.AddRange(nats_standard.Nats.DefaultBlackList());
            SearchType = NATS.ArgumentsObject.ArgumentsObject.eSearchType.Single;
            FilterType = NATS.Filters.FileExtentionFilter.filterType.BlackList;

            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            SetFilterDropdowns(true);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox1.Items.AddRange(responseItems.ToArray());
            button1.Text = "Search";
            MessageBox.Show("Seach Complete");
            //throw new NotImplementedException();
        }

        private void multiThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are " + threadCount + " threads correct", "Thread Count", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                int I = 0;
                while (I == 0)
                {
                    int.TryParse(Essy.Tools.InputBox.InputBox.ShowInputBox("Enter Number of Threads", "4", false), out I);
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




        private void disapppvedExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilterDropdowns(true);


        }


        private void SetFilterDropdowns(bool isBlackList)
        {
            disapppvedExtensionsToolStripMenuItem.Checked = isBlackList;
            approvedExtensionsToolStripMenuItem.Checked = !isBlackList;
            disapppvedExtensionsToolStripMenuItem.DropDownItems.Clear();
            approvedExtensionsToolStripMenuItem.DropDownItems.Clear();
            if (isBlackList) {

                disapppvedExtensionsToolStripMenuItem.DropDownItems.Add("+Add", null, AddItem);
                foreach (string items in BlackListItems)
                {
                    disapppvedExtensionsToolStripMenuItem.DropDownItems.Add(items, null, WBDC);
                }
            }
            else {

                approvedExtensionsToolStripMenuItem.DropDownItems.Add("+Add", null, AddItem);
                foreach (string items in WhiteListItems)
                {
                    approvedExtensionsToolStripMenuItem.DropDownItems.Add(items, null, WBDC);
                }
            }

        }

        private void approvedExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilterDropdowns(false);
        }

        List<string> BlackListItems = new List<string>();
        List<string> WhiteListItems = new List<string>();

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

        private void AddItem(object sender, EventArgs e)
        {
            string item;
            string inputbox = Essy.Tools.InputBox.InputBox.ShowInputBox("File Extention");
            if (!string.IsNullOrWhiteSpace(inputbox))
            {

                item = inputbox.Trim();
                if (item.StartsWith("*")) { item = item.Substring(1); }
                if (item.StartsWith(".")) { item = item.Substring(1); }


                if (disapppvedExtensionsToolStripMenuItem.Checked) { if (!BlackListItems.Contains(item)) { BlackListItems.Add(item);
                        disapppvedExtensionsToolStripMenuItem.DropDownItems.Add(item, null, WBDC);
                    } }
                else { if (!WhiteListItems.Contains(item)) {
                        WhiteListItems.Add(item);
                        approvedExtensionsToolStripMenuItem.DropDownItems.Add(item, null, WBDC);
                    } }
            }
            else { MessageBox.Show("unable to grab the value, please try again"); }

        }

        private string FilterString {get{
                return (approvedExtensionsToolStripMenuItem.Checked) ? string.Join("|", WhiteListItems) : string.Join("|", BlackListItems);
            } }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy) {
                backgroundWorker1.CancelAsync();
                MessageBox.Show("Search Terminated");
                button1.Text = "Search";
            }
            else
            {
                button1.Text = "Cancel";
                listBox1.Items.Clear();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    backgroundWorker1.RunWorkerAsync();
                }
                else { MessageBox.Show("your path is not defined, please select a path."); }
            }
        }

        private void multilineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multilineToolStripMenuItem.Checked = !multilineToolStripMenuItem.Checked;
        }

        private void ramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ramToolStripMenuItem.Checked = !ramToolStripMenuItem.Checked;
        }

        private void localIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        List<string> responseItems = new List<string>();

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

            responseItems.AddRange( nats.OldSearch(arg).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }
       
    }
}
