using System;
using System.Collections.Immutable;
using Backups.Entities.Repository;

namespace BackupsExtra.Tests
{
    public class RepositoryMock : IRepository
    {
        public string CreateStorage(byte[] data)
        {
            return Guid.NewGuid().ToString();
        }

        public void RemoveStorage(string storageId)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadStorage(string storageId)
        {
            throw new NotImplementedException();
        }

        public ImmutableArray<string> GetStorages()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RepositoryMock)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(RepositoryMock other)
        {
            return true;
        }
    }
}