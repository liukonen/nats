using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Farmhash.Sharp;
using System.Threading.Tasks;

namespace NATS.Index
{
    class SQLiteIndex : IDisposable
    {
        Dictionary<string, DateTime> _existingFiles = new Dictionary<string, DateTime>();


        Dictionary<string, Int64> KeywordsInDB = new Dictionary<string, Int64>();
        
        public string[] Inquire(string SearchString, string DirectoryPath)
        {
            string[] keywords = splitKeywords(SearchString);

            List<string> items = new List<string>();
            Boolean HasRunOnce = false;

            foreach (string keyword in keywords)
            {
                if (!HasRunOnce)
                {
                    HasRunOnce = true;
                    items.AddRange(Access.InquireOnDB(keyword, DirectoryPath));
                }
                else 
                {
                    string[] ItemsToMergeOn = Access.InquireOnDB(keyword, DirectoryPath);
                    var StillValid = from string MergeItem in ItemsToMergeOn where items.Contains(MergeItem) select MergeItem;
                    List<string> ReplaceList = new List<string>(StillValid);
                    items = ReplaceList;
                }

            }
            if (keywords.Count() > 1) {

                List<string> ValidatedResponse = new List<string>();
               foreach (var item in items)
                {
                    string TestString = File.ReadAllText(item);
                    if (TestString.Contains(SearchString)){ ValidatedResponse.Add(item); }
                }
                return ValidatedResponse.ToArray();
            }
            else { return items.ToArray(); }

        }

        private void Debug(string item, Boolean active)
        { 
        if (active) { Console.WriteLine(item); }
        }

        public void Generate(string path, Boolean debug)
        {
            Stopwatch.Start();
            long LookupSave, fileparse;

            FileAbstraction FilesAb = new FileAbstraction(path);
            KeywordAbstraction KeywordsAb = new KeywordAbstraction();
            fileparse = Stopwatch.ElapsedMilliseconds; Stopwatch.Restart();
            Debug("Load Time Abstraction: " + fileparse.ToString(), debug);


            EnumerationOptions Options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };

            Filters.SmartSearchFilter Filter = new Filters.SmartSearchFilter();

            IEnumerable<FileInfo> Files = (new DirectoryInfo(path)).EnumerateFiles("*", Options);
            foreach (FileInfo item in Files)
            {
                if (StaticDynamicReadFile(item, Filter) && FilesAb.NeedsUpdating(item.FullName, item.LastWriteTime))
                {
                    Debug("Processing: " + item.FullName, debug);
                    Stopwatch.Restart();

                    System.Collections.Concurrent.BlockingCollection<string> tsKeywords = new System.Collections.Concurrent.BlockingCollection<string>();
                    Parallel.ForEach(File.ReadAllLines(item.FullName), (line) => {
                        string[] keywords = ProcessString(line);
                        foreach( string keyword in keywords) { if (!tsKeywords.Contains(keyword)){ tsKeywords.Add(keyword); } }
                    });
                    fileparse = Stopwatch.ElapsedMilliseconds; Stopwatch.Restart();
                    KeywordsAb.LoadFileKeywords(tsKeywords.ToArray(), FilesAb.FileIndex(item.FullName));
                    FilesAb.UpdateLastModified(item.FullName, item.LastWriteTime);
                    LookupSave = Stopwatch.ElapsedMilliseconds;
                    Debug($"Process Time: {fileparse.ToString()}ms Database time: {LookupSave.ToString()}ms", debug);
                }
                

               

            }
            Debug($"TOTALS - Parse time: {TotalFileProcess.ToString()}, LookupSave: {TotalLookupSave.ToString()}, Keyword Save: {TotalKeywordSave.ToString()}", debug);
            Stopwatch.Stop();
            //For each file in enumerate
            //if FileNeedsProccesing then
            //Dictionary  = GenerateIndexes
            //ProcessDictionary
            //next
        }
        long TotalLookupSave;
        long TotalKeywordSave;
        long TotalFileProcess;
        System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        private bool StaticDynamicReadFile(FileInfo item, Filters.SmartSearchFilter filter)
        {
            string[] BlacklistFilter = (from string str in NATS.Properties.Resources.DefaultBlacklist.Split('|') select "." + str).ToArray();
            string[] WhiteListFilter = { ".cs", ".vb", ".txt", ".csproj", ".vbproj", ".html", ".aspx", ".ascx", ".js" };
            string Ext = item.Extension.ToLower();
            if (BlacklistFilter.Contains(Ext)) { return false; }
            if (WhiteListFilter.Contains(Ext)) { return true; }
            return filter.IsValid(item);        
        }



        private string[] splitKeywords(string Item)
        {
            return Item.Split(new Char[] { ',', '\\', '/', '<', '>', ':', '.', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        }



        private string[] ProcessString(string Item)
        {
            List<string> Items = new List<string>();
            string[] keywords = splitKeywords(Item);
            foreach (string keyword in keywords)
            {
                string CleanKeyword = keyword.ToLowerInvariant().Trim();
                if (CleanKeyword.Length < 1000)//if its bigger, chances are its a binary we are going through by mistake
                {
                    Items.Add(CleanKeyword);
                }
            }
            return Items.ToArray();
        }


        private sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();



        public SQLiteIndex()
        {

        }


        public void close()
        {
            Access.Dispose();
            _existingFiles.Clear();
            KeywordsInDB.Clear();
            
        }


     

        void IDisposable.Dispose()
        {
            Access.Dispose();
            _existingFiles.Clear();
            KeywordsInDB.Clear();
        }
    }
}
