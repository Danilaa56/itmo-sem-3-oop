using System.Collections.Immutable;

namespace Backups.Entities
{
    public interface IRepository
    {
        string CreateStorage(byte[] data);

        ImmutableArray<string> GetStorages();
    }
}