using System;
using System.Collections.Generic;
using System.Linq;


namespace NATS.Index
{
    
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
          
    public class KeywordCache
    {
        private static KeywordCache _instance = new KeywordCache();
        Dictionary<string, KeywordObject> existingKeywords = new Dictionary<string, KeywordObject>();
        Dictionary<string, KeywordObject> NewKeywords = new Dictionary<string, KeywordObject>();

        private long CurrentCollisionIndex = 0;
        HashSet<long> ExistingKeywordHash = new HashSet<long>();


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
            existingKeywords = Access.LookupAllKeywords();
            ExistingKeywordHash = (from KeywordObject o in existingKeywords.Values select o.Hash).Distinct().ToHashSet();
            CurrentCollisionIndex = Access.GetCurrentCollision();

        }

        public void SaveToDb()
        {
            Access.InsetKeywords(from KeyValuePair<string, KeywordObject> I in NewKeywords select new Tuple<string, long, long>(I.Key, I.Value.Hash, I.Value.Collision));
            Access.SaveCollision(CurrentCollisionIndex);
        }
    }
}