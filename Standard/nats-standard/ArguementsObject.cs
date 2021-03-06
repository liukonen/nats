﻿using System;
using System.Collections.Generic;

namespace NATS.ArgumentsObject
{
    public class ArgumentsObject
    {
        #region Private Properties

        public bool MultiLine = false;
        public bool MemoryLoad = false;

        #endregion

        #region Public Properties
        public enum eSearchType { Single, Threaded, WindowsIndex, LocalIndex, IndexGenerate, indexgenerateandsearch }
        public int ThreadCount;
        public string DirectoryPath;
        public string KeywordSearch;
        public Boolean DisplayHelp = false;
        public eSearchType SearchType = eSearchType.Single;
        public string FileNameOutput = string.Empty;
        public List<NATS.Filters.FileInfoFilters> FileInfoFilters = new List<Filters.FileInfoFilters>();
      
       // public System.IO.EnumerationOptions EOptions = new System.IO.EnumerationOptions()
       // {
       //     ReturnSpecialDirectories = false,
       //     IgnoreInaccessible = true,
       //     RecurseSubdirectories = true
       // };
        public NATS.Comparers.baseComparer Comparer
        {
            get
            {
                if (MultiLine)
                {
                    if (MemoryLoad) { return new Comparers.MultiLineLoadComparer(); }
                    return new Comparers.MultiLineRawComparer();
                }
                else
                {
                    if (MemoryLoad) { return new Comparers.SingleLineLoadComparer(); }
                    return new Comparers.SingleLineRawComparer();
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Used in cli based apps
        /// </summary>
        /// <param name="arguments"></param>
        public ArgumentsObject(string arguments)
        {
            string ExtentionList = NATS.Filters.FileExtentionFilter.DefaultFileExtentions;
            string[] Items = arguments.Split('-');
            Filters.FileExtentionFilter.filterType FilterType = Filters.FileExtentionFilter.filterType.DenyList;

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
                        case "D": ExtentionList = itemVal; break;
                        case "A":
                            ExtentionList = itemVal; FilterType = Filters.FileExtentionFilter.filterType.AproveList;
                            break;
                        case "M": MultiLine = true; break;
                        case "H": DisplayHelp = true; break;
                        case "R": MemoryLoad = true; break;
                        case "S": FileInfoFilters.Add(new Filters.SmartSearchFilter()); break;
                        case "O": FileNameOutput = itemVal; break;
                        case "W": SearchType = eSearchType.WindowsIndex; break;
                        case "B": SearchType = eSearchType.IndexGenerate; break;
                        case "L": SearchType = eSearchType.LocalIndex; break;
                        case "I": SearchType = eSearchType.indexgenerateandsearch; break;
                    }
                }

                /* -ApprovedList 
                 * -Build Index
                 * -DisapprovedList 
                 * -Help
                 * -Index search
                 * -Limited Index Search (not fresh)
                 * -Keyword
                 * -MultiLine
                 * -Output
                 * -Path
                 * -Ram [memory]
                 * -SmartSearch
                 * -Threading
                 * -Windows index search*/

            }
            if (String.IsNullOrWhiteSpace(KeywordSearch) || string.IsNullOrWhiteSpace(DirectoryPath)) { DisplayHelp = true; }
            FileInfoFilters.Add(new NATS.Filters.FileExtentionFilter(ExtentionList, FilterType));
        }

        /// <summary>
        /// Used in Winform generation
        /// </summary>
        public ArgumentsObject(Filters.FileExtentionFilter.filterType filterType, string filterExtentions, Boolean UseSmartSearch)
        {
            if(UseSmartSearch){ FileInfoFilters.Add(new Filters.SmartSearchFilter()); }
            FileInfoFilters.Add(new NATS.Filters.FileExtentionFilter(filterExtentions, filterType));
        }
        #endregion
    }
}