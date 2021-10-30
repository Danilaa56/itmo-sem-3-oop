using System.Collections.Immutable;

namespace Backups.Entities.Repository
{
    public interface IRepository
    {
        string CreateStorage(byte[] data);

        ImmutableArray<string> GetStorages();
    }
}