using System;
using System.Linq;

namespace Backups.Entities
{
    public abstract class Repository
    {
        private const string Hex = "0123456789abcdef";
        private readonly Random _random = new Random();

        public abstract string CreateStorage(byte[] data);

        protected string RandomHexString(int length)
        {
            return new string(Enumerable.Repeat(Hex, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}