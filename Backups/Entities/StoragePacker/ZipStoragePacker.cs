using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.Entities.DataObjects;
using Backups.Tools;

namespace Backups.Entities.StoragePacker
{
    public class ZipStoragePacker : IStoragePacker
    {
        public byte[] Pack(StorageBlank storageBlank)
        {
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Update))
            {
                foreach (IJobObject jobObject in storageBlank.JobObjects)
                {
                    ZipArchiveEntry orderEntry = archive.CreateEntry(jobObject.BackupObject.Name);
                    using Stream writer = orderEntry.Open();
                    writer.Write(jobObject.GetData());
                }
            }

            return ms.ToArray();
        }

        public StorageBlank Unpack(byte[] packedData)
        {
            using var ms = new MemoryStream(packedData);

            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
            {
                var jobObjects = archive.Entries.Select(entry =>
                {
                    using Stream entryStream = entry.Open();
                    return new JobObjectInMemory(entryStream.ReadAllBytes(), entry.Name);
                }).ToImmutableList();
                return new StorageBlank(jobObjects);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ZipStoragePacker)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(ZipStoragePacker other)
        {
            return true;
        }
    }
}