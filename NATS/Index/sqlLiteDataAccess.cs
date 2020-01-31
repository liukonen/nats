using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;


namespace NATS.Index
{
    class sqlLiteDataAccess : IDisposable
    {
        const string path = @"Data Source=C:\Users\liuko\source\repos\nats\NATS\Index\nats.db;Version=3;";

        private SQLiteConnection connection = new SQLiteConnection();


        public sqlLiteDataAccess()
        {
            connection = new SQLiteConnection(path);
            connection.Open();
        }



        public void UpdateFileRecord(string FileName, DateTime timestamp) { ExecuteNonQuery($"Update Files Set LastUpdated = {ToUnixDate(timestamp)} where FileName = '{FileName}';"); }

        public void InsertFileRecord(string FileName, DateTime timestamp){ExecuteNonQuery($"INSERT INTO Files (FileName, LastUpdated) VALUES ('{FileName}', {ToUnixDate(timestamp)} );");}

        public Int64 LookupFileIndex(string FileName)
        {
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"Select [Index] from Files where FileName = '{FileName}';";

                return (Int64)com.ExecuteScalar();
            }

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


        public HashSet<Int64> GetExistingLookupValues(Int64 fileIndex)
        {
            HashSet<Int64> response = new HashSet<Int64>();
            using (SQLiteCommand com = connection.CreateCommand())
            {
                com.CommandType = CommandType.Text;
                com.CommandText = $"Select Keyword from Lookup where FileIndex = {fileIndex};";
                using (IDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        response.Add((Int64)reader[0]);
                    }
                }
            }
            return response;
        }



       


        public void MergeKeywordPairs(ICollection<Int64> ExistingKeywords, ICollection<Int64> NewKeywords, Int64 FileIndex)
        {
           const string DeleteStatement = "DELETE FROM Lookup WHERE FileIndex = {1} AND Keyword = {0};";
            //const string InsertStatement = "INSERT INTO Lookup (keyword, FileIndex) VALUES ({0}, {1});";

            var ItemsToInsert = from Int64 Item in NewKeywords where !ExistingKeywords.Contains(Item) select Item;
            var ItemsToDelete = from Int64 Item in ExistingKeywords where !NewKeywords.Contains(Item) select Item;
            System.Text.StringBuilder sqlstatement = new StringBuilder();


            //using (var transaction = connection.BeginTransaction())
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = "INSERT INTO Lookup (keyword, FileIndex) VALUES($keyword, $fileindex);";

            //    var nameParameter = command.CreateParameter();
            //    nameParameter.ParameterName = "$keyword";
            //    command.Parameters.Add(nameParameter);

            //    var emailParameter = command.CreateParameter();
            //    emailParameter.ParameterName = "$fileindex";
            //    command.Parameters.Add(emailParameter);

            //    foreach (var contact in ItemsToInsert)
            //    {
            //        nameParameter.Value = contact;
            //        emailParameter.Value = FileIndex;
            //        command.ExecuteNonQuery();
            //    }

            //    transaction.Commit();
            //}

            //using (var transaction = connection.BeginTransaction())
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = "DELETE FROM Lookup WHERE FileIndex = $keyword AND Keyword = '$fileindex';";

            //    var nameParameter = command.CreateParameter();
            //    nameParameter.ParameterName = "$keyword";
            //    command.Parameters.Add(nameParameter);

            //    var emailParameter = command.CreateParameter();
            //    emailParameter.ParameterName = "$fileindex";
            //    command.Parameters.Add(emailParameter);

            //    foreach (var contact in ItemsToDelete)
            //    {
            //        nameParameter.Value = contact;
            //        emailParameter.Value = FileIndex;
            //        command.ExecuteNonQuery();
            //    }

            //    transaction.Commit();

            //}


            //sqlstatement.Append("BEGIN;").Append(Environment.NewLine);
            //foreach (ulong item in ItemsToInsert)
            //{
            //    sqlstatement.Append(String.Format(InsertStatement, item, FileIndex)).Append(Environment.NewLine);
            // }

            sqlstatement.Append(GenerateInsertStatements(ItemsToInsert, FileIndex));
            foreach (Int64 Item in ItemsToDelete)
            {
                sqlstatement.Append(string.Format(DeleteStatement, Item, FileIndex)).Append(Environment.NewLine);
            }
            //sqlstatement.Append("COMMIT;");
            ExecuteNonQuery(sqlstatement.ToString());
        }

        private string GenerateInsertStatements(IEnumerable<Int64> itemsToInsert, long FileIndex)
        {
            const string starter = "INSERT INTO Lookup (keyword, FileIndex) VALUES ";
             int index = 0;
            StringBuilder ssb = new StringBuilder();
            foreach (Int64 item in itemsToInsert)
            {
                if (index == 0)
                {
                    ssb.Append(starter);
                }
                ssb.Append("(").Append(item.ToString()).Append(",").Append(FileIndex.ToString()).Append("),");


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
            if (response.EndsWith(",")) { response = response.Substring(0, response.Length - 1); }
            if (!response.EndsWith(";")) { response = response + ";"; }
            return response;
        
        }

        public void TryInsertKeywords(IEnumerable<KeyValuePair<string, Int64>> Keywords)
        {
            const String TryInsert = "INSERT INTO Keywords(Text,Hash) SELECT '{0}', '{1}' WHERE NOT EXISTS(SELECT 1 FROM Keywords WHERE Hash = {1}); ";
            StringBuilder slCommand = new StringBuilder();
            foreach (var item in Keywords)
            {
                slCommand.Append(string.Format(TryInsert, SafeString(item.Key), item.Value)).Append(Environment.NewLine) ;
            }
            ExecuteNonQuery(slCommand.ToString());
        }


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
            DateTimeOffset item2 = item.ToLocalTime();
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
            return item2.LocalDateTime;
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
