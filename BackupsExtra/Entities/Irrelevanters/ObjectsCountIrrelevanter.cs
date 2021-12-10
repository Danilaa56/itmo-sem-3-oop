using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Entities;

namespace BackupsExtra.Entities.Irrelevanters
{
    public class ObjectsCountIrrelevanter : IIrrelevanter
    {
        public ObjectsCountIrrelevanter(int maxObjectsCount)
        {
            if (maxObjectsCount < 1)
                throw new ArgumentException("Max objects count must be positive", nameof(maxObjectsCount));
            MaxObjectsCount = maxObjectsCount;
        }

        public int MaxObjectsCount { get; }

        public List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints)
        {
            var irrelevant = restorePoints.ToList();
            irrelevant.Reverse();
            int objectsLimit = MaxObjectsCount;

            while (irrelevant.Count > 0)
            {
                int objectsCount = irrelevant[0].Storages.SelectMany(storage => storage.JobObjects).Count();
                if (objectsCount > objectsLimit)
                    break;
                objectsLimit -= objectsCount;
                irrelevant.RemoveAt(0);
            }

            irrelevant.Reverse();
            return irrelevant;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ObjectsCountIrrelevanter)obj);
        }

        public override int GetHashCode()
        {
            return MaxObjectsCount;
        }

        protected bool Equals(ObjectsCountIrrelevanter other)
        {
            return MaxObjectsCount == other.MaxObjectsCount;
        }
    }
}