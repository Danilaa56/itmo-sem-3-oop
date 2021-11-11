using System;
using System.Collections.Generic;

namespace Banks.UI
{
    public class Cli : ICli
    {
        public string[] ReadCommand()
        {
            string line = null;
            while (line == null)
                line = Console.ReadLine();
            char[] chars = line.ToCharArray();

            var list = new List<string>();

            char[] charsCache = new char[chars.Length];
            int cacheIndex = 0;

            bool passSpace = false;

            foreach (char c in chars)
            {
                if (c == ' ' && !passSpace)
                {
                    if (cacheIndex != 0)
                    {
                        list.Add(new string(charsCache, 0, cacheIndex));
                        cacheIndex = 0;
                    }
                }
                else if (c == '"')
                {
                    passSpace = !passSpace;
                }
                else
                {
                    charsCache[cacheIndex++] = c;
                }
            }

            if (cacheIndex != 0)
                list.Add(new string(charsCache, 0, cacheIndex));

            return list.ToArray();
        }

        public string ReadLine()
        {
            return Console.ReadLine()?.Trim();
        }

        public void WriteLine(object message)
        {
            Console.WriteLine(message);
        }

        public void Write(object message)
        {
            Console.Write(message);
        }
    }
}