using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NATS.ArgumentsObject;
using NATS.Filters;
using System.Collections.Concurrent;
using NATS.Properties;

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
            ArgumentsObject.ArgumentsObject ScanArg = new ArgumentsObject.ArgumentsObject(fullArgs);
            if (!ScanArg.DisplayHelp)
            {
                 consoleOutput = string.Concat(Resources.Name, Resources.Seperator, ScanArg.KeywordSearch, 
                    Resources.Seperator, ScanArg.DirectoryPath, Environment.NewLine, "start timestamp:", GetDateTime(), Environment.NewLine, Resources.Line, Environment.NewLine);

                Console.WriteLine(consoleOutput);

                IEnumerable<FileInfo> Files = (new DirectoryInfo(ScanArg.DirectoryPath)).EnumerateFiles("*", ScanArg.EOptions);


                SearchTypes.Searchbase Search;

                switch (ScanArg.SearchType)
                {

                    case ArgumentsObject.ArgumentsObject.eSearchType.Threaded:
                        Search = new SearchTypes.MultiThread(ScanArg); break;
                    case ArgumentsObject.ArgumentsObject.eSearchType.WindowsIndex: 
                        Search = new SearchTypes.WindowsSearchIndex(ScanArg);
                        break;
                    case ArgumentsObject.ArgumentsObject.eSearchType.IndexGenerate:
                        Search = new SearchTypes.InternalIndexGenerate(ScanArg);
                        break;
                    case ArgumentsObject.ArgumentsObject.eSearchType.LocalIndex:
                        Search = new SearchTypes.InternalIndex(ScanArg);
                        break;
                    case ArgumentsObject.ArgumentsObject.eSearchType.indexgenerateandsearch:
                        Search = new SearchTypes.InternalGenerateAndIndex(ScanArg);
                        break;
                    default:
                        Search = new SearchTypes.SingleThread(ScanArg); break;
                }
                Search.Execute();
                FilesFound.Append(Search.ToString());


                FilesFound.Append(Resources.Fin + Environment.NewLine + "End timestamp:" + GetDateTime()) ;

                if (!string.IsNullOrWhiteSpace(ScanArg.FileNameOutput))
                {
                    System.IO.File.WriteAllText(ScanArg.FileNameOutput, consoleOutput + FilesFound.ToString());
                    Console.WriteLine(Resources.Fin  + Environment.NewLine + "End timestamp:" + GetDateTime());
                }
                else { Console.WriteLine(FilesFound.ToString()); }

            }
            else { Console.WriteLine(NATS.Properties.Resources.Help); }
        }

        static string GetDateTime() { return string.Concat(DateTime.Now.ToShortDateString(), " ", DateTime.Now.ToString("HH:mm:ss fff")); }

        static string ArgsToArg(string[] args)
        { return string.Join(' ', args); }

       
    }
}
