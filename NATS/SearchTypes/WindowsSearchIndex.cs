using System;
using System.Text;
using System.Data.OleDb;

namespace NATS.SearchTypes
{
    class WindowsSearchIndex : IndexBase
    {
        const string Proc = "SELECT System.ItemPathDisplay FROM SYSTEMINDEX WHERE scope='{0}' and FREETEXT('%{1}%')";
        public WindowsSearchIndex(ArgumentsObject.ArgumentsObject O) : base(O) { }

        public override void Execute()
        {
            StringBuilder SB = new StringBuilder();
            string query = string.Format(Proc, Arguments.DirectoryPath, Arguments.KeywordSearch); 
            using (OleDbConnection objConnection = new OleDbConnection(Properties.Resources.WinIndexConnection))
            {
                objConnection.Open();
                OleDbCommand cmd = new OleDbCommand(query, objConnection);
                System.Data.DataTable DS = new System.Data.DataTable();
                DS.Load(cmd.ExecuteReader());
                using (OleDbDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        string responseItem = (string)dataReader[0];
                        if (CheckFileExt(responseItem)) { SB.Append(responseItem).Append(Environment.NewLine); }
                    }
                }
                objConnection.Close();
            }
            output = SB.ToString();
        }
    }
}//Optional from internet, but the one I have seams to work,
/*"SELECT System.ItemPathDisplay 
 * FROM SYSTEMINDEX 
 * WHERE scope='{0}'
 * and FREETEXT('%{1}%') 
 * and contains('%{Arguments.KeywordSearch}%'"
 * */
