using System;
using System.Collections.Generic;
using System.Text;
using NATS.ArgumentsObject;
using Nats_Cli_Core.Properties;

namespace Nats_Cli_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> HeaderItems = new List<string>();
            string consoleOutput;
            StringBuilder FilesFound = new StringBuilder();
            string fullArgs = ArgsToArg(args);

            ArgumentsObject ScanArg = new ArgumentsObject(fullArgs);
            if (!ScanArg.DisplayHelp)
            {
                consoleOutput = string.Concat( Resources.Name, Resources.Fin, ScanArg.KeywordSearch,
                   Resources.Fin, ScanArg.DirectoryPath, Environment.NewLine, "start timestamp:", GetDateTime(), Environment.NewLine, Resources.Line, Environment.NewLine);

                Console.WriteLine(consoleOutput);
                nats_standard.Nats nats = new nats_standard.Nats();        

                FilesFound.Append(nats.OldSearch(ScanArg));
                FilesFound.Append(Environment.NewLine).Append(Resources.Fin).Append(Environment.NewLine).Append("End timestamp:").Append(GetDateTime());

                if (!string.IsNullOrWhiteSpace(ScanArg.FileNameOutput))
                {
                    System.IO.File.WriteAllText(ScanArg.FileNameOutput, consoleOutput + FilesFound.ToString());
                    Console.WriteLine(Resources.Fin + Environment.NewLine + "End timestamp:" + GetDateTime());
                }
                else { Console.WriteLine(FilesFound.ToString()); }

            }
            else { Console.WriteLine(nats_standard.Nats.Helpfile()); }

            static string GetDateTime() { return string.Concat(DateTime.Now.ToShortDateString(), " ", DateTime.Now.ToString("HH:mm:ss fff")); }

            static string ArgsToArg(string[] args)
            { return string.Join(' ', args); }


        }
    }
}