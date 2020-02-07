using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Linq;

//Needs to be a singleton to help reduce any thread issues later on. (one and only one Data Access Control

namespace NATS.Index
{

    class sqlLiteDataAccess : IDisposable
    {

        #region Global Vars
        private SQLiteConnection connection = new SQLiteConnection();
        private static sqlLiteDataAccess _instance;
        #endregion

        #region Public Properties

        /// <summary>
        /// Get the Latest Instance of the class, or invokes a new instance if not created
        /// </summary>
        /// <returns></returns>
        public static sqlLiteDataAccess Instance()
        {
            if (_instance == null) { _instance = new sqlLiteDataAccess(); }
            return _instance;
        }

        /// <summary>
        /// Inquires on the Database, based on Keyword and Directory Path
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="directoryPath"></param>
        /// <returns>a list of FileNames that match the keyword and Directory</returns>
        public List<string> InquireOnDB(string keyword, string directoryPath)
        {
            List<string> response = new List<string>();
            string Proc = $"SELECT Files.FileName FROM Keywords INNER JOIN Lookup on Lookup.KeywordHash = Keywords.Hash AND Keywords.CollisionIndex = Lookup.CollisionIndex INNER JOIN Files ON Files.FileIndex = Lookup.FileIndex WHERE Files.FileName like'{directoryPath}%' AND Keywords.Text LIKE('%{keyword}%'); ";
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandText = Proc;
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add((string)reader[0]);
                    }
                }
            }
            return response;
        }

        public List<string> InquireOnDb(string[] keyword, string directoryPath)
        {
            string proc = "SELECT DISTINCT Files.FileName FROM Keywords INNER JOIN Lookup on Lookup.KeywordHash = Keywords.Hash AND Keywords.CollisionIndex = Lookup.CollisionIndex INNER JOIN Files ON Files.FileIndex = Lookup.FileIndex WHERE Files.FileName like'{0}%' AND Keywords.Text like ('%{1}%');";

            List<string> response = new List<string>();
            Boolean run = false;

            using (SQLiteTransaction Trans = connection.BeginTransaction())
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.Transaction = Trans;

                foreach (var item in keyword)
                {
                    List<string> specificFiles = new List<string>();
                    com.CommandText = string.Format(proc, directoryPath, item);

                    using (IDataReader reader = com.ExecuteReader()) { while (reader.Read()) { specificFiles.Add((string)reader[0]); } }
                    if (!run)
                    {
                        run = true; response = specificFiles;
                    }
                    else 
                    {
                        response = (from string S in specificFiles where response.Contains(S) select S).ToList();
                    }
                    if (response.Count == 0) { break; }

                }
             }
            return response;
        }

            /// <summary>
            /// Returns All Existing Keywords from the DB
            /// </summary>
            /// <returns>a Dictionary of <KeywordString, Tuple<Keywordhash / CollisionID></Keyword></returns>
            public Dictionary<string, Tuple<long, long>> LookupAllKeywords()
        {
            Dictionary<string, Tuple<long, long>> returnValue = new Dictionary<string, Tuple<long, long>>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = "Select Text, Hash, CollisionIndex from Keywords;";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returnValue.Add((string)reader[0], new Tuple<long, long>((long)reader[1], (long)reader[2]));
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// cleans up any connections if still open
        /// </summary>
        public void Dispose() { if (connection.State == ConnectionState.Open) connection.Close(); }

        /// <summary>
        /// Gets the Next Collision ID if NEEDED
        /// </summary>
        /// <returns>a new Collision ID. Collission ID right now is not specific to a given hash, but global</returns>
        public long Collision()
        {
            long Item = 0;
            using (SQLiteCommand com = connection.CreateCommand())
            using (SQLiteTransaction Trans = connection.BeginTransaction())
            {
                com.Transaction = Trans;
                com.CommandType = CommandType.Text;
                com.CommandText = "Select CurrentIndex FROM Collision;";
                Item = (long)com.ExecuteScalar();
            }
            ExecuteNonQuery($"UPDATE Collision Set CurrentIndex = {(Item + 1)}");
            return Item;
        }

        public long GetCurrentCollision()
        {
            long response = 0;
            using (SQLiteCommand com = connection.CreateCommand())
            using (SQLiteTransaction Trans = connection.BeginTransaction())
            {
                com.Transaction = Trans;
                com.CommandType = CommandType.Text;
                com.CommandText = "Select CurrentIndex FROM Collision;";
                response = (long)com.ExecuteScalar();
            }
            return response;
        }

        public void SaveCollision(long CurrentCollision)
        {
            ExecuteNonQuery($"Update Collision SET CurrentIndex = {CurrentCollision};");

        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// constructor
        /// </summary>
        protected sqlLiteDataAccess()
        {
            connection = new SQLiteConnection(Properties.Resources.InternalIndexConnection);
            connection.Open();
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Makes a string value safe to insert into SQLITE
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string SafeString(string item)
        {
            return item.Replace("'", "''");
        }

        /// <summary>
        /// Converts a .Net date time to a Unix based Date time
        /// </summary>
        /// <param name="item">Local DateTime</param>
        /// <returns>Unix Based DateTime in Number formate</returns>
        private long ToUnixDate(DateTime item)
        {
            DateTimeOffset item2 = item;
            return item2.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Converts a Unix Number DateTime into a .Net date time
        /// </summary>
        /// <param name="item">Unix Long DateTime (In Seconds)</param>
        /// <returns>Local DateTime</returns>
        private DateTime FromUnixDate(long item)
        {
            DateTimeOffset item2 = DateTimeOffset.FromUnixTimeSeconds(item);
            return item2.DateTime;
        }

        /// <summary>
        /// Provides easy write only processing to the SQLite DB
        /// </summary>
        /// <param name="command"></param>
        private void ExecuteNonQuery(string command)
        {
            using (var Transaction = connection.BeginTransaction())
            using (var com = connection.CreateCommand())
            {

                com.Transaction = Transaction;
                com.CommandText = command;
                com.ExecuteNonQuery();
                Transaction.Commit();

            }
        }

        #endregion

        #region Public File Table Specific Functions
        //Table Files  - FileIndex (INT) FileName (TEXT) LastUpdated (INT {converted to datetime})


        public void BulkFileInsert(KeyValuePair<string, DateTime>[] Items)
        {

            const string starter = "INSERT INTO Files (FileName, LastUpdated) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();
            foreach (KeyValuePair<string, DateTime> item in Items)
            {
                if (index == 0)
                {
                    ssb.Append(starter);
                }
                ssb.Append($"('{SafeString(item.Key)}', {ToUnixDate(item.Value)} ),");
                index += 1;
                if (index == 1000)
                {
                    index = 0;
                    int Length = ssb.Length;
                    ssb = new StringBuilder(ssb.ToString().Substring(0, Length - 1));
                    ssb.Append(";");
                }
            }
            string response = ssb.ToString();
            if (!string.IsNullOrWhiteSpace(response))
            {
                if (response.EndsWith(",")) { response = response.Substring(0, response.Length - 1); }
                if (!response.EndsWith(";")) { response = response + ";"; }
                ExecuteNonQuery(response);
            }


        }

        public void BulkFileUpdate(KeyValuePair<string, DateTime>[] Items)
        {
            using (var Transaction = connection.BeginTransaction())
            using (var com = connection.CreateCommand())
            {

                com.Transaction = Transaction;
                foreach(KeyValuePair<string, DateTime> item in Items){
                 
                    com.CommandText = $"Update Files Set LastUpdated = {ToUnixDate(item.Value)} where FileName = '{SafeString(item.Key)}';";
                    com.ExecuteNonQuery();
                }
                Transaction.Commit();

            }
        }

        /// <summary>
        /// Updates A File with the Timestamp
        /// </summary>
        /// <param name="FileName">File To Update</param>
        /// <param name="timestamp">Timestamp to set</param>
        public void UpdateFileRecord(string FileName, DateTime timestamp) { ExecuteNonQuery($"Update Files Set LastUpdated = {ToUnixDate(timestamp)} where FileName = '{SafeString(FileName)}';"); }

        /// <summary>
        /// Creates a new File Records
        /// </summary>
        /// <param name="FileName">FileName to Save</param>
        /// <param name="timestamp">Timestamp to Save</param>
        public void InsertFileRecord(string FileName, DateTime timestamp) { ExecuteNonQuery($"INSERT INTO Files (FileName, LastUpdated) VALUES ('{SafeString(FileName)}', {ToUnixDate(timestamp)} );"); }

        /// <summary>
        /// Looks up the File Index (Auto Generated GUID)
        /// </summary>
        /// <param name="FileName">FileName to lookup</param>
        /// <returns></returns>
        public Int64 LookupFileIndex(string FileName)
        {
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"Select FileIndex from Files where FileName = '{SafeString(FileName)}';";

                return (Int64)com.ExecuteScalar();
            }

        }

        /// <summary>
        /// Grabs all Existing Files Stored in the specific Directory
        /// </summary>
        /// <param name="StartDirectory"></param>
        /// <returns></returns>
        public List<Tuple<Int64, string, DateTime>> SelectFiles(string StartDirectory)
        {
            List<Tuple<Int64, string, DateTime>> response = new List<Tuple<long, string, DateTime>>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandText = $"Select FileIndex, FileName, LastUpdated From Files Where FileName like('{StartDirectory}%');";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add(new Tuple<long, string, DateTime>((Int64)reader[0], (string)reader[1], FromUnixDate((long)reader[2])));
                    }
                }
            }
            return response;
        }


        #endregion

        #region Public Lookup Table Functions
        //Table Lookup : KeywordHash (int), CollisionIndex (int), FileIndex (int)
       

        /// <summary>
        /// Grabs the Existing Keyword /Collision record for a file index
        /// </summary>
        /// <param name="fileIndex">GUID of the File</param>
        /// <returns></returns>
        public List<Tuple<long, long>> GetExistingLookupValues(Int64 fileIndex)
        {
            List<Tuple<long, long>> response = new List<Tuple<long, long>>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"SELECT KeywordHash, CollisionIndex FROM Lookup WHERE FileIndex = {fileIndex};";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add(new Tuple<long, long>((long)reader[0], (long)reader[1]));
                    }
                }
            }
            return response;
        }

     

        public List<KeywordObject> GetExistingSavedItems(string FileName)
        {
            List<KeywordObject> response = new List<KeywordObject>();

            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"select Lookup.KeywordHash, Lookup.CollisionIndex FROM Lookup INNER JOIN Files ON Files.FileIndex = Lookup.FileIndex WHERE Files.FileName = '{SafeString(FileName)}';";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add(new KeywordObject((long)reader[0], (long)reader[1]));
                    }

                }
            }
            return response;
        }


        public void BuldInsertLookups(List<Lookup> InsertItems)
        {
            const string starter = "INSERT INTO Lookup (KeywordHash,CollisionIndex, FileIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();
            foreach (Lookup item in InsertItems)
            {
                if (index == 0)
                {
                    ssb.Append(starter);
                }
                ssb.Append($"({item.KeywordHash}, {item.CollisionIndex}, {item.FileIndex}),");
                index += 1;
                if (index == 1000)
                {
                    index = 0;
                    int Length = ssb.Length;
                    ssb = new StringBuilder(ssb.ToString().Substring(0, Length - 1));
                    ssb.Append(";");
                }
            }
            string response = ssb.ToString();
            if (!string.IsNullOrWhiteSpace(response))
            {
                if (response.EndsWith(",")) { response = response.Substring(0, response.Length - 1); }
                if (!response.EndsWith(";")) { response = response + ";"; }
                ExecuteNonQuery(response);
            }

        }

        public void BulkDeleteLookups(List<Lookup> RemoveItems)
        {
            const string DeleteStatement = "DELETE FROM Lookup WHERE KeywordHash = {0} AND CollisionIndex = {1} AND FileIndex = {2};";
            if (RemoveItems.Count > 0)
            {
                StringBuilder DeleteString = new StringBuilder();
                foreach (Lookup item in RemoveItems)
                {
                    DeleteString.Append(string.Format(DeleteStatement, item.KeywordHash, item.CollisionIndex, item.FileIndex));
                }
                ExecuteNonQuery(DeleteString.ToString());
            }
        }

        /// <summary>
        /// Inserts Records into the Lookup Table
        /// </summary>
        /// <param name="LookupItem">List of Keyword Hash / Collision pairs</param>
        /// <param name="FileIndex">FileIndex of filename</param>
        public void InsertLookups(IEnumerable<Tuple<long, long>> LookupItem, long FileIndex)
        {
            const string starter = "INSERT INTO Lookup (KeywordHash,CollisionIndex, FileIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();
            foreach (Tuple<long, long> item in LookupItem)
            {
                if (index == 0)
                {
                    ssb.Append(starter);
                }
                ssb.Append($"({item.Item1}, {item.Item2}, {FileIndex}),");
                index += 1;
                if (index == 1000)
                {
                    index = 0;
                    int Length = ssb.Length;
                    ssb = new StringBuilder(ssb.ToString().Substring(0, Length - 1));
                    ssb.Append(";");
                }
            }
            string response = ssb.ToString();
            if (!string.IsNullOrWhiteSpace(response))
            {
                if (response.EndsWith(",")) { response = response.Substring(0, response.Length - 1); }
                if (!response.EndsWith(";")) { response = response + ";"; }
                ExecuteNonQuery(response);
            }
        }

        /// <summary>
        /// Removes Records from DB for Filename
        /// </summary>
        /// <param name="LookupItem">List of Keyword Hash / Collision pairs</param>
        /// <param name="FileIndex">FileIndex of filename</param>
        public void DeleteLookups(IEnumerable<Tuple<long, long>> LookupItem, long FileIndex)
        {
            const string DeleteStatement = "DELETE FROM Lookup WHERE KeywordHash = {0} AND CollisionIndex = {1} AND FileIndex = {2};";
            if (LookupItem.Any())
            {
                StringBuilder DeleteString = new StringBuilder();
                foreach (Tuple<long, long> item in LookupItem)
                {
                    DeleteString.Append(string.Format(DeleteStatement, item.Item1, item.Item2, FileIndex));
                }
                ExecuteNonQuery(DeleteString.ToString());
            }
        }

        #endregion

        #region Public Keyword Functions
        //Keywords (Text (Text), Hash(int), CollisionIndex (int))

        /// <summary>
        /// Inserts Records into the Keywords Table
        /// </summary>
        /// <param name="Keywords">List of (Keyword string, Generated Hashcode, CollisionId(default 0))</param>
        public void InsetKeywords(IEnumerable<Tuple<string, long, long>> Keywords)
        {
            const string starter = "INSERT INTO Keywords(Text,Hash,CollisionIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();

            foreach (Tuple<string, long, long> item in Keywords)
            {
                if (index == 0) { ssb.Append(starter); }

                ssb.Append($"('{SafeString(item.Item1)}',{item.Item2},{item.Item3}),");
                index += 1;
                if (index == 1000)
                {
                    index = 0;
                    int Length = ssb.Length;
                    ssb = new StringBuilder(ssb.ToString().Substring(0, Length - 1));
                    ssb.Append(";");
                }
            }
            string response = ssb.ToString();
            if (response.EndsWith(",")) { response = response[0..^1]; }
            if (!response.EndsWith(";")) { response += ";"; }
            ExecuteNonQuery(response);

        }
        #endregion

    }
}