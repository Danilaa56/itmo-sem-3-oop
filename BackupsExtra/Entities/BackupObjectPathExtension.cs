using System.Linq;
using Backups.Entities;
using Backups.Entities.DataObjects;

namespace BackupsExtra.Entities
{
    public static class BackupObjectPathExtension
    {
        public static IJobObject ReadJobObject(this BackupObjectPath backupObjectPath)
        {
            byte[] storage = backupObjectPath.Repository.ReadStorage(backupObjectPath.Storage.Id);
            StorageBlank storageBlank = backupObjectPath.RestorePoint.StoragePacker.Unpack(storage);
            return storageBlank.JobObjects.First(jobObject =>
                jobObject.BackupObject.Equals(backupObjectPath.BackupObject));
        }

        public static byte[] ReadBytes(this BackupObjectPath backupObjectPath)
        {
            byte[] storage = backupObjectPath.Repository.ReadStorage(backupObjectPath.Storage.Id);
            StorageBlank storageBlank = backupObjectPath.RestorePoint.StoragePacker.Unpack(storage);
            IJobObject jobObject = storageBlank.JobObjects.First(jobObject =>
                jobObject.BackupObject.Equals(backupObjectPath.BackupObject));
            return jobObject.GetData();
        }
    }
}