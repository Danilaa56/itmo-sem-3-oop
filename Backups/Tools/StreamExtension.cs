using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Backups.Entities.Repository;

namespace Backups.Tools
{
    public static class StreamExtension
    {
        public static int ReadInt(this Stream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes);
        }

        public static void WriteInt(this Stream stream, int num)
        {
            stream.Write(BitConverter.GetBytes(num), 0, 4);
        }

        public static RepositoryRemote.ActionCode ReadAction(this Stream stream)
        {
            byte readCode = (byte)stream.ReadByte();
            if (!Enum.IsDefined(typeof(RepositoryRemote.ActionCode), readCode))
            {
                throw new BackupException("Unknown action code has passed");
            }

            return (RepositoryRemote.ActionCode)readCode;
        }

        public static void WriteAction(this Stream stream, RepositoryRemote.ActionCode actionCode)
        {
            stream.WriteByte((byte)actionCode);
        }

        public static byte[] ReadByteArray(this Stream stream)
        {
            byte[] bytes = new byte[ReadInt(stream)];
            int length = 0;
            while (length != bytes.Length)
                length += stream.Read(bytes, length, bytes.Length - length);
            return bytes;
        }

        public static void WriteByteArray(this Stream stream, byte[] array)
        {
            WriteInt(stream, array.Length);
            stream.Write(array);
        }

        public static string ReadString(this Stream stream)
        {
            byte[] bytes = ReadByteArray(stream);
            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteString(this Stream stream, string str)
        {
            WriteByteArray(stream, Encoding.UTF8.GetBytes(str));
        }

        public static ImmutableArray<string> ReadStringList(this Stream stream)
        {
            int length = ReadInt(stream);
            string[] array = new string[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ReadString(stream);
            }

            return array.ToImmutableArray();
        }

        public static void WriteStringList(this Stream stream, ICollection<string> strings)
        {
            WriteInt(stream, strings.Count);
            foreach (string str in strings)
                WriteString(stream, str);
        }
    }
}