using System;
using System.Linq;

namespace Backups.Tools
{
    public static class StringUtils
    {
        private const string Hex = "0123456789abcdef";
        private static readonly Random Random = new Random();

        public static string RandomHexString(int length)
        {
            return new string(Enumerable.Repeat(Hex, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}