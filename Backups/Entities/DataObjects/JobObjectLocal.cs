using System;
using System.IO;
using Newtonsoft.Json;

namespace Backups.Entities.DataObjects
{
    public class JobObjectLocal : IJobObject
    {
        public JobObjectLocal(string rootPath, string fileName)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            BackupObject = new BackupObject(fileName);
        }

        [JsonConstructor]
        public JobObjectLocal(string rootPath, BackupObject backupObject)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            BackupObject = backupObject ?? throw new ArgumentNullException(nameof(backupObject));
        }

        public string RootPath { get; }
        public BackupObject BackupObject { get; }

        public byte[] GetData()
        {
            return File.ReadAllBytes(RootPath + Path.DirectorySeparatorChar + BackupObject.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(JobObjectLocal)) return false;
            return Equals((JobObjectLocal)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BackupObject, RootPath);
        }

        protected bool Equals(JobObjectLocal other)
        {
            return Equals(BackupObject, other.BackupObject) && RootPath == other.RootPath;
        }
    }
}