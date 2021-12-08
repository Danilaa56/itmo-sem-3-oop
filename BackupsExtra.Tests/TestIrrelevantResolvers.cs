using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using BackupsExtra.Entities.IrrelevantResolvers;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class TestIrrelevantResolvers
    {
        private static readonly string Sep = Path.DirectorySeparatorChar.ToString();

        private RepositoryLocal _localRepo;
        private SingleStorageDistributor _distributor;
        private ZipStoragePacker _packer;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory($"tmp{Sep}repo");
            _localRepo = new RepositoryLocal($"tmp{Sep}repo");
            _distributor = new SingleStorageDistributor();
            _packer = new ZipStoragePacker();
        }

        [Test]
        public void TestDeleterResolver()
        {
            // Prepare
            var storages = new Storage[0];

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _localRepo));

            var resolver = new RestorePointDeleter();

            // Test
            List<RestorePoint> resultChain = resolver.Resolve(points, points);
            Assert.AreEqual(0, resultChain.Count);

            resultChain = resolver.Resolve(points, new RestorePoint[0]);
            Assert.AreEqual(1, resultChain.Count);
            Assert.AreEqual(points[0], resultChain[0]);
        }

        [Test]
        public void TestMergerResolver()
        {
            // Prepare
            var storages = new Storage[0];

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _localRepo));

            var resolver = new RestorePointMerger();

            // Test
            List<RestorePoint> resultChain = resolver.Resolve(points, points);
            Assert.AreEqual(1, resultChain.Count);
            Assert.AreEqual(points[0], resultChain[0]);

            resultChain = resolver.Resolve(points, new RestorePoint[0]);
            Assert.AreEqual(1, resultChain.Count);
            Assert.AreEqual(points[0], resultChain[0]);
        }

        [TearDown]
        public void TearDown()
        {
            Tests.DeleteDirRecursively(new DirectoryInfo("tmp"));
        }
    }
}