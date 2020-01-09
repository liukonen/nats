using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NATS
{
    class Scanner
    {
        #region properties
        private IEnumerable<string> Files { get { return System.IO.Directory.EnumerateFiles(Path, Whitelist, EOptions); } }
        #endregion

        #region Global Variables 
        private System.IO.EnumerationOptions EOptions = new System.IO.EnumerationOptions() 
        { 
            ReturnSpecialDirectories = false, 
            IgnoreInaccessible = true, 
            RecurseSubdirectories = false 
        };
        //Boolean SubDirectoryScan = false;
        // Boolean PipeSpit = true;
        int ThreadCount = 1;
        string output = string.Empty;
        string Blacklist = "7z|bmp|doc|docx|jpg|m4v|mov|mp3|mp4|pdf|png|tmp|xls|xlsx";
        string Whitelist = "*";
        Boolean LineNumbers = false;
        string keyword = string.Empty;
        string Path = string.Empty;
        System.IO.SearchOption ScanType = System.IO.SearchOption.TopDirectoryOnly;
        Boolean DisplayHelp = false;
        HashSet<string> BlackListArray = new HashSet<string>();
        #endregion

        public Scanner(string arg){DisectArgs(arg);}

        public string Scan()
        {
            if (DisplayHelp) { return NATS.Properties.Resources.Help; }

            if (ThreadCount == 0) { ThreadCount = 4; }
            StringBuilder sb = new StringBuilder();
            sb.Append(keyword).Append(" - ").Append(Path).Append(Environment.NewLine);
            sb.Append("-----------------------------------------------------------------").Append(Environment.NewLine);
            if (LineNumbers)
            {
                if (ThreadCount == 1)
                { sb.Append(SingleThreadWithLinesScan()); }
                else { sb.Append(MultiThreadWithLinesScan()); }
            }
            else
            {
                if (ThreadCount == 1)
                { sb.Append(SingleThreadScan()); }
                else { sb.Append(MultithreadScan()); }
            }


            sb.Append(Environment.NewLine).Append("- NATS Complete.");
            if (!string.IsNullOrEmpty(output))
            {
                System.IO.File.WriteAllText(output, sb.ToString());
                return "- NATS Complete.";
            }
            else { return sb.ToString(); }
        }

        #region Functions and Methods

        private void CompileBlackList(string BlackList)
        {
            if (Whitelist == "*")
            {
                foreach (string item in Blacklist.Split('|'))
                {
                    string Ext = item.Replace("|", string.Empty);
                    if (!BlackListArray.Contains(Ext)) { BlackListArray.Add(Ext); }
                }
            }
        }

        private void DisectArgs(string arg)
        {
            string[] Items = arg.Split('-');
            foreach (string Item in Items)
            {
                if (!string.IsNullOrEmpty(Item))
                {
                    string Keyitem = Item.Substring(0, 1).ToUpper();
                    string itemVal = Item.Substring(1).Trim();
                    switch (Keyitem)
                    {
                        case "P": Path = itemVal; break;
                        case "K": keyword = itemVal; break;
                        case "T": int.TryParse(itemVal, out ThreadCount); break;
                        case "B": Blacklist = itemVal; break;
                        case "W": Whitelist = itemVal; break;
                        case "L": LineNumbers = true; break;
                        case "S": EOptions.RecurseSubdirectories = true; break;//ScanType = System.IO.SearchOption.AllDirectories; break;
                        case "H": DisplayHelp = true; break;
                    }
                }

            }
            if (String.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(Path)) { DisplayHelp = true; }
            CompileBlackList(Blacklist);
        }



        #region MultiLineOutput functions
        private string SingleThreadWithLinesScan()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string S in Files)
            {
                string Item = CheckFileWithLines(S);
                if (!string.IsNullOrEmpty(Item)) { sb.Append(Item).Append(Environment.NewLine); }
            }
            return sb.ToString();
        }

        private string MultiThreadWithLinesScan()
        {
            System.Collections.Concurrent.ConcurrentBag<string> ReturnItems = new System.Collections.Concurrent.ConcurrentBag<string>();
            System.Threading.Tasks.ParallelOptions Options = new ParallelOptions() { MaxDegreeOfParallelism = ThreadCount };


            Parallel.ForEach(Files, Options, (currentFile) =>
            {
                string Item = CheckFileWithLines(currentFile);
                if (!string.IsNullOrEmpty(Item)) { ReturnItems.Add(Item); }
            });

            StringBuilder sb = new StringBuilder();
            foreach (string S in ReturnItems) { sb.Append(S).Append(Environment.NewLine); }
            return sb.ToString();
        }

        private string CheckFileWithLines(string FilePath)
        {

            Dictionary<Int64, string> values = new Dictionary<long, string>();
            if (!Blacklist.Contains(System.IO.Path.GetExtension(FilePath)))
            {
                Int64 Counter = 1;
                foreach (string line in System.IO.File.ReadLines(FilePath))
                {
                    if (line.IndexOf(keyword) > -1) { values.Add(Counter, FilePath); }
                    Counter++;
                }
            }
            System.Text.StringBuilder SB = new StringBuilder();
            foreach (var Item in values) { SB.Append(Item.Key.ToString()).Append("    ").Append(Item.Value).Append(Environment.NewLine); }
            return SB.ToString();

        }

        #endregion

        #region Single Line Functions

        private string SingleThreadScan()
        {
            List<string> ReturnItems = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (string S in Files)
            {
                string Item = CheckFile(S);
                if (!string.IsNullOrEmpty(Item)) { sb.Append(Item).Append(Environment.NewLine); }
            }
            return sb.ToString();

        }
        private string MultithreadScan()
        {
            System.Collections.Concurrent.ConcurrentBag<string> ReturnItems = new System.Collections.Concurrent.ConcurrentBag<string>();
            System.Threading.Tasks.ParallelOptions Options = new ParallelOptions() { MaxDegreeOfParallelism = ThreadCount };


            Parallel.ForEach(Files, Options, (currentFile) =>
            {
                string Item = CheckFile(currentFile);
                if (!string.IsNullOrEmpty(Item)) { ReturnItems.Add(Item); }
            });

            StringBuilder sb = new StringBuilder();
            foreach (string S in ReturnItems) { sb.Append(S).Append(Environment.NewLine); }
            return sb.ToString();
        }

        private string CheckFile(string FilePath)
        {
            string Ext = System.IO.Path.GetExtension(FilePath);
            if (Ext.Length > 0) { Ext = Ext.Substring(1); }
            if (!Blacklist.Contains(Ext))
            {

                foreach (string line in System.IO.File.ReadLines(FilePath))
                {
                    if (line.IndexOf(keyword) > -1) { return FilePath; }
                }
            }

            return string.Empty;
        }

        #endregion

        #endregion
    }
}
