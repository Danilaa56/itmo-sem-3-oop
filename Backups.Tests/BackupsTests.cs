using System.IO;
using Backups.Entities;
using Backups.Entities.Repository;
using Backups.Server.Entities;
using NUnit.Framework;

namespace Backups.Tests
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
        public void TestBackupJob()
        {
            IRepository repo = new RepositoryLocal($"tmp{Sep}repo");

            var backupJob = new BackupJob(repo, StorageType.SplitStorage);
            var jobObject1 = new JobObject($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObject($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            backupJob.Remove(jobObject1);
            RestorePoint restorePoint2 = backupJob.CreateRestorePoint();

            Assert.AreEqual(2, backupJob.GetRestorePoints().Count);
            Assert.AreEqual(3, repo.GetStorages().Length);
        }

        [Test]
        public void TestLocalRepo()
        {
            IRepository repo = new RepositoryLocal($"tmp{Sep}repo");

            var backupJob = new BackupJob(repo, StorageType.SingleStorage);
            var jobObject1 = new JobObject($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObject($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            Assert.AreEqual(true, File.Exists($"tmp{Sep}repo{Sep}{restorePoint1.StorageIds[0]}"));
        }

        [Test]
        public void TestRemoteRepo()
        {
            var server = new RepositoryRemoteServer(8080, $"tmp{Sep}repo");
            server.Start();

            IRepository repo = new RepositoryRemote("localhost", 8080);

            var backupJob = new BackupJob(repo, StorageType.SingleStorage);
            var jobObject1 = new JobObject($"tmp{Sep}data", "file1.txt");
            var jobObject2 = new JobObject($"tmp{Sep}data", "file2.txt");

            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();

            Assert.AreEqual(true, File.Exists($"tmp{Sep}repo{Sep}{restorePoint1.StorageIds[0]}"));
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDirRecursively(new DirectoryInfo("tmp"));
        }

        private static void DeleteDirRecursively(DirectoryInfo dir)
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