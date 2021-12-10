using System;
using Backups.Entities;
using Backups.Entities.DataObjects;
using Backups.Entities.Repository;

namespace BackupsExtra.Entities
{
    public class BackupObjectPath
    {
        public BackupObjectPath(
            IRepository repository,
            RestorePoint restorePoint,
            Storage storage,
            IJobObject jobObject,
            BackupObject backupObject)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            RestorePoint = restorePoint ?? throw new ArgumentNullException(nameof(restorePoint));
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            JobObject = jobObject ?? throw new ArgumentNullException(nameof(jobObject));
            BackupObject = backupObject ?? throw new ArgumentNullException(nameof(backupObject));
        }

        public IRepository Repository { get; private init; }
        public RestorePoint RestorePoint { get; private init; }
        public Storage Storage { get; private init; }
        public IJobObject JobObject { get; private init; }
        public BackupObject BackupObject { get; private init; }
    }
}