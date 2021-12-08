using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using Backups.Tools;
using Newtonsoft.Json;

namespace Backups.Entities
{
    public class BackupJob
    {
        private HashSet<IJobObject> _jobObjects = new ();
        private List<RestorePoint> _restorePoints = new ();

        private IObjectDistributor _objectDistributor = new SingleStorageDistributor();
        private IStoragePacker _storagePacker = new ZipStoragePacker();
        private IRepository _repository;

        public BackupJob(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [JsonConstructor]
        private BackupJob(
            HashSet<IJobObject> jobObjects,
            List<RestorePoint> restorePoints,
            IObjectDistributor objectDistributor,
            IStoragePacker storagePacker,
            IRepository repository)
        {
            if (jobObjects is null) throw new ArgumentNullException(nameof(jobObjects));
            _jobObjects = new HashSet<IJobObject>(jobObjects);
            if (restorePoints is null) throw new ArgumentNullException(nameof(restorePoints));
            _restorePoints = new List<RestorePoint>(restorePoints);
            ObjectDistributor = objectDistributor;
            StoragePacker = storagePacker;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public HashSet<IJobObject> JobObjects => _jobObjects.ToHashSet();

        public List<RestorePoint> RestorePoints
        {
            get => _restorePoints.ToList();
            protected set
            {
                if (value is null)
                    throw new ArgumentNullException();
                _restorePoints.Clear();
                _restorePoints.AddRange(value);
            }
        }

        public IObjectDistributor ObjectDistributor
        {
            get => _objectDistributor;
            set => _objectDistributor = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IStoragePacker StoragePacker
        {
            get => _storagePacker;
            set => _storagePacker = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IRepository Repository => _repository;

        public void Add(IJobObject jobObject)
        {
            if (!_jobObjects.Add(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("This object is already added");
        }

        public void Remove(IJobObject jobObject)
        {
            if (!_jobObjects.Remove(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("There is no such object in this Job");
        }

        public virtual RestorePoint CreateRestorePoint()
        {
            long date = DateUtils.CurrentTimeMillis();

            List<StorageBlank> storageBlanks = _objectDistributor.DistributeJobObjects(_jobObjects);
            List<Storage> storages = storageBlanks.ConvertAll(storageBlank =>
            {
                byte[] data = _storagePacker.Pack(storageBlank);
                string storageId = _repository.CreateStorage(data);
                return new Storage(storageId, _repository, storageBlank.JobObjects);
            });

            var restorePoint = new RestorePoint(date, storages, _objectDistributor, _storagePacker, _repository);
            _restorePoints.Add(restorePoint);

            return _restorePoints.Last();
        }

        public ImmutableList<RestorePoint> GetRestorePoints()
        {
            return _restorePoints.ToImmutableList();
        }

        public void SetRestorePoints(IEnumerable<RestorePoint> restorePoints)
        {
            if (restorePoints is null)
                throw new ArgumentNullException(nameof(restorePoints));
            _restorePoints.Clear();
            _restorePoints.AddRange(restorePoints);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BackupJob)obj);
        }

        protected bool Equals(BackupJob other)
        {
            return _jobObjects.SetEquals(other._jobObjects) && _restorePoints.SequenceEqual(other._restorePoints) &&
                   Equals(_objectDistributor, other._objectDistributor) &&
                   Equals(_storagePacker, other._storagePacker) && Equals(_repository, other._repository);
        }
    }
}