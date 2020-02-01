using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Linq;

//Needs to be a singleton

namespace NATS.Index
{



    class sqlLiteDataAccess : IDisposable
    {
        const string path = @"Data Source=nats.db;Version=3;";

        private SQLiteConnection connection = new SQLiteConnection();

        private static sqlLiteDataAccess _instance;

        public static sqlLiteDataAccess Instance()
        { 
        if (_instance == null) { _instance = new sqlLiteDataAccess(); }
            return _instance;
        }


        protected sqlLiteDataAccess()
        {
            connection = new SQLiteConnection(path);
            connection.Open();
        }

        public string[] InquireOnDB(string keyword, string directoryPath)
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
            return response.ToArray();
        }

        #region File Table Specific Calls
        //Table Files  - FileIndex (INT) FileName (TEXT) LastUpdated (INT {converted to datetime})

        public void UpdateFileRecord(string FileName, DateTime timestamp) { ExecuteNonQuery($"Update Files Set LastUpdated = {ToUnixDate(timestamp)} where FileName = '{SafeString(FileName)}';"); }

        public void InsertFileRecord(string FileName, DateTime timestamp){ExecuteNonQuery($"INSERT INTO Files (FileName, LastUpdated) VALUES ('{SafeString(FileName)}', {ToUnixDate(timestamp)} );");}

        public Int64 LookupFileIndex(string FileName)
        {
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"Select FileIndex from Files where FileName = '{SafeString(FileName)}';";

                return (Int64)com.ExecuteScalar();
            }

        }
        
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
        
        public Dictionary<string, DateTime> LookupExistingFiles()
        {
            Dictionary<string, DateTime> returnValue = new Dictionary<string, DateTime>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = "Select FileName, LastUpdated from Files;";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returnValue.Add((string)reader[0], FromUnixDate((long)reader[1]));
                    }
                }
            }
            return returnValue;
        }

        #endregion

        #region Lookup Table Functions
        //Table Lookup : KeywordHash (int), CollisionIndex (int), FileIndex (int)


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
                        //response.Add(new Tuple<long, long>((long)reader[0], (long)reader[1]);
                    }
                }
            }
            return response;
        }


        public void InsertLookups(IEnumerable<Tuple<long, long>> LookupItem, long FileIndex)
        {
            const string starter = "INSERT INTO Lookup (KeywordHash,CollisionIndex, FileIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();
            foreach (Tuple <long, long> item in LookupItem)
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
            if (!string.IsNullOrWhiteSpace(response)) { 
            if (response.EndsWith(",")) { response = response.Substring(0, response.Length - 1); }
            if (!response.EndsWith(";")) { response = response + ";"; }
            ExecuteNonQuery(response);
            }
        }

        public void DeleteLookups(IEnumerable<Tuple<long, long>> LookupItem, long FileIndex)
        {
            const string DeleteStatement = "DELETE FROM Lookup WHERE KeywordHash = {0} AND CollisionIndex = {1} AND FileIndex = {2};";
            if (LookupItem.Any()) { 
            StringBuilder DeleteString = new StringBuilder();
            foreach (Tuple<long, long> item in LookupItem)
            {
                DeleteString.Append(string.Format(DeleteStatement, item.Item1, item.Item2, FileIndex));
            }
            ExecuteNonQuery(DeleteString.ToString());
            }
        }


        #endregion

        #region Keyword Functions
        //Keywords (Text (Text), Hash(int), CollisionIndex (int))


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

        public long GetNextCollission()
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

        public void InsetKeywords(IEnumerable<Tuple<string, long, long>> Keywords)
        { 
         const string starter = "INSERT INTO Keywords(Text,Hash,CollisionIndex) VALUES ";
            int index = 0;
            StringBuilder ssb = new StringBuilder();

            foreach (Tuple<string, long, long> item in Keywords)
            {
                if (index == 0){ssb.Append(starter);}

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


        /// <summary>
        /// Makes a string value safe to insert into SQLITE
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string SafeString(string item)
        { 
        return item.Replace("'","''");
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

        /// <summary>
        /// cleans up any connections if still open
        /// </summary>
        public void Dispose()
        {
            
            if (connection.State == ConnectionState.Open) connection.Close();
        }
    }
}
