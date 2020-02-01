using System;
using System.Collections.Generic;
using System.Text;

namespace NATS.SearchTypes
{
    class InternalIndex : Searchbase
    {

        public InternalIndex(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.SQLiteIndex CustomIndex = new Index.SQLiteIndex();
            string[] Response = CustomIndex.Inquire(Arguments.KeywordSearch, Arguments.DirectoryPath);
            output = string.Join(Environment.NewLine, Response);
            CustomIndex.close();
        }
    }

    class InternalIndexGenerate : Searchbase
    {
        public InternalIndexGenerate(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.SQLiteIndex CustomIndex = new Index.SQLiteIndex();
            CustomIndex.Generate(Arguments.DirectoryPath, true);
            output = "complete.";
            CustomIndex.close();
        }
    }

    class InternalGenerateAndIndex : Searchbase
    {
        public InternalGenerateAndIndex(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.SQLiteIndex CustomIndex = new Index.SQLiteIndex();
            Console.WriteLine("Building Index");
            CustomIndex.Generate(Arguments.DirectoryPath, false);
            Console.WriteLine("Search:");
            string[] Response = CustomIndex.Inquire(Arguments.KeywordSearch, Arguments.DirectoryPath);
            output = string.Join(Environment.NewLine, Response);
            CustomIndex.close();
        }

    }
}
