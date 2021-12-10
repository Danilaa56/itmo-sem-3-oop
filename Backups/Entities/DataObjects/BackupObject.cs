using System;

namespace Backups.Entities.DataObjects
{
    public class BackupObject
    {
        public BackupObject(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; private init; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BackupObject)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        protected bool Equals(BackupObject other)
        {
            return Name == other.Name;
        }
    }
}