using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private readonly HashSet<JobObject> _files = new HashSet<JobObject>();
        private readonly Repository _repository;
        private readonly List<RestorePoint> _restorePoints = new List<RestorePoint>();
        private StorageType _activeStorageType;

        public BackupJob(Repository repository, StorageType storageType = StorageType.SingleStorage)
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

            HashSet<string> storages = StoragesFromJobObjects();
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

        private static byte[] Zip(Dictionary<string, byte[]> filesInfo)
        {
            using var ms = new MemoryStream();
            {
                using var archive = new ZipArchive(ms, ZipArchiveMode.Update);
                {
                    foreach ((string fileName, byte[] data) in filesInfo)
                    {
                        ZipArchiveEntry orderEntry = archive.CreateEntry(fileName);
                        using var writer = new BinaryWriter(orderEntry.Open());
                        writer.Write(data);
                    }
                }
            }

            return ms.ToArray();
        }

        private HashSet<string> StoragesFromJobObjects()
        {
            var storageIds = new HashSet<string>();

            var filesInfo = new Dictionary<string, byte[]>();
            switch (_activeStorageType)
            {
                case StorageType.SplitStorages:
                    foreach (JobObject jobObject in _files)
                    {
                        filesInfo.Clear();
                        filesInfo[jobObject.FileName] = jobObject.GetData();
                        storageIds.Add(_repository.CreateStorage(Zip(filesInfo)));
                    }

                    break;
                case StorageType.SingleStorage:
                    filesInfo.Clear();
                    foreach (JobObject jobObject in _files)
                    {
                        filesInfo[jobObject.FileName] = jobObject.GetData();
                    }

                    storageIds.Add(_repository.CreateStorage(Zip(filesInfo)));
                    break;
            }

            return storageIds;
        }
    }
}