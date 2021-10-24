using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;

namespace Backups
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string path = args[0];
            var repo = new RepositoryLocal(args[1]);

            if (path.Length == 0 || path[path.Length - 1] != '/')
                path += '/';

            var backupJob = new BackupJob(repo);
            backupJob.SetStorageType(StorageType.SPLIT_STORAGES);

            var jobObject1 = new JobObject(path, "File 1.txt");
            var jobObject2 = new JobObject(path, "File 2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            var restore1 = backupJob.CreateRestorePoint();
            Console.WriteLine(restore1);

            backupJob.Remove(jobObject1);
            var restore2 = backupJob.CreateRestorePoint();
        }

        public static void PrintLn(object obj)
        {
            Console.WriteLine(obj);
        }

        public static List<FileInfo> GetFiles(DirectoryInfo dirInfo)
        {
            var fileInfos = new List<FileInfo>();
            fileInfos.AddRange(dirInfo.EnumerateFiles());
            fileInfos.AddRange(dirInfo.EnumerateDirectories().SelectMany(GetFiles));

            return fileInfos;
        }

        // public static byte[] Backup(string path, List<FileInfo> files)
        // {
        //     using var ms = new MemoryStream();
        //     using var archive = new ZipArchive(ms, ZipArchiveMode.Update);
        //     foreach (FileInfo file in files)
        //     {
        //         ZipArchiveEntry orderEntry = archive.CreateEntry(file.FullName.Split(path)[1]);
        //         using var writer = new BinaryWriter(orderEntry.Open());
        //         byte[] da = File.ReadAllBytes(file.FullName);
        //         // Console.WriteLine(da.Length);
        //         writer.Write(da);
        //         writer.Flush();
        //         writer.Close();
        //     }
        //     archive.Dispose();
        //
        //     return ms.ToArray();
        // }
    }
}