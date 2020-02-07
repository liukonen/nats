using System;
using System.Collections.Generic;
using System.Linq;


namespace NATS.Index
{
    /// <summary>
    /// Handles both a high level Keyword as well as the Lookup Abstraction layer
    /// </summary>
    public class KeywordAbstraction
    {
        #region Global Vars
        Dictionary<string, Tuple<long, long>> DBKeywords = new Dictionary<string, Tuple<long, long>>();
        HashSet<long> HashCodeLookup = new HashSet<long>();
        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();
        #endregion

        #region Public Properties

        /// <summary>
        /// Constructor
        /// </summary>
        public KeywordAbstraction()
        {
            DBKeywords = Access.LookupAllKeywords();
            HashCodeLookup = new HashSet<long>((from Tuple<long, long> item in DBKeywords.Values select item.Item1).Distinct());
        }

        /// <summary>
        /// Saves the Keywords to the Lookup Table of the DB for a perticular File Index
        /// </summary>
        /// <param name="keywords">List of Keyword strings to save or update</param>
        /// <param name="FileIndex">File Index Value (returned from FileAbstraction.FileIndex)</param>
        public void LoadFileKeywords(IEnumerable<string> keywords, long FileIndex)
        {
            //LookupSaveRecords(keyword, collission, File)
            List<Tuple<long, long>> LookupSaveRecords = new List<Tuple<long, long>>();
            List<Tuple<string, long, long>> KeywordSaveRecords = new List<Tuple<string, long, long>>();
            foreach (var item in keywords.Distinct())
            {
                if (DBKeywords.ContainsKey(item))
                {
                    Tuple<long, long> FoundItem = DBKeywords[item];
                    LookupSaveRecords.Add(new Tuple<long, long>(FoundItem.Item1, FoundItem.Item2));
                }
                else
                {
                    long HashItem = Farmhash.Sharp.Farmhash.Hash32(item);
                    if (HashCodeLookup.Contains(HashItem))//we have a collission
                    {
                        long NewIndex = Access.Collision();
                        KeywordSaveRecords.Add(new Tuple<string, long, long>(item, HashItem, NewIndex));
                        LookupSaveRecords.Add(new Tuple<long, long>(HashItem, NewIndex));
                    }
                    else
                    {
                        KeywordSaveRecords.Add(new Tuple<string, long, long>(item, HashItem, 0));
                        LookupSaveRecords.Add(new Tuple<long, long>(HashItem, 0));
                        HashCodeLookup.Add(HashItem);
                    }
                }
            }
            Access.InsetKeywords(KeywordSaveRecords);

            foreach (var item in KeywordSaveRecords)
            {
                DBKeywords.Add(item.Item1, new Tuple<long, long>(item.Item2, item.Item3));
                if (!HashCodeLookup.Contains(item.Item2)) { HashCodeLookup.Add(item.Item2); }
            }

            List<Tuple<long, long>> ExistingFileRecords = Access.GetExistingLookupValues(FileIndex);

            var ItemsToAdd = from Tuple<long, long> Item in LookupSaveRecords where !ExistingFileRecords.Contains(Item) select Item;
            var ItemsToRemove = from Tuple<long, long> Item in ExistingFileRecords where !LookupSaveRecords.Contains(Item) select Item;
            Access.InsertLookups(ItemsToAdd, FileIndex);
            Access.DeleteLookups(ItemsToRemove, FileIndex);
        }

        #endregion
    }




 
    public struct Lookup
    {
        public long FileIndex;
        public long KeywordHash;
        public long CollisionIndex;
        public Lookup(long keywordHash, long collisionIndex, long fileIndex) { KeywordHash = keywordHash; CollisionIndex = collisionIndex; FileIndex = fileIndex; }
    }

    public struct KeywordObject
    {

        public long Hash;
        public long Collision;
        public KeywordObject(long hash, long collision) { Hash = hash; Collision = collision; }
    }


    public class IndexFiles
    {

        private IEnumerable<System.IO.FileInfo> _FilesToProcess;
        private string _directoryScanned;
        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        Dictionary<string, FileObjects> Collection = new Dictionary<string, FileObjects>();



        public IndexFiles(IEnumerable<System.IO.FileInfo> filesToProcess, string directoryScanned)
        {
            _FilesToProcess = filesToProcess;
            RefreshCollection();
        }

        public void ProcessFiles()
        { }

        public void SaveFiles()
        { }


        private void RefreshCollection()
        {
            List<Tuple<long, string, DateTime>> Existing = Access.SelectFiles(_directoryScanned);
            foreach (Tuple<long, string, DateTime> record in Existing)
            {
                Collection.Add(record.Item2, new FileObjects(record.Item1, record.Item3));
            }
        }

        /*
         * list<fa2> items
         * for each File
         FA2 = new FA2(file);
         if (FA2.NeedsUpdating){FA2.Populate;}
         next

         */


    }

    public class KeywordCache
    {
        private static KeywordCache _instance = new KeywordCache();
        Dictionary<string, KeywordObject> existingKeywords = new Dictionary<string, KeywordObject>();
        Dictionary<string, KeywordObject> NewKeywords = new Dictionary<string, KeywordObject>();

        private long CurrentCollisionIndex = 0;
        HashSet<long> ExistingKeywordHash = new HashSet<long>();

        // ConcurrentDictionary<string, long> KeywordsToAdd = new ConcurrentDictionary<string, long>();

        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

       
        public static KeywordCache Instance()
        {
            if (_instance == null) { _instance = new KeywordCache(); }
            return _instance;
        }

        protected KeywordCache()
        {
            LoadCache();
        }

        private readonly object key = new object();

        public List<KeywordObject> AddKeywords(IEnumerable<string> keywords)
        {
            List<KeywordObject> response = new List<KeywordObject>();
            lock (key)
            {
                foreach (string keyword in keywords)
                {
                    if (existingKeywords.ContainsKey(keyword))
                    {
                        response.Add(existingKeywords[keyword]);
                    }
                    else if (NewKeywords.ContainsKey(keyword))
                    {
                        response.Add(NewKeywords[keyword]);
                    }
                    else
                    {
                        long Col = 0;
                        long Hash = Farmhash.Sharp.Farmhash.Hash32(keyword);
                        if (ExistingKeywordHash.Contains(Hash))
                        {
                            Col = CurrentCollisionIndex;
                            CurrentCollisionIndex += 1;
                        }
                        else { ExistingKeywordHash.Add(Hash); }
                        KeywordObject Item = new KeywordObject(Hash, Col);
                        NewKeywords.Add(keyword, Item);
                        response.Add(Item);
                    }
                }
            }
            return response;

        }


        private void LoadCache()
        {
            Dictionary<string, Tuple<long, long>> DBKeywords = Access.LookupAllKeywords();
            foreach (var item in DBKeywords)
            {
                existingKeywords.Add(item.Key, new KeywordObject(item.Value.Item1, item.Value.Item2));
                if (!ExistingKeywordHash.Contains(item.Value.Item1)) { ExistingKeywordHash.Add(item.Value.Item1); }

            }
            CurrentCollisionIndex = Access.GetCurrentCollision();

        }

        public void SaveToDb()
        {
            Access.InsetKeywords(from KeyValuePair<string, KeywordObject> I in NewKeywords select new Tuple<string, long, long>(I.Key, I.Value.Hash, I.Value.Collision));
            Access.SaveCollision(CurrentCollisionIndex);
        }
    }
}