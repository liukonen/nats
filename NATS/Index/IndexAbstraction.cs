using System;
using System.Collections.Generic;
using System.Linq;


namespace NATS.Index
{

    public struct FileObjects
    {
        public long FileIndex;
        public DateTime LastModified;
        public FileObjects(long fileIndex, DateTime lastModified)
        {
            FileIndex = fileIndex; LastModified = lastModified;
        }
    }

    public class FileAbstraction
    {
        #region Global Var
        Dictionary<string, FileObjects> Collection = new Dictionary<string, FileObjects>();
        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        string searchDictionary;
        System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> ItemsToInsert = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();
        System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> ItemsToUpdate = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

        //System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>> LookupItem = new System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>>();
        System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>> ItemsToAdd = new System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>>();
        System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>> ItemsToRemove = new System.Collections.Concurrent.ConcurrentDictionary<string, List<KeywordObject>>();


        #endregion

        #region Public Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SearchDirectory"></param>
        public FileAbstraction(string SearchDirectory)
        {
            searchDictionary = SearchDirectory;
            RefreshCollection();
        }

        private void RefreshCollection()
        {
            Collection = Access.SelectFiles(searchDictionary);
        }

        /// <summary>
        /// Checks to See if the file needs updating since last scaned
        /// </summary>
        /// <param name="FileName">FileName to Check</param>
        /// <param name="LastUpdated">Files Last Modified date</param>
        /// <returns></returns>
        public bool NeedsUpdating(string FileName, DateTime LastUpdated)
        {

            if (Collection.ContainsKey(FileName))
            {

                if (Collection[FileName].LastModified >= DateTimeWithoutMiliseconds(LastUpdated)) { return false; }
                else { ItemsToUpdate.TryAdd(FileName, LastUpdated); }
            }
            else
            {
                ItemsToInsert.TryAdd(FileName, LastUpdated);

            }
            return true;
        }


        public void AddLookups(string fileName, List<KeywordObject> KeywordIndexes)
        {
            List<KeywordObject> existing = Access.GetExistingSavedItems(fileName);
            List<KeywordObject> toAdd = new List<KeywordObject>();
            List<KeywordObject> toRemove = new List<KeywordObject>();

            toRemove.AddRange(from KeywordObject A in existing where !KeywordIndexes.Contains(A) select A);
            toAdd.AddRange(from KeywordObject a in KeywordIndexes where !existing.Contains(a) select a);
            if (toAdd.Count > 0) { ItemsToAdd.TryAdd(fileName, toAdd); }
            if (toRemove.Count> 0){ ItemsToRemove.TryAdd(fileName, toRemove); }

        }

        public void SaveToDatabase()
        {

            Access.BulkFileInsert(ItemsToInsert.ToArray());
            Access.BulkFileUpdate(ItemsToUpdate.ToArray());
            RefreshCollection();
            Access.BulkDeleteLookups(GenerateLookups(ItemsToRemove.ToArray()));
            Access.BuldInsertLookups(GenerateLookups(ItemsToAdd.ToArray()));
        }

        public List<Lookup> GenerateLookups(KeyValuePair<string, List<KeywordObject>>[] Items)
        {
            List<Lookup> response = new List<Lookup>();
            foreach (KeyValuePair<string, List<KeywordObject>> item in Items)
            {
                long FileIndex = Collection[item.Key].FileIndex;

                foreach (KeywordObject keywordItem in item.Value)
                {
                    response.Add(new Lookup(keywordItem.Hash, keywordItem.Collision, FileIndex));
                }
            }
            return response;
        }



        /// <summary>
        /// Returns the File Index Value used in the Database
        /// </summary>
        /// <param name="FileName">FileName to retreive</param>
        /// <returns></returns>
        public Int64 FileIndex(string FileName)
        {
            return Collection[FileName].FileIndex;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Removes any Milliseconds from a DateTime, which helps return stronger results in date compare
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DateTime DateTimeWithoutMiliseconds(DateTime item)
        {
            return new DateTime(item.Year, item.Month, item.Day, item.Hour, item.Minute, item.Second);
        }
        #endregion
    }
}
