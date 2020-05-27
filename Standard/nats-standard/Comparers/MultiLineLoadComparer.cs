using System;
using System.IO;
using System.Text;
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
                try
                {
                    using (StreamReader FS = file.OpenText())
                    {

                        string[] line = FS.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
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
                }
                catch (System.UnauthorizedAccessException) { }
                catch (System.IO.IOException) { }//Do nothing
                return new Tuple<bool, string>(_hasItem, Items.ToString());
            }
            else { return base.Compare(file, keyword); }
        }
    }
}