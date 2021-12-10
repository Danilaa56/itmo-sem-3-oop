using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Entities.Irrelevanters
{
    public class DummyIrrelevanter : IIrrelevanter
    {
        public List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints)
        {
            return new List<RestorePoint>();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DummyIrrelevanter)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        protected bool Equals(DummyIrrelevanter other)
        {
            return true;
        }
    }
}