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
}