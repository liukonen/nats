using System;
using System.Collections.Generic;
using System.Linq;

namespace NATS.Index
{
    public class FileAbstraction
    {
        Dictionary<string, Tuple<Int64, DateTime>> Collection = new Dictionary<string, Tuple<long, DateTime>>();

        //FileIndex
        //FileName
        //LastUpdated
        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        public FileAbstraction(string SearchDirectory)
        {
            List<Tuple<Int64, string, DateTime>> Existing = Access.SelectFiles(SearchDirectory);
            foreach (var record in Existing)
            {
                Collection.Add(record.Item2, new Tuple<long, DateTime>(record.Item1, record.Item3));
            }        
        }

        public bool NeedsUpdating(string FileName, DateTime LastUpdated)
        {

            if (Collection.ContainsKey(FileName))
            {
                if (Collection[FileName].Item2 >= DateTimeWithoutMiliseconds(LastUpdated)) { return false; }
            }
            else
            {
                DateTime MinDate = new DateTime(1970, 1, 1);
                Access.InsertFileRecord(FileName, MinDate);
                Int64 Index = Access.LookupFileIndex(FileName);
                Collection.Add(FileName, new Tuple<long, DateTime>(Index, MinDate));

            }//Create a new record right away
            return true;
        }

        public void UpdateLastModified(string FileName, DateTime LastModifed)
        {
            Access.UpdateFileRecord(FileName, DateTimeWithoutMiliseconds(LastModifed));
        }

        public DateTime LastModified(string FileName)
        { 
        if (Collection.ContainsKey(FileName)) { return Collection[FileName].Item2; }
            return DateTime.MinValue;
        }

        public Int64 FileIndex(string FileName)
        { 
         return Collection[FileName].Item1;         
        }

        private DateTime DateTimeWithoutMiliseconds(DateTime item)
        {
            return new DateTime(item.Year, item.Month, item.Day, item.Hour, item.Minute, item.Second);
        }

    }

    public class KeywordAbstraction
    {
        //Handles both a high level Keyword as well as the Lookup Abstraction layer

        Dictionary<string, Tuple<long, long>> DBKeywords = new Dictionary<string, Tuple<long, long>>();
        HashSet<long> HashCodeLookup = new HashSet<long>();

        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        public KeywordAbstraction()
        {
            DBKeywords = Access.LookupAllKeywords();
            HashCodeLookup = new HashSet<long>((from Tuple<long, long> item in DBKeywords.Values select item.Item1).Distinct());
        }

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
                        long NewIndex = Access.GetNextCollission();
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

       

    }
         
}
