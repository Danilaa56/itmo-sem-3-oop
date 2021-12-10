using System.Collections.Generic;
using Backups.Entities;
using Backups.Entities.ObjectDistributor;
using Backups.Entities.Repository;
using Backups.Entities.StoragePacker;
using BackupsExtra.Entities.IrrelevantResolvers;
using NUnit.Framework;

namespace BackupsExtra.Tests.IrrelevantResolvers
{
    [TestFixture]
    public class MergerTest
    {
        private IObjectDistributor _distributor = new SingleStorageDistributor();
        private IStoragePacker _packer = new ZipStoragePacker();
        private IRepository _repo = new RepositoryInMemory();

        [Test]
        public void MergePointWithItSelf()
        {
            // Prepare
            var storages = new Storage[0];

            var points = new List<RestorePoint>();
            points.Add(new RestorePoint(0, storages, _distributor, _packer, _repo));
            points.Add(points[0]);
            var irrelevant = new List<RestorePoint>();
            irrelevant.Add(points[0]);

            var resolver = new RestorePointMerger();

            // Test
            List<RestorePoint> resultChain = resolver.Resolve(points, irrelevant);
            Assert.AreEqual(1, resultChain.Count);
            Assert.AreEqual(points[0], resultChain[0]);
        }
    }
}