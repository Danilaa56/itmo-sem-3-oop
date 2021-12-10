using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public RestorePoint(
            long creationDateUtc,
            IEnumerable<Storage> storages,
            IObjectDistributor objectDistributor,
            IStoragePacker storagePacker,
            IRepository repository)
        {
            CreationDateUtc = creationDateUtc;
            if (storages is null) throw new ArgumentNullException(nameof(storages));
            Storages = storages.ToImmutableList();
            ObjectDistributor = objectDistributor ?? throw new ArgumentNullException(nameof(objectDistributor));
            StoragePacker = storagePacker ?? throw new ArgumentNullException(nameof(storagePacker));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public long CreationDateUtc { get; }
        public ImmutableList<Storage> Storages { get; }
        public IObjectDistributor ObjectDistributor { get; }
        public IStoragePacker StoragePacker { get; }
        public IRepository Repository { get; }

        public void Restore(string destination)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            Storages
                .Select(storage => Repository.ReadStorage(storage.Id))
                .Select(storageBytes => StoragePacker.Unpack(storageBytes))
                .SelectMany(storageBlank => storageBlank.JobObjects).ToList()
                .ForEach(jobObject => WriteJobObject(jobObject, destination));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RestorePoint)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CreationDateUtc, Storages, ObjectDistributor, StoragePacker);
        }

        protected bool Equals(RestorePoint other)
        {
            return CreationDateUtc == other.CreationDateUtc
                   && Storages.SequenceEqual(other.Storages)
                   && ObjectDistributor.Equals(other.ObjectDistributor)
                   && StoragePacker.Equals(other.StoragePacker);
        }

        private void WriteJobObject(IJobObject jobObject, string destination)
        {
            var fileInfo = new FileInfo(Path.Combine(destination, jobObject.BackupObject.Name));
            if (fileInfo.Directory is not null)
                fileInfo.Directory.Create();
            File.WriteAllBytes(fileInfo.FullName, jobObject.GetData());
        }
    }
}