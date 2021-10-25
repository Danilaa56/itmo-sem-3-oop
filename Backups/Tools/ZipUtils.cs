using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Backups.Tools
{
    public class ZipUtils
    {
        public static byte[] Zip(Dictionary<string, byte[]> filesInfo)
        {
            using var ms = new MemoryStream();
            {
                using var archive = new ZipArchive(ms, ZipArchiveMode.Update);
                {
                    foreach ((string fileName, byte[] data) in filesInfo)
                    {
                        ZipArchiveEntry orderEntry = archive.CreateEntry(fileName);
                        using var writer = new BinaryWriter(orderEntry.Open());
                        writer.Write(data);
                    }
                }
            }

            return ms.ToArray();
        }
    }
}