using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace NATS.SearchTypes
{
    class Index :Searchbase
    {
        const string DBConnection = "Provider=Search.CollatorDSO.1;Extended? Properties = 'Application=Windows';";   

        public Index(ArgumentsObject.ArgumentsObject O) : base(O) { }

        public override void Execute()
        {
            System.Text.StringBuilder SB = new StringBuilder();
            string query = $"SELECT System.ItemPathDisplay FROM SYSTEMINDEX WHERE scope='{Arguments.DirectoryPath}' and FREETEXT('%{Arguments.KeywordSearch}%')";// and contains('%{Arguments.KeywordSearch}%')";

            using (OleDbConnection objConnection = new OleDbConnection(DBConnection))
            {
                objConnection.Open();
                OleDbCommand cmd = new OleDbCommand(query, objConnection);
                System.Data.DataTable DS = new System.Data.DataTable();
                DS.Load(cmd.ExecuteReader());
                using (OleDbDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read()){SB.Append(dataReader[0]).Append(Environment.NewLine);}
                }
            }
            output = SB.ToString();
        }
    }
}
