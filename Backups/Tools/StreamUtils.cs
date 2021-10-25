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

        public static RepositoryRemote.Action ReadAction(Stream stream)
        {
            return (RepositoryRemote.Action) stream.ReadByte();
        }

        public static void WriteAction(Stream stream, RepositoryRemote.Action action)
        {
            stream.WriteByte((byte) action);
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
    }
}