using System;
using System.Linq;

namespace Backups.Entities
{
    public abstract class Repository
    {

        public abstract string CreateStorage(byte[] data);

        private const string hex = "0123456789abcdef";
        private readonly Random random = new Random();
        protected string randomHexString(int length)
        {
            return new string(Enumerable.Repeat(hex, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}