using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Backups.Tools;

namespace Backups.Entities.Repository
{
    public class RepositoryLocal : IRepository
    {
        private readonly DirectoryInfo _dirInfo;
        private readonly List<string> _storages = new List<string>();

        public RepositoryLocal(string path)
        {
            _dirInfo = new DirectoryInfo(path ?? throw new ArgumentNullException(nameof(path)));
            if (!_dirInfo.Exists)
            {
                try
                {
                    _dirInfo.Create();
                }
                catch (IOException e)
                {
                    throw new BackupException("Failed to create dir for repository", e);
                }
            }

            foreach (FileInfo enumerateFile in _dirInfo.EnumerateFiles())
            {
                File.Delete(enumerateFile.FullName);
            }
        }

        public string CreateStorage(byte[] data)
        {
            string storageId;
            string fullName;
            do
            {
                storageId = StringUtils.RandomHexString(16);
                fullName = _dirInfo + "/" + storageId;
            }
            while (File.Exists(fullName));

            File.WriteAllBytes(fullName, data);
            _storages.Add(storageId);
            return storageId;
        }

        public ImmutableArray<string> GetStorages()
        {
            return _storages.ToImmutableArray();
        }
    }
}