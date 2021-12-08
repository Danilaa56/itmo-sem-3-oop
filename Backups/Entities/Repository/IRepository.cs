using System.Collections.Immutable;

namespace Backups.Entities.Repository
{
    public interface IRepository
    {
        string CreateStorage(byte[] data);
        void RemoveStorage(string storageId);
        byte[] ReadStorage(string storageId);

        ImmutableArray<string> GetStorages();
    }
}