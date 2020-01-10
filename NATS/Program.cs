using System;

namespace NATS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scanning...");
            Scanner scanner = new Scanner(ArgsToArg(args));
            Console.WriteLine(scanner.Scan());
        }

        static string ArgsToArg(string[] args)
        { return string.Join(' ', args); }

    }


    //@"-P C:\Users\liuko\source\repos -K if -S -M -O d:\1.txt -B targets|cs"
}
