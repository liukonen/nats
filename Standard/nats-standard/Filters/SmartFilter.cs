using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NATS.Filters
{
    public class SmartSearchFilter : FileInfoFilters
    {
        #region Global Vars
        private List<byte[]> knownEncodings;
        #endregion

        #region Public

        /// <summary>
        /// Constructor
        /// </summary>
        public SmartSearchFilter()
        {
            knownEncodings = GetEncodings();
        }

        /// <summary>
        /// Uses Byte Order Marker to see if the file is a text file
        /// </summary>
        /// <param name="FileInfo"></param>
        /// <returns></returns>
        public override bool IsValid(FileInfo FileInfo)
        {
            ///Using 4096 [default sector size] as an initial size
            int FileSize = (FileInfo.Length > 4096) ? 4096 : (int)FileInfo.Length;
            if (FileSize == 0) { return false; }
            return IsText(FileInfo, 4096);
        }

        #endregion

        #region Private

        /// <summary>
        /// Attemps to grab the first [size] number of bytes of the File, to check its Byte order Marker
        /// </summary>
        /// <param name="File"></param>
        /// <param name="size"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if the start of the Data Stream starts with the Byte order mark 
        /// </summary>
        /// <param name="data">Data to check</param>
        /// <param name="knownEncodings">list of known Byte order marks</param>
        /// <returns></returns>
        private bool DataStartsWithBom(byte[] data, List<byte[]> knownEncodings)
        {
            foreach (byte[] bom in knownEncodings)
            {
                if (data.Zip(bom, (x, y) => x == y).All(x => x)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Grabs the list of known text encodings
        /// </summary>
        /// <returns></returns>
        private static List<byte[]> GetEncodings()
        {
            System.Text.EncodingInfo[] encodings = System.Text.Encoding.GetEncodings();
            List<byte[]> EncodePre = new List<byte[]>();

            foreach (System.Text.EncodingInfo encoding in encodings)
            {
                try
                {
                    byte[] pre = encoding.GetEncoding().GetPreamble();
                    if (pre.Length > 0) { EncodePre.Add(pre); }
                }
                catch  { };
            }
            return EncodePre;
        }
        #endregion
    }
}
