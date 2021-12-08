using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities;

namespace BackupsExtra.Entities.IrrelevantResolvers
{
    public class RestorePointDeleter : IIrrelevantResolver
    {
        public List<RestorePoint> Resolve(
            IEnumerable<RestorePoint> existingPoints,
            IEnumerable<RestorePoint> irrelevantPoints)
        {
            return existingPoints.ToImmutableList().RemoveRange(irrelevantPoints).ToList();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RestorePointDeleter)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(RestorePointDeleter other)
        {
            return true;
        }
    }
}