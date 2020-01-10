using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NATS
{
    class Scanner
    {
        #region Constants
        const long TenMB = 1048576;
        const int Sector32 = 4096;

        #endregion
        #region properties
        private IEnumerable<System.IO.FileInfo> Files { get 
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                return di.EnumerateFiles(SearchParam, EOptions);
                 }
        }
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
        string Blacklist = "7z|bmp|doc|docx|dll|exe|jpg|m4v|mov|mp3|mp4|pdb|pdf|png|tmp|xls|xlsx";
        string SearchParam = "*";
        Boolean LineNumbers = false;
        string keyword = string.Empty;
        string Path = string.Empty;
        Boolean DisplayHelp = false;
        Boolean LoadToRam = false;
        Boolean SmartSearch = false;
        HashSet<string> BlackListArray = new HashSet<string>();
        #endregion

        public Scanner(string arg){DisectArgs(arg);}

        public string Scan()
        {
            if (DisplayHelp) { return NATS.Properties.Resources.Help; }
            knownEncodings = GetEncodings();
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
            if (SearchParam == "*")
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
                        case "W": SearchParam = itemVal; break;
                        case "L": LineNumbers = true; break;
                        case "S": EOptions.RecurseSubdirectories = true; break;//ScanType = System.IO.SearchOption.AllDirectories; break;
                        case "H": DisplayHelp = true; break;
                        case "R": LoadToRam = true; break;
                        case "M": SmartSearch = true; break;
                        case "O": output = itemVal; break;
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
            foreach (FileInfo S in Files)
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

        private string CheckFileWithLines(FileInfo FilePath)
        {
            long fileLength = FilePath.Length;
            int CheckSize = (fileLength > Sector32) ? Sector32 : (int)fileLength;
            Dictionary<Int64, string> values = new Dictionary<long, string>();
            if (!Blacklist.Contains(FilePath.Extension) && fileLength > 0 && IsText(FilePath, CheckSize))
            {
                //if ()
                Int64 Counter = 1;
                foreach (string line in System.IO.File.ReadLines(FilePath.FullName))
                {
                    if (line.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1) { values.Add(Counter, FilePath.FullName); }
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
            foreach (FileInfo S in Files)
            {
                string Item = CheckFile(S);
                if (!string.IsNullOrEmpty(Item)) { sb.Append(Item).Append(Environment.NewLine); }
            }
            System.IO.File.WriteAllText("d:\\NonFiles.txt", String.Join(Environment.NewLine, NonText.ToArray()));
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
            System.IO.File.WriteAllText("d:\\NonFiles.txt", String.Join(Environment.NewLine, NonText.ToArray()));
            StringBuilder sb = new StringBuilder();
            foreach (string S in ReturnItems) { sb.Append(S).Append(Environment.NewLine); }
            return sb.ToString();
        }

        System.Collections.Concurrent.ConcurrentBag<string> NonText = new System.Collections.Concurrent.ConcurrentBag<string>();
        private string CheckFile(FileInfo FilePath)
        {
            //List<string> NonText = new List<string>();

            string Ext = FilePath.Extension;
            if (Ext.Length > 0) { Ext = Ext.Substring(1); }
            if (!Blacklist.Contains(Ext, StringComparison.OrdinalIgnoreCase))
            {
                long fileLength = FilePath.Length;
                int CheckSize = (fileLength > Sector32) ? Sector32 : (int)fileLength;

                try
                {
                    if (fileLength > 0 && IsText(FilePath, CheckSize))
                    {

                        if (LoadToRam && TenMB > FilePath.Length)
                        {
                            using (StreamReader FS = FilePath.OpenText())
                            {
                                string line = FS.ReadToEnd();
                                if (line.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1) { return FilePath.FullName; }
                            }
                        }
                        else {
                            using (StreamReader FS = FilePath.OpenText())
                            {
                                string line = String.Empty;
                                while ((line = FS.ReadLine()) != null)
                                {
                                    if (line.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1) { return FilePath.FullName; }
                                }
                            }
                        }
                    }
                }
                catch  { }
            }

            return string.Empty;
        }

        #endregion

        #region Text Checker Logic


        private List<byte[]> knownEncodings;

        private List<byte[]> GetEncodings()
        {
            var X = System.Text.Encoding.GetEncodings();
            List<byte[]> EncodePre = new List<byte[]>();
            
            foreach (var Y in X)
            {
                byte[] pre = Y.GetEncoding().GetPreamble();
                if (pre.Length > 0) { EncodePre.Add(pre); }
            }
            return EncodePre;

        }

        private Boolean IsText(FileInfo File, int size)
        {           
            if (!SmartSearch) { return true; }
            byte[] bytes = new byte[size];
            using (FileStream fs = File.OpenRead())
            {

                fs.Read(bytes, 0, size);
                fs.Close();
            }
            return DataStartsWithBom(bytes, knownEncodings);
       }

        private bool DataStartsWithBom(byte[] data, List<byte[]> knownEncodings)
        {
            foreach (byte[] bom in knownEncodings)
            {
                if (data.Zip(bom, (x, y) => x == y).All(x => x)) { return true; }
            }
            return false;
        }


        #endregion
        #endregion
    }
}
