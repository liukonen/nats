using System;
using System.Text;
using System.IO;
namespace NATS.Comparers
{
    public class MultiLineRawComparer : baseComparer
    {
        public override Tuple<bool, string> Compare(FileInfo file, string keyword)
        {
            StringBuilder Items = new StringBuilder();
            Boolean _hasItem = false;
            Int64 counter = 1;
            using (StreamReader FS = file.OpenText())
            {
                string line = String.Empty;
                while ((line = FS.ReadLine()) != null)
                {
                    if (HasRecord(line, keyword))
                    {
                        _hasItem = true;
                        if (Items.Length > 0) { Items.Append(Environment.NewLine); }
                        Items.Append(counter.ToString()).Append(" ").Append(file.FullName);
                    }
                }
            }
            return new Tuple<bool, string>(_hasItem, Items.ToString());
        }
    }
}