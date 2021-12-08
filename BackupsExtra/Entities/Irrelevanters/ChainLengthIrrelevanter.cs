using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities;

namespace BackupsExtra.Entities.Irrelevanters
{
    public class ChainLengthIrrelevanter : IIrrelevanter
    {
        public ChainLengthIrrelevanter(int maxChainLength)
        {
            if (maxChainLength < 1)
                throw new ArgumentException("Max chain length must be positive", nameof(maxChainLength));
            MaxChainLength = maxChainLength;
        }

        public int MaxChainLength { get; }

        public List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints)
        {
            RestorePoint[] existingPoints = restorePoints.ToArray();
            int irrelevantCount = existingPoints.Length - MaxChainLength;
            var irrelevant = new List<RestorePoint>();
            for (int i = 0; i < irrelevantCount; i++)
            {
                irrelevant.Add(existingPoints[i]);
            }

            return irrelevant;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ChainLengthIrrelevanter)obj);
        }

        public override int GetHashCode()
        {
            return MaxChainLength;
        }

        protected bool Equals(ChainLengthIrrelevanter other)
        {
            return MaxChainLength == other.MaxChainLength;
        }
    }
}