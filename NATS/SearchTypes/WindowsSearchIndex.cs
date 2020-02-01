using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace NATS.SearchTypes
{
    class WindowsSearchIndex :Searchbase
    {
        const string DBConnection = "Provider=Search.CollatorDSO.1;Extended? Properties = 'Application=Windows';";
        const string Proc = "SELECT System.ItemPathDisplay FROM SYSTEMINDEX WHERE scope='{0}' and FREETEXT('%{1}%')";// and contains('%{Arguments.KeywordSearch}%')";
        public WindowsSearchIndex(ArgumentsObject.ArgumentsObject O) : base(O) { }

        public override void Execute()
        {
            System.Text.StringBuilder SB = new StringBuilder();
            string query = string.Format(Proc, Arguments.DirectoryPath, Arguments.KeywordSearch); 
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
