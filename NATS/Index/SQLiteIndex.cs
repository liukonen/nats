﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace NATS.Index
{
    class SQLiteIndex : IDisposable
    {
        #region Global Vars

        Dictionary<string, DateTime> _existingFiles = new Dictionary<string, DateTime>();
        Dictionary<string, Int64> KeywordsInDB = new Dictionary<string, Int64>();
        long TotalLookupSave;
        long TotalFileProcess;
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
            Boolean HasRunOnce = false;

            foreach (string keyword in keywords)
            {
                if (!HasRunOnce)
                {
                    HasRunOnce = true;
                    items = Access.InquireOnDB(keyword, DirectoryPath);
                }
                else
                {
                    List<string> ItemsToMergeOn = Access.InquireOnDB(keyword, DirectoryPath);
                    List<string> StillValid = (from string MergeItem in ItemsToMergeOn where items.Contains(MergeItem) select MergeItem).ToList();
                    items = StillValid;
                }

            }
            if (keywords.Count() > 1)
            {

                List<string> ValidatedResponse = new List<string>();
                foreach (var item in items)
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

                    List<string> Keywords = ProcessString(File.ReadAllText(item.FullName));
                    fileparse = Stopwatch.ElapsedMilliseconds; Stopwatch.Restart();
                    KeywordsAb.LoadFileKeywords(Keywords, FilesAb.FileIndex(item.FullName));
                    FilesAb.UpdateLastModified(item.FullName, item.LastWriteTime);
                    LookupSave = Stopwatch.ElapsedMilliseconds;
                    Debug($"Process Time: {fileparse.ToString()}ms Database time: {LookupSave.ToString()}ms", debug);
                    TotalFileProcess += fileparse; TotalLookupSave += LookupSave;
                }
            }
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
        private string[] splitKeywords(string Item)
        {
            return Item.Split(new Char[] { ',', '\\', '/', '<', '>', ':', '.', ';', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        }


        /// <summary>
        /// processes a file string into keywords
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private List<string> ProcessString(string Item)
        {
            List<string> response = new List<string>();
            string[] keywords = splitKeywords(Item);


           // return (from String S in keywords where S.Length < 1000 select S.Trim().ToLowerInvariant()).Distinct().ToArray();
            foreach (string keyword in keywords)
            {
                string CleanKeyword = keyword.ToLowerInvariant().Trim();
                if (CleanKeyword.Length < 1000 && !Item.Contains(CleanKeyword))//if its bigger, chances are its a binary we are going through by mistake
                {
                    response.Add(CleanKeyword);
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
