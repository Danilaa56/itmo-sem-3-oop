using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using Backups.Tools;

namespace Backups.Entities.Repository
{
    public class RepositoryInMemory : IRepository
    {
        private readonly Dictionary<string, byte[]> _storages = new ();

        public RepositoryInMemory()
        {
        }

        [JsonConstructor]
        private RepositoryInMemory(Dictionary<string, byte[]> storages)
            : this()
        {
            if (storages is null)
                throw new ArgumentNullException(nameof(storages));
            _storages = new Dictionary<string, byte[]>(storages);
        }

        public string CreateStorage(byte[] data)
        {
            string storageId;
            do
            {
                storageId = StringUtils.RandomHexString(16);
            }
            while (_storages.ContainsKey(storageId));

            _storages[storageId] = data.ToArray();

            return storageId;
        }

        public void RemoveStorage(string storageId)
        {
            if (storageId is null)
                throw new ArgumentNullException(nameof(storageId));
            if (!_storages.Remove(storageId))
                throw new BackupException("There is no storage with such storage id");
        }

        public byte[] ReadStorage(string storageId)
        {
            if (storageId is null)
                throw new ArgumentNullException(nameof(storageId));
            if (!_storages.TryGetValue(storageId, out byte[] data))
                throw new BackupException("There is no storage with such storage id");
            return data.ToArray();
        }

        public ImmutableArray<string> GetStorages()
        {
            return _storages.Keys.ToImmutableArray();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RepositoryInMemory)obj);
        }

        public override int GetHashCode()
        {
            return _storages.GetHashCode();
        }

        protected bool Equals(RepositoryInMemory other)
        {
            return _storages.SequenceEqual(other._storages);
        }
    }
}