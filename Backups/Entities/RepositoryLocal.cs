using System;
using System.IO;
using Backups.Tools;

namespace Backups.Entities
{
    public class RepositoryLocal : Repository
    {
        private DirectoryInfo dirInfo;

        public RepositoryLocal(string path)
        {
            dirInfo = new DirectoryInfo(path ?? throw new ArgumentNullException(nameof(path)));
            if (!dirInfo.Exists)
            {
                try
                {
                    dirInfo.Create();
                }
                catch (IOException e)
                {
                    throw new BackupException("Failed to create dir for repository", e);
                }
            }

            foreach (FileInfo enumerateFile in dirInfo.EnumerateFiles())
            {
                File.Delete(enumerateFile.FullName);
            }
        }

        public override string CreateStorage(byte[] data)
        {
            string fileName;
            string fullName;
            do
            {
                fileName = randomHexString(16);
                fullName = dirInfo + "/" + fileName;
            }
            while (File.Exists(fullName));

            File.WriteAllBytes(fullName, data);
            return fileName;
        }
    }
}