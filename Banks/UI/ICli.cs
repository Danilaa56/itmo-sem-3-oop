using System;

namespace Banks.UI
{
    public interface ICli
    {
        string ReadLine();
        string[] ReadCommand();
        void WriteLine(object message);
        void Write(object message);

        public virtual bool Read<T>(Func<string, T> parser, out T value)
        {
            value = default;
            while (true)
            {
                string input = ReadLine();
                if (string.Empty.Equals(input))
                    return false;
                try
                {
                    value = parser.Invoke(input);
                    return true;
                }
                catch (Exception e)
                {
                    WriteLine(e);
                }
            }
        }

        public virtual bool Read(out decimal value)
        {
            return Read(decimal.Parse, out value);
        }

        public virtual bool Read(out int value)
        {
            return Read(int.Parse, out value);
        }

        public virtual bool Read(out long value)
        {
            return Read(long.Parse, out value);
        }

        public virtual bool Read(out string value)
        {
            return Read(str => str, out value);
        }
    }
}