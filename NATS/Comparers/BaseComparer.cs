using System;
using System.IO;
namespace NATS.Comparers
{
    public abstract class baseComparer
    {
        internal const int TenMB = 1048576;
        public abstract Tuple<Boolean, string> Compare(FileInfo File, string keyword);
        internal static Boolean HasRecord(string Text, string keyword) { return (Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1); }
    }
}