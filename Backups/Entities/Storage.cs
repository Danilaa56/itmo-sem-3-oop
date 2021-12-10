using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities.DataObjects;
using Backups.Entities.Repository;

namespace Backups.Entities
{
    public class Storage
    {
        public Storage(string id, IRepository repository, IEnumerable<IJobObject> jobObjects)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            JobObjects = jobObjects.ToImmutableList();
        }

        public string Id { get; }
        public IRepository Repository { get; }
        public ImmutableList<IJobObject> JobObjects { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Storage)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, JobObjects);
        }

        protected bool Equals(Storage other)
        {
            return Id == other.Id && JobObjects.SequenceEqual(other.JobObjects);
        }
    }
}