using System.Collections.Generic;
using System.IO;

namespace NATS.Filters
{
    public class FileExtentionFilter : FileInfoFilters
    {
        public enum filterType { WhiteList, BlackList }

        public filterType Ftype = filterType.BlackList;
        public const string DefaultFileExtentions = "";
        List<string> FileExtentions = new List<string>();

        public FileExtentionFilter(string ParamArguments, filterType type)
        {
            FileExtentions = ExtractBlackListExtentions(ParamArguments);
            Ftype = type;
        }

        public override bool IsValid(FileInfo FileInfo)
        {

            if (FileExtentions.Contains(FileInfo.Extension.ToLower())) { return Ftype == filterType.WhiteList; }
            return (Ftype == filterType.BlackList);
            /*
             * Same as
             * if (filterType.WhiteList)
             * {
             *   if (FileExtentions.Contains(FileInfo.Extension.ToLower())){return false;}
             *   return true;
             *  }else{
             *  if (FileExtentions.Contains(FileInfo.Extension.ToLower())){return true;}
             *  return false;
             *  }
             * */
        }

        private static List<string> ExtractBlackListExtentions(string BlackListVaraible)
        {
            List<string> BlackListArray = new List<string>();
            foreach (string item in BlackListVaraible.Split('|'))
            {
                string Ext = "." + item.Replace("|", string.Empty).ToLower();
                if (!BlackListArray.Contains(Ext)) { BlackListArray.Add(Ext); }
            }
            return BlackListArray;
        }
    }
}
