using System;
using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class TestSerializer
    {
        [Test]
        public void TestSomething()
        {
            byte[] bytes = new byte[] { 1, 2 };
            byte[] bytes2 = new byte[] { 1, 2 };

            Console.WriteLine(Hash(bytes));
            Console.WriteLine(Hash(bytes2));
            bytes[1] = 1;
            Console.WriteLine(Hash(bytes));
        }

        public int Hash(byte[] ar)
        {
            var hashCode = new HashCode();
            hashCode.Add(ar);
            return hashCode.ToHashCode();
        }

        [Test]
        public void SerializeBackupObjectTest()
        {
            TrySerialize(new BackupObject("name"));
        }

        [Test]
        public void SerializeJobObjectLocalTest()
        {
            TrySerialize(new JobObjectLocal("rootPath", "fileName"));
        }

        [Test]
        public void SerializeJobObjectStoredTest()
        {
            TrySerialize(new JobObjectInMemory(new byte[] { 1, 2, 3, 4, 5 }, "fileName"));
        }

        [Test]
        public void SerializeIJobObjectTest()
        {
            TrySerialize((IJobObject)new JobObjectLocal("rootPath", "fileName"));
        }

        [Test]
        public void SerializeStorageBlankTest()
        {
            var jobObjects = new List<IJobObject>() { new JobObjectLocal("rootPath", "fileName") };
            TrySerialize(new StorageBlank(jobObjects));
        }

        [Test]
        public void SerializeStorageTest()
        {
            var repo = new RepositoryLocal("tmp");
            var jobObjects = new List<IJobObject>() { new JobObjectLocal("rootPath", "fileName") };
            TrySerialize(new Storage("someId", repo, jobObjects));
        }

        [Test]
        public void SerializeRestorePointTest()
        {
            var repo = new RepositoryLocal("tmp");
            var jobObjects = new List<IJobObject>() { new JobObjectLocal("rootPath", "fileName") };
            var storages = new List<Storage>() { new Storage("someId", repo, jobObjects) };
            TrySerialize(new RestorePoint(123, storages, new SingleStorageDistributor(), new ZipStoragePacker(),
                new RepositoryLocal("tmp")));
        }

        [Test]
        public void SerializeRepositoryLocalTest()
        {
            TrySerialize(new RepositoryLocal("tmp"));
        }

        [Test]
        public void SerializeRepositoryRemoteTest()
        {
            TrySerialize(new RepositoryRemote("localhost", 8080));
        }

        [Test]
        public void SerializeBackupJobTest()
        {
            var repo = new RepositoryLocal("tmp");
            var backupJob = new BackupJob(repo);
            File.WriteAllBytes("tmp" + Path.PathSeparator + "fileName", new byte[] { 1, 2, 3 });
            backupJob.Add(new JobObjectLocal("tmp", "fileName"));
            backupJob.CreateRestorePoint();

            TrySerialize(backupJob);
        }

        [TearDown]
        public void TearDown()
        {
            var tmpDir = new DirectoryInfo("tmp");
            if (tmpDir.Exists)
                Tests.DeleteDirRecursively(tmpDir);
        }

        private void TrySerialize<T>(T obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            });

            Console.WriteLine(jsonString);

            object deserialized = JsonConvert.DeserializeObject(jsonString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            });

            Assert.AreEqual(obj, deserialized);
        }
    }
}