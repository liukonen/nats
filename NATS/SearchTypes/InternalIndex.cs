using System;
using System.Collections.Generic;
using System.Linq;

namespace NATS.SearchTypes
{
    class InternalIndex : IndexBase
    {

        public InternalIndex(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.SQLiteIndex CustomIndex = new Index.SQLiteIndex();
            List<string> Response = CustomIndex.Inquire(Arguments.KeywordSearch, Arguments.DirectoryPath);
            output = string.Join(Environment.NewLine, from string Item in Response where CheckFileExt(Item) select Item);
            CustomIndex.close();
        }
    }


}
