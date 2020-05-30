using System.IO;
using System.Collections.Generic;
using NATS.SearchTypes;
using NATS.ArgumentsObject;
using System.Text;



namespace nats_standard
{
    public class Nats
    {

        public static string Helpfile()
        {
            return Properties.Resources.HelpFile;
        }
        public static string GUIHelpFile() { return Properties.Resources.GUIHelp; }

        public static string[] DefaultBlackList()
        {
            return Properties.Resources.DefaultBlacklist.Split('|');
                }


        public string OldSearch(ArgumentsObject ScanArg)
        {
            IEnumerable<FileInfo> Files = (new DirectoryInfo(ScanArg.DirectoryPath)).EnumerateFiles("*");
            Searchbase Search = LookupSearchBase(ScanArg);
            Search.Execute();
            return Search.ToString();      
        }

        private static Searchbase LookupSearchBase(ArgumentsObject arguments)
        {
            switch (arguments.SearchType)
            {
                case ArgumentsObject.eSearchType.Threaded:
                    return new MultiThread(arguments);
                case ArgumentsObject.eSearchType.LocalIndex:
                    return new InternalIndex(arguments);
                case ArgumentsObject.eSearchType.WindowsIndex:
                    return new WindowsSearchIndex(arguments);
                case ArgumentsObject.eSearchType.IndexGenerate:
                    return new InternalIndexGenerate(arguments);
                case ArgumentsObject.eSearchType.indexgenerateandsearch:
                    return new InternalGenerateAndIndex(arguments);
                default:
                    return new SingleThread(arguments);
            }
        }

    }
}
