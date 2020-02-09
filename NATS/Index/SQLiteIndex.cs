using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NATS.Index
{

    class SQLiteIndex : IDisposable
    {
        #region Global Vars

        Dictionary<string, DateTime> _existingFiles = new Dictionary<string, DateTime>();
        Dictionary<string, Int64> KeywordsInDB = new Dictionary<string, Int64>();
        Stopwatch Stopwatch = new Stopwatch();
        private sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        #endregion

        #region Public Functions

        /// <summary>
        /// Constructor
        /// </summary>
        public SQLiteIndex()
        {

        }

        /// <summary>
        /// Scans Index for files, and returns files matching Keyphrase and Directory Path
        /// </summary>
        /// <param name="SearchString">Keyword Phrase to search on</param>
        /// <param name="DirectoryPath">Directory to search through</param>
        /// <returns>An Array of File locations that are valid based on Search and Directory Path</returns>
        /// <remarks>Splits search into keywords, searchs on every keyword, then validates phrase in the end</remarks>
        public List<string> Inquire(string SearchString, string DirectoryPath)
        {
            string[] keywords = splitKeywords(SearchString);

            List<string> items = new List<string>();
   



            items = Access.InquireOnDb(keywords, DirectoryPath);
            if (keywords.Count() > 1)
            {

                List<string> ValidatedResponse = new List<string>();
                foreach (string item in items)
                {
                    string TestString = File.ReadAllText(item);
                    if (TestString.Contains(SearchString)) { ValidatedResponse.Add(item); }
                }
                return ValidatedResponse;
            }
            else { return items; }

        }

        /// <summary>
        /// Generates the New index based on Path
        /// </summary>
        /// <param name="path">Directory to search</param>
        /// <param name="debug">Display File Output</param>
        public void Generate(string path, Boolean debug)
        {
            Stopwatch.Start();
            long  fileparse, TotalLookupSave, TotalFileProcess;

            FileAbstraction FilesAb = new FileAbstraction(path);
            EnumerationOptions Options = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
            Filters.SmartSearchFilter Filter = new Filters.SmartSearchFilter();
            KeywordCache Kc = KeywordCache.Instance();
            IEnumerable<FileInfo> Files = (new DirectoryInfo(path)).EnumerateFiles("*", Options);
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = 4 };

            fileparse = Stopwatch.ElapsedMilliseconds; Stopwatch.Restart();
            Debug("Load Time Abstraction: " + fileparse.ToString(), debug);
            Parallel.ForEach(Files, options, (item) =>
            {
            if (StaticDynamicReadFile(item, Filter) && FilesAb.NeedsUpdating(item.FullName, item.LastWriteTime))
                {
                    Debug("Processing: " + item.FullName, debug);
                    HashSet<string> Keywords = new HashSet<string>();
                    if (1048576 > item.Length){ Keywords = ProcessString(File.ReadAllText(item.FullName)); }
                    else { Keywords = ProcessLargeString(item); }
                    List<KeywordObject> Items = Kc.AddKeywords(Keywords);
                    FilesAb.AddLookups(item.FullName, Items);
                }
            });
            TotalFileProcess = Stopwatch.ElapsedMilliseconds;
            Debug("Files Parsed. Saving...", debug);
            Stopwatch.Restart();
            Kc.SaveToDb();
            FilesAb.SaveToDatabase();
            TotalLookupSave = Stopwatch.ElapsedMilliseconds;
            Debug($"TOTALS - Parse time: {TotalFileProcess.ToString()}, DB Save: {TotalLookupSave.ToString()}", debug);
            Stopwatch.Stop();
        }

        /// <summary>
        /// Acts simmilar to Dispose, closing access to DB,and clearing in memory objects
        /// </summary>
        public void close()
        {
            Access.Dispose();
            _existingFiles.Clear();
            KeywordsInDB.Clear();

        }
        #endregion

        #region Private Properties

        /// <summary>
        /// Handles any internal Console Writelines in the event Active is set to true
        /// </summary>
        /// <param name="item">String to display</param>
        /// <param name="active">Do we display</param>
        /// <remarks>Used for index generation. No need to display on search, but nice to have if just generating index</remarks>
        private void Debug(string item, Boolean active)
        {
            if (active) { Console.WriteLine(item); }
        }

        /// <summary>
        /// Returns if the file should be scanned. First attempts a Black list and Whitelist based search (hard coded) followed by a smart search
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private bool StaticDynamicReadFile(FileInfo item, Filters.SmartSearchFilter filter)
        {
            string[] BlacklistFilter = (from string str in NATS.Properties.Resources.DefaultBlacklist.Split('|') select "." + str).ToArray();
            string[] WhiteListFilter = { ".cs", ".vb", ".txt", ".csproj", ".vbproj", ".html", ".aspx", ".ascx", ".js" };
            string Ext = item.Extension.ToLower();
            if (BlacklistFilter.Contains(Ext)) { return false; }
            if (WhiteListFilter.Contains(Ext)) { return true; }
            return filter.IsValid(item);
        }

        /// <summary>
        /// Splits Text based on hard coded Criteria
        /// </summary>
        /// <param name="Item">String to split</param>
        /// <returns>List of strings</returns>
        private static string[] splitKeywords(string Item)
        {
            return Item.Split(new Char[] { ',', '\\', '/', '<', '>', ':', '.', ';', ' ', '\r', '\n', '[', ']', '(', ')', '-', '?', '&', '|' }, StringSplitOptions.RemoveEmptyEntries);

        }


        /// <summary>
        /// processes a file string into keywords
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private static HashSet<string> ProcessString(string Item)
        {
            HashSet<string> response = new HashSet<string>();
            string[] keywords = splitKeywords(Item);
            // return (from String S in keywords where S.Length < 1000 select S.Trim().ToLowerInvariant()).Distinct().ToArray();
            foreach (string keyword in keywords)
            {
                string CleanKeyword = keyword.ToLowerInvariant().Trim();
                if (CleanKeyword.Length < 1000 && !response.Contains(CleanKeyword))//if its bigger, chances are its a binary we are going through by mistake
                {
                    response.Add(CleanKeyword);
                }
            }
            return response;
        }

        private static HashSet<string> ProcessLargeString(FileInfo file)
        {
            HashSet<string> response = new HashSet<string>();
            using (StreamReader FS = file.OpenText())
            {
                 string line = String.Empty;
                while ((line = FS.ReadLine()) != null)
                {
                    string[] keywords = splitKeywords(line);
                    foreach (string keyword in keywords)
                    {
                        string CleanKeyword = keyword.ToLowerInvariant().Trim();
                        if (CleanKeyword.Length < 1000 && !response.Contains(CleanKeyword))
                        {
                            response.Add(CleanKeyword);
                        }
                    }
                }
            }
            return response;
        }

        void IDisposable.Dispose()
        {
            Access.Dispose();
            _existingFiles.Clear();
            KeywordsInDB.Clear();
        }
        #endregion
    }
}
