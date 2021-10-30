using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities.Repository;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private readonly HashSet<JobObject> _files = new HashSet<JobObject>();
        private readonly IRepository _repository;
        private readonly List<RestorePoint> _restorePoints = new List<RestorePoint>();
        private StorageType _activeStorageType;

        public BackupJob(IRepository repository, StorageType storageType)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _activeStorageType = storageType;
        }

        public void Add(JobObject jobObject)
        {
            if (!_files.Add(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("This object is already added");
        }

        public void Remove(JobObject jobObject)
        {
            if (!_files.Remove(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("There is no such object in this Job");
        }

        public RestorePoint CreateRestorePoint()
        {
            long date = DateUtils.CurrentTimeMillis();

            var storages = _activeStorageType.PackJobObjects(_files).Select(data => _repository.CreateStorage(data))
                .ToList();
            var restorePoint = new RestorePoint(date, storages);
            _restorePoints.Add(restorePoint);
            return restorePoint;
        }

        public ImmutableList<RestorePoint> GetRestorePoints()
        {
            return _restorePoints.ToImmutableList();
        }

        public void SetStorageType(StorageType storageType)
        {
            _activeStorageType = storageType;
        }
    }
}