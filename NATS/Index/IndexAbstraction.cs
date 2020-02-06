using System;
using System.Collections.Generic;

namespace NATS.Index
{
    public class FileAbstraction
    {
        #region Global Var
        Dictionary<string, Tuple<Int64, DateTime>> Collection = new Dictionary<string, Tuple<long, DateTime>>();
        sqlLiteDataAccess Access = sqlLiteDataAccess.Instance();

        #endregion

        #region Public Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SearchDirectory"></param>
        public FileAbstraction(string SearchDirectory)
        {
            List<Tuple<Int64, string, DateTime>> Existing = Access.SelectFiles(SearchDirectory);
            foreach (var record in Existing)
            {
                Collection.Add(record.Item2, new Tuple<long, DateTime>(record.Item1, record.Item3));
            }        
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

        /// <summary>
        /// Updates Files Last Modified Date in the DB
        /// </summary>
        /// <param name="FileName">File To Update</param>
        /// <param name="LastModifed">Date To change to</param>
        public void UpdateLastModified(string FileName, DateTime LastModifed)
        {
            Access.UpdateFileRecord(FileName, DateTimeWithoutMiliseconds(LastModifed));
        }

        /// <summary>
        /// Returns the File Index Value used in the Database
        /// </summary>
        /// <param name="FileName">FileName to retreive</param>
        /// <returns></returns>
        public Int64 FileIndex(string FileName)
        { 
         return Collection[FileName].Item1;         
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
