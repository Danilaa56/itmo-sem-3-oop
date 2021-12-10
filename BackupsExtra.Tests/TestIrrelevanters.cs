using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Entities.DataObjects;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using Backups.Tools;
using BackupsExtra.Entities.Irrelevanters;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class TestIrrelevanters
    {
        private RepositoryLocal _localRepo;
        private SingleStorageDistributor _distributor;
        private ZipStoragePacker _packer;

        [SetUp]
        public void Setup()
        {
            _localRepo = new RepositoryLocal(Path.Combine("tmp", "repo"));
            _distributor = new SingleStorageDistributor();
            _packer = new ZipStoragePacker();
        }

        [Test]
        public void TestChainLengthIrrelevanter()
        {
            // Prepare
            var storages = new Storage[0];

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _localRepo));

            var irrelevanter = new ChainLengthIrrelevanter(1);

            // Test
            List<RestorePoint> irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(0, irrelevantPoints.Count);

            points.Add(new RestorePoint(1, storages, _distributor, _packer, _localRepo));

            irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(1, irrelevantPoints.Count);
            Assert.AreEqual(points[0], irrelevantPoints[0]);
        }

        [Test]
        public void TestDateIrrelevanter()
        {
            // Prepare
            var storages = new Storage[0];

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _localRepo));

            var irrelevanter = new DateIrrelevanter(6000);

            DateUtils.RotateTime(5000);

            // Test
            List<RestorePoint> irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(0, irrelevantPoints.Count);

            DateUtils.RotateTime(5000);

            irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(1, irrelevantPoints.Count);
            Assert.AreEqual(points[0], irrelevantPoints[0]);
        }

        [Test]
        public void TestObjectCountIrrelevanter()
        {
            // Prepare
            var jobObjects = new List<IJobObject>();
            jobObjects.Add(new JobObjectInMemory(new byte[] { 1, 2, 3 }, "file1.bin"));

            var storages = new List<Storage>();
            storages.Add(new Storage("id1", _localRepo, jobObjects));

            var irrelevanter = new ObjectsCountIrrelevanter(1);

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _localRepo));

            // Test
            List<RestorePoint> irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(0, irrelevantPoints.Count);

            points.Add(new RestorePoint(1, storages, _distributor, _packer, _localRepo));

            irrelevantPoints = irrelevanter.DefineIrrelevant(points);
            Assert.AreEqual(1, irrelevantPoints.Count);
            Assert.AreEqual(points[0], irrelevantPoints[0]);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete("tmp", true);
        }
    }
}