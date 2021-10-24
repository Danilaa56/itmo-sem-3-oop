using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Tools;

namespace Backups.Entities
{
    public class BackupJob
    {
        private readonly HashSet<JobObject> files = new HashSet<JobObject>();
        private StorageType activeStorageType = StorageType.SINGLE_STORAGE;
        private Repository repository;

        public BackupJob(Repository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Add(JobObject jobObject)
        {
            if (!files.Add(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("This object is already added");
        }

        public void Remove(JobObject jobObject)
        {
            if (!files.Remove(jobObject ?? throw new ArgumentNullException(nameof(jobObject))))
                throw new ArgumentException("There is no such object in this Job");
        }

        public RestorePoint CreateRestorePoint()
        {
            long date = DateUtils.CurrentTimeMillis();

            HashSet<string> storages = StoragesFromJobObjects();
            var restorePoint = new RestorePoint(date, storages);
            return restorePoint;
        }

        public void SetStorageType(StorageType storageType)
        {
            activeStorageType = storageType;
        }

        private HashSet<string> StoragesFromJobObjects()
        {
            var storageIds = new HashSet<string>();

            var filesInfo = new Dictionary<string, byte[]>();
            switch (activeStorageType)
            {
                case StorageType.SPLIT_STORAGES:
                    foreach (var jobObject in files)
                    {
                        filesInfo.Clear();
                        filesInfo[jobObject.FileName] = jobObject.GetData();
                        storageIds.Add(repository.CreateStorage(zip(filesInfo)));
                    }

                    break;
                case StorageType.SINGLE_STORAGE:
                    filesInfo.Clear();
                    foreach (var jobObject in files)
                    {
                        filesInfo[jobObject.FileName] = jobObject.GetData();
                    }

                    storageIds.Add(repository.CreateStorage(zip(filesInfo)));
                    break;
            }

            return storageIds;
        }

        private byte[] zip(Dictionary<string, byte[]> filesInfo)
        {
            using var ms = new MemoryStream();
            using var archive = new ZipArchive(ms, ZipArchiveMode.Update);
            foreach (var fileInfo in filesInfo)
            {
                ZipArchiveEntry orderEntry = archive.CreateEntry("/" + fileInfo.Key);
                using var writer = new BinaryWriter(orderEntry.Open());
                writer.Write(fileInfo.Value);
            }

            archive.Dispose();

            return ms.ToArray();
        }
    }
}