namespace NATS.SearchTypes
{
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
}
