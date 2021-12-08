using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities.DataObjects;

namespace Backups.Entities
{
    public class StorageBlank
    {
        public StorageBlank(IEnumerable<IJobObject> jobObjects)
        {
            JobObjects = jobObjects.ToImmutableList();
        }

        public ImmutableList<IJobObject> JobObjects { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StorageBlank)obj);
        }

        public override int GetHashCode()
        {
            return JobObjects.GetHashCode();
        }

        protected bool Equals(StorageBlank other)
        {
            return JobObjects.SequenceEqual(other.JobObjects);
        }
    }
}