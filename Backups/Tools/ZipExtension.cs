using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backups.Tools
{
    public static class ZipExtension
    {
        public static byte[] Zip(this IEnumerable<NamedData> namedData)
        {
            using var ms = new MemoryStream();
            {
                using var archive = new ZipArchive(ms, ZipArchiveMode.Update);
                {
                    foreach (NamedData namedDataSample in namedData)
                    {
                        ZipArchiveEntry orderEntry = archive.CreateEntry(namedDataSample.Name);
                        using var writer = new BinaryWriter(orderEntry.Open());
                        writer.Write(namedDataSample.Data.ToArray());
                    }
                }
            }

            return ms.ToArray();
        }
    }
}