using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Farmhash.Sharp;

namespace NATS.Index
{
    class SQLiteIndex : IDisposable
    {
        const string Space = " ";
        Dictionary<string, DateTime> _existingFiles = new Dictionary<string, DateTime>();

        //Key, Keyword, Value Hash
        Dictionary<string, Int64> KeywordsInDB = new Dictionary<string, Int64>();
        //HashSet<string> ActiveKeywords = new HashSet<string>();

        System.Collections.Concurrent.ConcurrentBag<string> ActiveKeywords = new System.Collections.Concurrent.ConcurrentBag<string>();



        public void Generate(string path)
        {

            EnumerationOptions Options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false
            };

            Filters.SmartSearchFilter Filter = new Filters.SmartSearchFilter();

            IEnumerable<FileInfo> Files = (new DirectoryInfo(path)).EnumerateFiles("*", Options);
            foreach (FileInfo item in Files)
            {
                ProcessFile(item, Filter);              

            }
            Console.WriteLine($"TOTALS - Parse time: {TotalFileProcess.ToString()}, LookupSave: {TotalLookupSave.ToString()}, Keyword Save: {TotalKeywordSave.ToString()}");
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

        private void ProcessFile(FileInfo item, Filters.SmartSearchFilter Filter)
        {
            if (item.Extension != ".db" && item.Extension != ".db-journal" && Filter.IsValid(item) && FileNeedsProcessing(item))
            {

                long LookupSave;
                long KeywordSave;
                long fileparse;
                Stopwatch.Start();
                Console.WriteLine($"Processing file {item.FullName}");
 
                System.Threading.Tasks.Parallel.ForEach(File.ReadAllLines(item.FullName), (line) =>
 {
     ProcessString(line);
 });
                fileparse = Stopwatch.ElapsedMilliseconds;Stopwatch.Restart();

                var Keywords = SaveFileKeywordPair(item);
                LookupSave = Stopwatch.ElapsedMilliseconds; Stopwatch.Restart();
                UpdateKeywords(Keywords);
                KeywordSave = Stopwatch.ElapsedMilliseconds; Stopwatch.Stop();
                Console.WriteLine($"Parse time: {fileparse.ToString()}, LookupSave: {LookupSave.ToString()}, Keyword Save: {KeywordSave.ToString()}");
                TotalKeywordSave += KeywordSave;
                TotalFileProcess += fileparse;
                TotalLookupSave += LookupSave;
            }
        }

        private void UpdateKeywords(Dictionary<string, Int64> NewKeywords)
        {
            //Assume All PreExisting Keywords have run through the Process
            var NonExisting = from KeyValuePair<string, Int64> NewKeyword in NewKeywords where !KeywordsInDB.ContainsKey(NewKeyword.Key) select NewKeyword;
            Access.TryInsertKeywords(NonExisting);

            foreach (KeyValuePair<string, Int64> keyword in NonExisting)
            {

                KeywordsInDB.Add(keyword.Key, keyword.Value);
            }



        }

        private Dictionary<string, Int64> SaveFileKeywordPair(FileInfo item)
        {
            Int64 FileIndex = Access.LookupFileIndex(item.FullName);
            HashSet<Int64> ExistingKeywords = Access.GetExistingLookupValues(FileIndex);
            Dictionary<string, Int64> FileKeywords = GenerateKeywordDictionary();
            Access.MergeKeywordPairs(ExistingKeywords, FileKeywords.Values, FileIndex);
            return FileKeywords;
        
        }


        private Dictionary<string, Int64> GenerateKeywordDictionary()
        {
            Dictionary<string, Int64> response = new Dictionary<string, Int64>();
            foreach (string I in ActiveKeywords.Distinct())
            {
                if (KeywordsInDB.ContainsKey(I)) { response.Add(I, KeywordsInDB[I]); }
                else { response.Add(I, Convert.ToInt64(Farmhash.Sharp.Farmhash.Hash32(I))); }
            }
            return response;
        }


        private void ProcessString(string Item)
        {
            string[] keywords = Item.Split(Space);
            foreach (string keyword in keywords)
            {
                string CleanKeyword = keyword.ToLowerInvariant().Trim();
                if (CleanKeyword.Length < 2500 && !ActiveKeywords.Contains(CleanKeyword))//if its bigger, chances are its a binary we are going through by mistake
                {
                    ActiveKeywords.Add(CleanKeyword);
                }
            }
        }


        private sqlLiteDataAccess Access = new sqlLiteDataAccess();



        public SQLiteIndex()
        {

        }



        private Boolean FileNeedsProcessing(System.IO.FileInfo File)
        {
            if (ExistingFiles.ContainsKey(File.FullName))
            {
                if (ExistingFiles.GetValueOrDefault(File.FullName) == DateTimeWithoutMiliseconds(File.LastWriteTime))
                {
                    return false;
                }
                else { Access.UpdateFileRecord(File.FullName, File.LastWriteTime); }
            }
            else { Access.InsertFileRecord(File.FullName, File.LastWriteTime); }
            return true;
        }


        private DateTime DateTimeWithoutMiliseconds(DateTime item)
        {
            return new DateTime(item.Year, item.Month, item.Day, item.Hour, item.Minute, item.Second);
        }



        private Dictionary<string, DateTime> ExistingFiles
        {
            get { if (_existingFiles.Count == 0) { _existingFiles = Access.LookupExistingFiles(); } return _existingFiles; }
        }


        public void close()
        {
            Access.Dispose();
            _existingFiles.Clear();
            ActiveKeywords.Clear();
            KeywordsInDB.Clear();
        }


     

        void IDisposable.Dispose()
        {
            Access.Dispose();
            _existingFiles.Clear();
            ActiveKeywords.Clear();
            KeywordsInDB.Clear();
        }
    }
}
