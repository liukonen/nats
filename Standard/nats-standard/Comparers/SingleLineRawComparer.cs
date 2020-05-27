using System;
using System.IO;
namespace NATS.Comparers
{
    public class SingleLineRawComparer : baseComparer
    {
        public override Tuple<bool, string> Compare(FileInfo file, string keyword)
        {
            try
            {
                using (StreamReader FS = file.OpenText())
                {
                    string line = String.Empty;
                    while ((line = FS.ReadLine()) != null)
                    {
                        if (HasRecord(line, keyword))
                        {
                            return new Tuple<bool, string>(true, file.FullName);
                        }
                    }
                }
            }
            catch (System.UnauthorizedAccessException) { }
            catch (System.IO.IOException) { }//do nothing if its an IO exception
            return new Tuple<bool, string>(false, string.Empty);
        }
    }
}