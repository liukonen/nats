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
            FileExtentions = ExtractBlackListExtentions(ParamArguments, type);
            Ftype = type;
        }

        public override bool IsValid(FileInfo FileInfo)
        {
            if (FileExtentions.Contains(FileInfo.Extension.ToLower())) { return Ftype == filterType.WhiteList; }
            return (Ftype == filterType.BlackList);
        }

        private static List<string> ExtractBlackListExtentions(string BlackListVaraible, filterType type)
        {
            List<string> BlackListArray = new List<string>();
            if (BlackListVaraible == string.Empty && type == filterType.BlackList) { BlackListVaraible = nats_standard.Properties.Resources.DefaultBlacklist; }

            foreach (string item in BlackListVaraible.Split('|'))
            {
                string ext = item.ToLower();
                if (!ext.StartsWith(".")) { ext = "." + ext; }
                if (!BlackListArray.Contains(ext)) { BlackListArray.Add(ext); }
            }
            return BlackListArray;
        }
    }
}
