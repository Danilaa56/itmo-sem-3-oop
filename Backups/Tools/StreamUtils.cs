using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Backups.Entities;

namespace Backups.Tools
{
    public static class StreamUtils
    {
        public static int ReadInt(Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            return bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24);
        }

        public static void WriteInt(Stream stream, int num)
        {
            byte[] bytes = { (byte)num, (byte)(num >> 8), (byte)(num >> 16), (byte)(num >> 24) };
            stream.Write(bytes, 0, 4);
        }

        public static RepositoryRemote.ActionCode ReadAction(Stream stream)
        {
            return (RepositoryRemote.ActionCode)stream.ReadByte();
        }

        public static void WriteAction(Stream stream, RepositoryRemote.ActionCode actionCode)
        {
            stream.WriteByte((byte)actionCode);
        }

        public static byte[] ReadByteArray(Stream stream)
        {
            byte[] bytes = new byte[ReadInt(stream)];
            int length = 0;
            while (length != bytes.Length)
                length += stream.Read(bytes, length, bytes.Length - length);
            return bytes;
        }

        public static void WriteByteArray(Stream stream, byte[] array)
        {
            WriteInt(stream, array.Length);
            stream.Write(array);
        }

        public static string ReadString(Stream stream)
        {
            byte[] bytes = ReadByteArray(stream);
            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteString(Stream stream, string str)
        {
            WriteByteArray(stream, Encoding.UTF8.GetBytes(str));
        }

        public static ImmutableArray<string> ReadStringList(Stream stream)
        {
            int length = ReadInt(stream);
            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadString(stream);
            }

            return array.ToImmutableArray();
        }

        public static void WriteStringList(Stream stream, ICollection<string> strings)
        {
            WriteInt(stream, strings.Count);
            foreach (string str in strings)
                WriteString(stream, str);
        }
    }
}