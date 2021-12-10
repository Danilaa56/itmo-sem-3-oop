using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;
using Backups.Tools;

namespace BackupsExtra.Entities.Irrelevanters
{
    public class DateIrrelevanter : IIrrelevanter
    {
        public DateIrrelevanter(long storageTimeMs)
        {
            if (storageTimeMs < 0)
                throw new ArgumentException("Storage time cannot be negative", nameof(storageTimeMs));
            StorageTimeMs = storageTimeMs;
        }

        public long StorageTimeMs { get; }

        public List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints)
        {
            long currentTime = DateUtils.CurrentTimeMillis();
            return restorePoints.ToList()
                .FindAll(restorePoint => currentTime - restorePoint.CreationDateUtc > StorageTimeMs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DateIrrelevanter)obj);
        }

        public override int GetHashCode()
        {
            return StorageTimeMs.GetHashCode();
        }

        protected bool Equals(DateIrrelevanter other)
        {
            return StorageTimeMs == other.StorageTimeMs;
        }
    }
}