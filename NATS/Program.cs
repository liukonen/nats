using NATS.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NATS
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> HeaderItems = new List<string>();
            string consoleOutput;
            StringBuilder FilesFound = new StringBuilder();
            string fullArgs = ArgsToArg(args);

            fullArgs = @"-P d:\newdocs -K luke -b";

            ArgumentsObject.ArgumentsObject ScanArg = new ArgumentsObject.ArgumentsObject(fullArgs);
            if (!ScanArg.DisplayHelp)
            {
                consoleOutput = string.Concat(Resources.Name, Resources.Seperator, ScanArg.KeywordSearch,
                   Resources.Seperator, ScanArg.DirectoryPath, Environment.NewLine, "start timestamp:", GetDateTime(), Environment.NewLine, Resources.Line, Environment.NewLine);

                Console.WriteLine(consoleOutput);
                IEnumerable<FileInfo> Files = (new DirectoryInfo(ScanArg.DirectoryPath)).EnumerateFiles("*", ScanArg.EOptions);
                SearchTypes.Searchbase Search = LookupSearchBase(ScanArg);
                Search.Execute();
                FilesFound.Append(Search.ToString());
                FilesFound.Append(Environment.NewLine).Append(Resources.Fin).Append(Environment.NewLine).Append("End timestamp:").Append(GetDateTime());

                if (!string.IsNullOrWhiteSpace(ScanArg.FileNameOutput))
                {
                    System.IO.File.WriteAllText(ScanArg.FileNameOutput, consoleOutput + FilesFound.ToString());
                    Console.WriteLine(Resources.Fin + Environment.NewLine + "End timestamp:" + GetDateTime());
                }
                else { Console.WriteLine(FilesFound.ToString()); }

            }
            else { Console.WriteLine(Resources.Help); }
        }

        static string GetDateTime() { return string.Concat(DateTime.Now.ToShortDateString(), " ", DateTime.Now.ToString("HH:mm:ss fff")); }

        static string ArgsToArg(string[] args)
        { return string.Join(' ', args); }

        private static SearchTypes.Searchbase LookupSearchBase(ArgumentsObject.ArgumentsObject arguments)
        {
            switch (arguments.SearchType)
            {
                case ArgumentsObject.ArgumentsObject.eSearchType.Threaded:
                    return new SearchTypes.MultiThread(arguments);
                case ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex:
                    return new SearchTypes.InternalIndex(arguments);
                case ArgumentsObject.ArgumentsObject.eSearchType.WindowsIndex:
                    return new SearchTypes.WindowsSearchIndex(arguments);
                case ArgumentsObject.ArgumentsObject.eSearchType.IndexGenerate:
                    return new SearchTypes.InternalIndexGenerate(arguments);
                case ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch:
                    return new SearchTypes.InternalGenerateAndIndex(arguments);
                default:
                    return new SearchTypes.SingleThread(arguments);
            }
        }

    }
}
