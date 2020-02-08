using System;
using System.IO;
using System.Linq;

namespace NATS.SearchTypes
{
    public class Searchbase
    {

        protected string output = string.Empty;

        protected ArgumentsObject.ArgumentsObject Arguments;
        public Searchbase(ArgumentsObject.ArgumentsObject arguments)
        { Arguments = arguments; }

        public static Tuple<bool, string> CheckFile(FileInfo item, ArgumentsObject.ArgumentsObject args)
        {
            var NotValid = (from NATS.Filters.FileInfoFilters F in args.FileInfoFilters where F.IsValid(item) == false select F).Any();
            if (!NotValid) //double negative... could word it better
            {
                return args.Comparer.Compare(item, args.KeywordSearch);
            }
            return new Tuple<bool, string>(false, string.Empty);
        }

        public override string ToString()
        {
            return output;
        }

        //MustOverride object
        public virtual void Execute() { throw new NotImplementedException(); }

    }
}
