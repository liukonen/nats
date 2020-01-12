using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NATS.Filters
{
    public class SmartSearchFilter : FileInfoFilters
    {
        private List<byte[]> knownEncodings;

        private static List<byte[]> GetEncodings()
        {
            var X = System.Text.Encoding.GetEncodings();
            List<byte[]> EncodePre = new List<byte[]>();

            foreach (var Y in X)
            {
                byte[] pre = Y.GetEncoding().GetPreamble();
                if (pre.Length > 0) { EncodePre.Add(pre); }
            }
            return EncodePre;
        }

        public SmartSearchFilter()
        {
            knownEncodings = GetEncodings();
        }

        public override bool IsValid(FileInfo FileInfo)
        {
            int FileSize = (FileInfo.Length > 4096) ? 4096 : (int)FileInfo.Length;
            if (FileSize == 0) { return false; }
            return IsText(FileInfo, 4096);
        }

        private Boolean IsText(FileInfo File, int size)
        {
            byte[] bytes = new byte[size];
            using (FileStream fs = File.OpenRead())
            {

                fs.Read(bytes, 0, size);
                fs.Close();
            }
            return DataStartsWithBom(bytes, knownEncodings);
        }

        private bool DataStartsWithBom(byte[] data, List<byte[]> knownEncodings)
        {
            foreach (byte[] bom in knownEncodings)
            {
                if (data.Zip(bom, (x, y) => x == y).All(x => x)) { return true; }
            }
            return false;
        }

    }

}
