﻿using System.Collections.Generic;
using Backups.Tools;

namespace Backups.Entities.StorageType
{
    public class StorageTypeSplit : IStorageType
    {
        public HashSet<string> StoragesFromJobObjects(HashSet<JobObject> files, IRepository repo)
        {
            var storageIds = new HashSet<string>();
            var filesInfo = new Dictionary<string, byte[]>();

            foreach (JobObject jobObject in files)
            {
                filesInfo.Clear();
                filesInfo[jobObject.FileName] = jobObject.GetData();
                storageIds.Add(repo.CreateStorage(ZipUtils.Zip(filesInfo)));
            }

            return storageIds;
        }
    }
}