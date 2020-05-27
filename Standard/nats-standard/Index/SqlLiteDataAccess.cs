using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

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

        public List<string> InquireOnDb(string[] keyword, string directoryPath)
        {
            string proc = "SELECT DISTINCT Files.FileName FROM Keywords INNER JOIN Lookup on Lookup.KeywordHash = Keywords.Hash AND Keywords.CollisionIndex = Lookup.CollisionIndex INNER JOIN Files ON Files.FileIndex = Lookup.FileIndex WHERE Files.FileName like'{0}%' AND Keywords.Text like ('%{1}%');";

            List<string> response = new List<string>();
            Boolean run = false;

            using (SQLiteTransaction Trans = connection.BeginTransaction())
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.Transaction = Trans;

                foreach (string item in keyword)
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
        public Dictionary<string, KeywordObject> LookupAllKeywords()
        {
            Dictionary<string, KeywordObject> returnValue = new Dictionary<string, KeywordObject>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = "Select Text, Hash, CollisionIndex from Keywords;";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returnValue.Add((string)reader[0], new KeywordObject((long)reader[1], (long)reader[2]));
                    }
                }
            }
            return returnValue;
        }

        /// <summary>
        /// cleans up any connections if still open
        /// </summary>
        public void Dispose() { if (connection.State == ConnectionState.Open) connection.Close(); }


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
            connection = new SQLiteConnection(nats_standard.Properties.Resources.InternalIndexConnection);
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
            using (SQLiteTransaction Transaction = connection.BeginTransaction())
            using (SQLiteCommand com = connection.CreateCommand())
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
            using (SQLiteTransaction Transaction = connection.BeginTransaction())
            using (SQLiteCommand com = connection.CreateCommand())
            {

                com.Transaction = Transaction;
                foreach (KeyValuePair<string, DateTime> item in Items)
                {

                    com.CommandText = $"Update Files Set LastUpdated = {ToUnixDate(item.Value)} where FileName = '{SafeString(item.Key)}';";
                    com.ExecuteNonQuery();
                }
                Transaction.Commit();

            }
        }



        /// <summary>
        /// Grabs all Existing Files Stored in the specific Directory
        /// </summary>
        /// <param name="StartDirectory"></param>
        /// <returns></returns>
        public Dictionary<string, FileObjects> SelectFiles(string StartDirectory)
        {
            Dictionary<string, FileObjects> response = new Dictionary<string, FileObjects>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandText = $"Select FileIndex, FileName, LastUpdated From Files Where FileName like('{StartDirectory}%');";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add((string)reader[1], new FileObjects((long)reader[0], FromUnixDate((long)reader[2])));
                    }
                }
            }
            return response;
        }


        #endregion

        #region Public Lookup Table Functions
        //Table Lookup : KeywordHash (int), CollisionIndex (int), FileIndex (int)

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
        #endregion

        #region Public Keyword Functions
        //Keywords (Text (Text), Hash(int), CollisionIndex (int))

        /// <summary>
        /// Inserts Records into the Keywords Table
        /// </summary>
        /// <param name="Keywords">List of (Keyword string, Generated Hashcode, CollisionId(default 0))</param>
        public void InsetKeywords(IEnumerable<KeyValuePair<string, KeywordObject>> Keywords)
        {
            const string starter = "INSERT INTO Keywords(Text,Hash,CollisionIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();

            foreach (KeyValuePair<string, KeywordObject> item in Keywords)
            {
                if (index == 0) { ssb.Append(starter); }

                ssb.Append($"('{SafeString(item.Key)}',{item.Value.Hash},{item.Value.Collision}),");
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
            if (response.EndsWith(","))
            {
                response = response.Substring(0, response.Length - 1); // response[0..^1]; }
                if (!response.EndsWith(";")) { response += ";"; }
                ExecuteNonQuery(response);
            }
            #endregion

        }
    }
}