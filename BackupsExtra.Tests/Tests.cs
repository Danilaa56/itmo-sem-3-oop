using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using Backups.Server.Entities;
using Backups.Tools;
using BackupsExtra.Entities;
using BackupsExtra.Entities.Irrelevanters;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly char Sep = Path.DirectorySeparatorChar;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory($"tmp{Sep}data");
            Directory.CreateDirectory($"tmp{Sep}repo");

            File.WriteAllText($"tmp{Sep}data{Sep}file1.txt", "Hello 1");
            File.WriteAllText($"tmp{Sep}data{Sep}file2.txt", "Hello 2 and longer");
        }

        [Test]
        public void TestRestore()
        {
            IRepository repo = new RepositoryLocal($"tmp{Sep}repo");

            File.WriteAllText($"tmp{Sep}data{Sep}tmp.txt", "Test content");

            var backupJob = new BackupJobExtra(repo);
            backupJob.ObjectDistributor = new SplitStorageDistributor();
            var jobObject1 = new JobObjectLocal($"tmp{Sep}data", "tmp.txt");
            backupJob.Add(jobObject1);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            File.Delete($"tmp{Sep}data{Sep}tmp.txt");

            restorePoint1.Restore(new DirectoryInfo($"tmp{Sep}data"));

            Assert.AreEqual("Test content", File.ReadAllText($"tmp{Sep}data{Sep}tmp.txt"));
        }

        [Test]
        public void TestSaveLoadBackupContext()
        {
            var contextCreated = new Context($"tmp{Sep}workdir");

            IRepository repo = new RepositoryLocal($"tmp{Sep}repo");

            var backupJob = new BackupJob(repo);
            contextCreated.AddBackupJob(backupJob);
            var jobObject1 = new JobObjectLocal($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObjectLocal($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            backupJob.CreateRestorePoint();

            contextCreated.Save();

            var contextLoaded = new Context($"tmp{Sep}workdir");

            BackupJob[] backupJobsSaved = contextCreated.BackupJobs().ToArray();
            BackupJob[] backupJobsLoaded = contextLoaded.BackupJobs().ToArray();

            Assert.AreEqual(backupJobsSaved.Length, backupJobsLoaded.Length);

            for (int i = 0; i < backupJobsSaved.Length; i++)
            {
                Assert.AreEqual(backupJobsSaved[i], backupJobsLoaded[i]);
            }
        }

        [Test]
        public void TestRemoteRepo()
        {
            var server = new RepositoryRemoteServer(8080, $"tmp{Sep}repo");
            server.Start();

            IRepository repo = new RepositoryRemote("localhost", 8080);

            var backupJob = new BackupJob(repo);
            var jobObject1 = new JobObjectLocal($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObjectLocal($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            Assert.AreEqual(true, File.Exists($"tmp{Sep}repo{Sep}{restorePoint1.Storages[0].Id}"));
        }

        [Test]
        public void TestRemoteGetStorageIds()
        {
            var server = new RepositoryRemoteServer(8080, $"tmp{Sep}repo");
            server.Start();

            IRepository repo = new RepositoryRemote("localhost", 8080);

            var backupJob = new BackupJob(repo);
            var jobObject1 = new JobObjectLocal($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObjectLocal($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            var storageIds = restorePoint1.Storages.Select(storage => storage.Id).ToHashSet();

            Assert.That(repo.GetStorages().ToHashSet().SetEquals(storageIds));
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDirRecursively(new DirectoryInfo("tmp"));
        }

        public static void DeleteDirRecursively(DirectoryInfo dir)
        {
            foreach (FileSystemInfo enumerateFileSystemInfo in dir.EnumerateFileSystemInfos())
            {
                if (enumerateFileSystemInfo is DirectoryInfo info)
                    DeleteDirRecursively(info);
                else
                    enumerateFileSystemInfo.Delete();
            }

            dir.Delete();
        }
    }
}