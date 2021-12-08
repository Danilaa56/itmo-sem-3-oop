using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities;

namespace BackupsExtra.Entities.Irrelevanters
{
    public class HybridIrrelevanter : IIrrelevanter
    {
        public HybridIrrelevanter(Mode mode, List<IIrrelevanter> irrelevanters)
        {
            ActiveMode = mode;
            if (irrelevanters is null)
                throw new ArgumentNullException(nameof(irrelevanters));
            if (irrelevanters.Count < 1)
                throw new ArgumentException("Irrelevanters count must be positive", nameof(irrelevanters));

            Irrelevanters = irrelevanters.ToImmutableList();
        }

        public enum Mode
        {
            All,
            Any,
        }

        public Mode ActiveMode { get; }
        public ImmutableList<IIrrelevanter> Irrelevanters { get; }

        public List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints)
        {
            var results = Irrelevanters.Select(irrelevanter => irrelevanter.DefineIrrelevant(restorePoints)).ToList();
            var irrelevant = results[0].ToList();

            results.ForEach(result =>
            {
                switch (ActiveMode)
                {
                    case Mode.All:
                        irrelevant = irrelevant.Intersect(result).ToList();
                        break;
                    case Mode.Any:
                        irrelevant = irrelevant.Union(result).ToList();
                        break;
                }
            });

            return irrelevant;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HybridIrrelevanter)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)ActiveMode, Irrelevanters);
        }

        protected bool Equals(HybridIrrelevanter other)
        {
            return ActiveMode == other.ActiveMode && Irrelevanters.SequenceEqual(other.Irrelevanters);
        }
    }
}