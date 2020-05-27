using System;

namespace NATS.SearchTypes
{
    public class IndexBase : Searchbase
    {

        private Filters.FileExtentionFilter FileFilter;
        private Boolean HasFilter = false;

        public IndexBase(ArgumentsObject.ArgumentsObject o) : base(o)
        {
            foreach (NATS.Filters.FileInfoFilters item in Arguments.FileInfoFilters)
            {
                if (item is Filters.FileExtentionFilter) { FileFilter = (Filters.FileExtentionFilter)item; HasFilter = true; }
            }
        }

        public Boolean CheckFileExt(string FileName)
        {

            if (!HasFilter) { return true; }
            return FileFilter.IsValid(new System.IO.FileInfo(FileName));
        }

    }
}