using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities;
using Backups.Entities.DataObjects;

namespace BackupsExtra.Entities.IrrelevantResolvers
{
    public class RestorePointMerger : IIrrelevantResolver
    {
        public List<RestorePoint> Resolve(
            IEnumerable<RestorePoint> existingPoints,
            IEnumerable<RestorePoint> irrelevantPoints)
        {
            var irrelevantList = irrelevantPoints.ToImmutableList();
            var resultList = existingPoints.ToImmutableList().RemoveRange(irrelevantList).ToList();
            resultList[0] = MergeIrrelevantPointsIntoOne(irrelevantList, resultList[0]);
            return resultList;
        }

        private static ImmutableList<BackupObjectPath> RestorePointToObjectsPaths(RestorePoint restorePoint)
        {
            return
                restorePoint
                    .Storages
                    .SelectMany(storage => storage
                        .JobObjects
                        .Select(jobObject => new BackupObjectPath(
                            restorePoint.Repository,
                            restorePoint,
                            storage,
                            jobObject,
                            jobObject.BackupObject))).ToImmutableList();
        }

        private RestorePoint MergeIrrelevantPointsIntoOne(
            IEnumerable<RestorePoint> irrelevantPoints,
            RestorePoint to)
        {
            if (irrelevantPoints is null)
                throw new ArgumentNullException(nameof(irrelevantPoints));
            if (to is null)
                throw new ArgumentNullException(nameof(to));

            var irrelevantList = irrelevantPoints.ToImmutableList();

            var irrelevantObjectsPaths =
                irrelevantList.SelectMany(restorePoint => RestorePointToObjectsPaths(restorePoint)).ToImmutableList();
            ImmutableList<BackupObjectPath> existingObjectsPaths = RestorePointToObjectsPaths(to);

            ImmutableList<BackupObjectPath> objectsToMerge = irrelevantObjectsPaths.FindAll(
                path => !existingObjectsPaths.Any(
                    existingPath => path.BackupObject.Equals(existingPath.BackupObject)));

            if (objectsToMerge.Count == 0)
                return to;

            objectsToMerge = objectsToMerge.AddRange(existingObjectsPaths);

            ImmutableList<IJobObject> jobObjects = objectsToMerge.ConvertAll(objectPath => objectPath.ReadJobObject());

            List<StorageBlank> storageBlanks = to.ObjectDistributor.DistributeJobObjects(jobObjects);
            List<Storage> storages = storageBlanks.ConvertAll(storageBlank =>
            {
                byte[] data = to.StoragePacker.Pack(storageBlank);
                string storageId = to.Repository.CreateStorage(data);
                return new Storage(storageId, to.Repository, storageBlank.JobObjects);
            });

            return new RestorePoint(
                to.CreationDateUtc,
                storages,
                to.ObjectDistributor,
                to.StoragePacker,
                to.Repository);
        }
    }
}