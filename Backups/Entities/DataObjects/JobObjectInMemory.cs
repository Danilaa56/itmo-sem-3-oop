using System;
using System.Linq;
using Newtonsoft.Json;

namespace Backups.Entities.DataObjects
{
    public class JobObjectInMemory : IJobObject
    {
        public JobObjectInMemory(byte[] data, string fileName)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            BackupObject = new BackupObject(fileName);
        }

        [JsonConstructor]
        public JobObjectInMemory(byte[] data, BackupObject backupObject)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            BackupObject = backupObject ?? throw new ArgumentNullException(nameof(backupObject));
        }

        public byte[] Data { get; }
        public BackupObject BackupObject { get; }

        public byte[] GetData()
        {
            return Data.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((JobObjectInMemory)obj);
        }

        public override int GetHashCode()
        {
            return BackupObject.GetHashCode();
        }

        protected bool Equals(JobObjectInMemory other)
        {
            return Data.SequenceEqual(other.Data) && Equals(BackupObject, other.BackupObject);
        }
    }
}