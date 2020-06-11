using System;
using System.Collections.Generic;
using System.Linq;

namespace NATS.SearchTypes
{

    class InternalGenerateAndIndex : IndexBase
    {
        public InternalGenerateAndIndex(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.liteDBindex CustomIndex = new Index.liteDBindex();
            Console.WriteLine("Building Index");
            CustomIndex.Generate(Arguments.DirectoryPath, false);
            Console.WriteLine("Search:");
            List<string> Response = CustomIndex.Inquire(Arguments.KeywordSearch, Arguments.DirectoryPath);
            output = string.Join(Environment.NewLine, from string Item in Response where CheckFileExt(Item) select Item);
            CustomIndex.close();
        }

    }
}
