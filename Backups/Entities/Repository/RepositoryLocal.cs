using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Backups.Tools;

namespace Backups.Entities.Repository
{
    public class RepositoryLocal : IRepository
    {
        public RepositoryLocal(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(Path))
            {
                try
                {
                    Directory.CreateDirectory(Path);
                }
                catch (IOException e)
                {
                    throw new BackupException("Failed to create dir for repository", e);
                }
            }
        }

        [JsonConstructor]
        private RepositoryLocal(string path, List<string> storages)
            : this(path)
        {
            if (storages is null)
                throw new ArgumentNullException(nameof(storages));
            Storages = new List<string>(storages);
        }

        public string Path { get; init; }
        public List<string> Storages { get; init; } = new ();

        public string CreateStorage(byte[] data)
        {
            string storageId;
            string fullName;
            do
            {
                storageId = StringUtils.RandomHexString(16);
                fullName = Path + System.IO.Path.DirectorySeparatorChar.ToString() + storageId;
            }
            while (File.Exists(fullName));

            File.WriteAllBytes(fullName, data);
            Storages.Add(storageId);
            return storageId;
        }

        public void RemoveStorage(string storageId)
        {
            if (storageId is null)
                throw new ArgumentNullException(nameof(storageId));
            if (!Storages.Remove(storageId))
                throw new BackupException("There is no storage with such storage id");
            File.Delete(Path + System.IO.Path.DirectorySeparatorChar.ToString() + storageId);
        }

        public byte[] ReadStorage(string storageId)
        {
            if (storageId is null)
                throw new ArgumentNullException(nameof(storageId));
            if (!Storages.Contains(storageId))
                throw new BackupException("There is no storage with such storage id");
            return File.ReadAllBytes(Path + System.IO.Path.DirectorySeparatorChar.ToString() + storageId);
        }

        public ImmutableArray<string> GetStorages()
        {
            return Storages.ToImmutableArray();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RepositoryLocal)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path, Storages);
        }

        protected bool Equals(RepositoryLocal other)
        {
            return Equals(Path, other.Path) && Storages.SequenceEqual(other.Storages);
        }
    }
}