using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NATS.SearchTypes
{
    class MultiThread : Searchbase
    {
        public MultiThread(ArgumentsObject.ArgumentsObject O) : base(O) { }

        public override void Execute()
        {
            IEnumerable<FileInfo> Files = (new DirectoryInfo(Arguments.DirectoryPath)).EnumerateFiles("*", Arguments.EOptions);

            ConcurrentBag<string> ReturnItems = new ConcurrentBag<string>();
            ParallelOptions Options = new ParallelOptions() { MaxDegreeOfParallelism = Arguments.ThreadCount };
            Parallel.ForEach(Files, Options, (currentFile) =>
            {
                var response = CheckFile(currentFile, Arguments);
                if (response.Item1) { ReturnItems.Add(response.Item2); }
            });
            output = (string.Join(Environment.NewLine, from string X in ReturnItems select X));
        }

    }
}
