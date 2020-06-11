namespace NATS.SearchTypes
{
    class InternalIndexGenerate : Searchbase
    {
        public InternalIndexGenerate(ArgumentsObject.ArgumentsObject o) : base(o) { }

        public override void Execute()
        {
            Index.liteDBindex CustomIndex = new Index.liteDBindex();
            CustomIndex.Generate(Arguments.DirectoryPath, true);
            output = "complete.";
            CustomIndex.close();
        }
    }
}
