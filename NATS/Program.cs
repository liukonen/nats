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
        //@"-P C:\Users\liuko\source\repos -K if -S -M -O d:\1.txt -B targets|cs"
        static void Main(string[] args)
        {
            List<string> HeaderItems = new List<string>();
            string consoleOutput;
            StringBuilder FilesFound = new StringBuilder();
            string fullArgs = @"-P D:\choco -K return -S -T";//ArgsToArg(args);

            ArgumentsObject.ArgumentsObject ScanArg = new ArgumentsObject.ArgumentsObject(fullArgs);
            if (!ScanArg.DisplayHelp)
            {
                 consoleOutput = string.Concat(Resources.Name, Resources.Seperator, ScanArg.KeywordSearch, 
                    Resources.Seperator, ScanArg.DirectoryPath, Environment.NewLine, "start timestamp:", GetDateTime(), Environment.NewLine, Resources.Line, Environment.NewLine);

                Console.WriteLine(consoleOutput);

                IEnumerable<FileInfo> Files = (new DirectoryInfo(ScanArg.DirectoryPath)).EnumerateFiles("*", ScanArg.EOptions);

                if (ScanArg.SearchType == ArgumentsObject.ArgumentsObject.eSearchType.Single)
                {
                    foreach (FileInfo item in Files)
                    {
                        var Response = CheckFile(item, ScanArg);
                        if (Response.Item1) { FilesFound.Append(Response.Item2).Append(Environment.NewLine); }
                    }
                }
                else
                {
                    ConcurrentBag<string> ReturnItems = new ConcurrentBag<string>();
                    ParallelOptions Options = new ParallelOptions() { MaxDegreeOfParallelism = ScanArg.ThreadCount };
                    Parallel.ForEach(Files, Options, (currentFile) =>
                    {
                        var response = CheckFile(currentFile, ScanArg);
                        if (response.Item1) { ReturnItems.Add(response.Item2); }
                    });
                    FilesFound.Append(string.Join(Environment.NewLine, from string X in ReturnItems select X));
                }
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

        static Tuple<bool, string> CheckFile(FileInfo item, ArgumentsObject.ArgumentsObject Object)
        {
            var NotValid = (from NATS.Filters.FileInfoFilters F in Object.FileInfoFilters where F.IsValid(item) == false select F).Any();
            if (!NotValid) //double negative... could word it better
            {
                return Object.Comparer.Compare(item, Object.KeywordSearch);
            }
            return new Tuple<bool, string>(false, string.Empty);
        }
    }
}
