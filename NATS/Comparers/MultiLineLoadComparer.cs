using System;
using System.Text;
using System.IO;
namespace NATS.Comparers
{
    public class MultiLineLoadComparer : MultiLineRawComparer
    {
        public override Tuple<bool, string> Compare(FileInfo file, string keyword)
        {
            StringBuilder Items = new StringBuilder();
            Boolean _hasItem = false;
            if (TenMB > file.Length)
            {
                Int64 counter = 1;
                using (StreamReader FS = file.OpenText())
                {
                    string[] line = FS.ReadToEnd().Split(Environment.NewLine);
                    foreach (string TestItem in line)
                    {
                        if (HasRecord(TestItem, keyword))
                        {
                            if (Items.Length > 0) { Items.Append(Environment.NewLine); }
                            Items.Append(counter.ToString()).Append(" ").Append(file.FullName);
                            _hasItem = true;
                        }
                        counter++;
                    }
                }
                return new Tuple<bool, string>(_hasItem, Items.ToString());
            }
            else { return base.Compare(file, keyword); }
        }
    }
}