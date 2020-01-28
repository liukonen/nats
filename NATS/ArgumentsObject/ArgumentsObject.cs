using System;
using System.Collections.Generic;
using System.Text;

namespace NATS.ArgumentsObject
{
    public class ArgumentsObject
    {
        public enum eSearchType { Single, Threaded, Index }
        public int ThreadCount;
        public string DirectoryPath;
        public string KeywordSearch;
        public Boolean DisplayHelp = false;
        public eSearchType SearchType = eSearchType.Single;
        public string FileNameOutput = string.Empty;

        public System.IO.EnumerationOptions EOptions = new System.IO.EnumerationOptions()
        {
            ReturnSpecialDirectories = false,
            IgnoreInaccessible = true,
            RecurseSubdirectories = false
        };

        public NATS.Comparers.baseComparer Comparer
        {
            get {
                if (MultiLine)
                { 
                if (MemoryLoad) { return new Comparers.MultiLineLoadComparer(); }
                    else { return new Comparers.MultiLineRawComparer(); }
                }
                else
                { 
                if (MemoryLoad) { return new Comparers.SingleLineLoadComparer(); }
                else { return new Comparers.SingleLineRawComparer(); }
                }
            }
        }


        private bool MultiLine = false;
        private bool MemoryLoad = false;

        public List<NATS.Filters.FileInfoFilters> FileInfoFilters = new List<Filters.FileInfoFilters>();

        public ArgumentsObject(string arguments)
        {
            string ExtentionList = NATS.Filters.FileExtentionFilter.DefaultFileExtentions;
            string[] Items = arguments.Split('-');
            Filters.FileExtentionFilter.filterType FilterType = Filters.FileExtentionFilter.filterType.BlackList;

            foreach (string Item in Items)
            {
                if (!string.IsNullOrEmpty(Item))
                {
                    string Keyitem = Item.Substring(0, 1).ToUpper();
                    string itemVal = Item.Substring(1).Trim();
                    switch (Keyitem)
                    {
                        case "P": DirectoryPath = itemVal; break;
                        case "K": KeywordSearch = itemVal; break;
                        case "T": 
                            if (!int.TryParse(itemVal, out ThreadCount))
                            { ThreadCount = 4; }
                            SearchType = eSearchType.Threaded;
                            break;
                        case "B": ExtentionList = itemVal; break;
                        case "W": 
                            ExtentionList = itemVal; 
                            FilterType = Filters.FileExtentionFilter.filterType.WhiteList; 
                            break;
                        case "L": MultiLine = true; break;
                        case "S": EOptions.RecurseSubdirectories = true; break;
                        case "H": DisplayHelp = true; break;
                        case "R": MemoryLoad = true; break;
                        case "M": FileInfoFilters.Add(new Filters.SmartSearchFilter()); break;
                        case "O": FileNameOutput = itemVal; break;
                        case "I": SearchType = eSearchType.Index; break;
                    }
                }

            }
            if (String.IsNullOrWhiteSpace(KeywordSearch) || string.IsNullOrWhiteSpace(DirectoryPath)) { DisplayHelp = true; }
            FileInfoFilters.Add(new NATS.Filters.FileExtentionFilter(ExtentionList, FilterType));
        }

        public void SaveOutputIfNeeded(StringBuilder TextToSave)
        {
            if (!string.IsNullOrWhiteSpace(FileNameOutput))
            {
                System.IO.File.WriteAllText(FileNameOutput, TextToSave.ToString());
            }
        }
    }
}