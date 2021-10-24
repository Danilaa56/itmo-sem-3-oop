using Backups.Entities;

namespace Backups.Server
{
    public class RepositoryRemote : Repository
    {
        public enum Action : byte
        {
            CREATE_STORAGE
        }
    }
}