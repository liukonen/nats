using System;
using System.IO;
namespace NATS.Comparers
{
    public class SingleLineLoadComparer : SingleLineRawComparer
    {
        public override Tuple<bool, string> Compare(FileInfo file, string keyword)
        {
            if (TenMB > file.Length)
            {
                try
                {
                    using (StreamReader FS = file.OpenText())
                    {
                        string line = FS.ReadToEnd();
                        if (HasRecord(line, keyword))
                        {
                            return new Tuple<bool, string>(true, file.FullName);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException) { }
                catch (System.IO.IOException) { }//do nothing
                return new Tuple<bool, string>(false, string.Empty);
            }
            else { return base.Compare(file, keyword); }
        }
    }
}