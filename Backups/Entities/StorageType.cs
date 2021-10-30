using System.Collections.Generic;
using Backups.Entities.StorageTypeImpl;

namespace Backups.Entities
{
    public abstract class StorageType
    {
        public static StorageType SingleStorage { get; } = new StorageTypeSingle();
        public static StorageType SplitStorage { get; } = new StorageTypeSplit();

        public abstract IEnumerable<byte[]> PackJobObjects(IEnumerable<JobObject> jobObjects);
    }
}