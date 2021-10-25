using System.Collections.Generic;

namespace Backups.Entities.StorageType
{
    public interface IStorageType
    {
        HashSet<string> StoragesFromJobObjects(HashSet<JobObject> files, IRepository repo);
    }
}