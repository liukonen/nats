using System.Collections.Generic;
using System.IO;

namespace NATS.Filters
{
    public class FileExtentionFilter : FileInfoFilters
    {
        public enum filterType { AproveList, DenyList }

        public filterType Ftype = filterType.DenyList;
        public const string DefaultFileExtentions = "";
        List<string> FileExtentions = new List<string>();

        public FileExtentionFilter(string ParamArguments, filterType type)
        {
            FileExtentions = ExtractDenyListExtentions(ParamArguments, type);
            Ftype = type;
        }

        public override bool IsValid(FileInfo FileInfo)
        {
            if (FileExtentions.Contains(FileInfo.Extension.ToLower())) { return Ftype == filterType.AproveList; }
            return (Ftype == filterType.DenyList);
        }

        private static List<string> ExtractDenyListExtentions(string DenyListVaraible, filterType type)
        {
            List<string> DenyListArray = new List<string>();
            if (DenyListVaraible == string.Empty && type == filterType.DenyList) { DenyListVaraible = nats_standard.Properties.Resources.DefaultDenylist; }

            foreach (string item in DenyListVaraible.Split('|'))
            {
                string ext = item.ToLower();
                if (!ext.StartsWith(".")) { ext = "." + ext; }
                if (!DenyListArray.Contains(ext)) { DenyListArray.Add(ext); }
            }
            return DenyListArray;
        }
    }
}
