using System;
using System.IO;

namespace Backups.Entities
{
    public class JobObjectLocal : IJobObject
    {
        public JobObjectLocal(string rootPath, string fileName)
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public string FileName { get; }
        public string RootPath { get; }

        public byte[] GetData()
        {
            return File.ReadAllBytes(RootPath + Path.DirectorySeparatorChar + FileName);
        }
    }
}