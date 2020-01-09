using System;

namespace NATS
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner(ArgsToArg(args));
            Console.WriteLine(scanner.Scan());
        }

        static string ArgsToArg(string[] args)
        { return string.Join(' ', args); }

    }



}
