using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NATS.SearchTypes
{
    internal class SingleThread : Searchbase
    {

        public SingleThread(ArgumentsObject.ArgumentsObject arguments) : base(arguments) { }

        public override void Execute()
        {
            StringBuilder FilesFound = new StringBuilder();
            //IEnumerable<FileInfo> Files = (new DirectoryInfo(Arguments.DirectoryPath)).EnumerateFiles("*", Arguments.EOptions);
            IEnumerable<FileInfo> Files = (new DirectoryInfo(Arguments.DirectoryPath)).EnumerateFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo item in Files)
            {
                Tuple<bool, string> Response = CheckFile(item, Arguments);
                if (Response.Item1) { FilesFound.Append(Response.Item2).Append(Environment.NewLine); }
            }
            output = FilesFound.ToString();
        }
    }
}
