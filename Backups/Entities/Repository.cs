using System.Collections.Immutable;

namespace Backups.Entities
{
    public abstract class Repository
    {
        public abstract string CreateStorage(byte[] data);

        public abstract ImmutableArray<string> GetStorages();
    }
}