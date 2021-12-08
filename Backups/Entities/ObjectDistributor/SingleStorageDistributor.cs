using System.Collections.Generic;
using Backups.Entities.DataObjects;

namespace Backups.Entities.ObjectDistributor
{
    public class SingleStorageDistributor : IObjectDistributor
    {
        public List<StorageBlank> DistributeJobObjects(IEnumerable<IJobObject> jobObjects)
        {
            return new List<StorageBlank> { new StorageBlank(jobObjects) };
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SingleStorageDistributor)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(SingleStorageDistributor other)
        {
            return true;
        }
    }
}