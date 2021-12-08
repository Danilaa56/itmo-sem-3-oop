using System.Collections.Generic;
using System.Linq;
using Backups.Entities.DataObjects;

namespace Backups.Entities.ObjectDistributor
{
    public class SplitStorageDistributor : IObjectDistributor
    {
        public List<StorageBlank> DistributeJobObjects(IEnumerable<IJobObject> jobObjects)
        {
            return jobObjects
                .Select(jobObject => new StorageBlank(new List<IJobObject> { jobObject }))
                .ToList();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SplitStorageDistributor)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(SplitStorageDistributor other)
        {
            return true;
        }
    }
}