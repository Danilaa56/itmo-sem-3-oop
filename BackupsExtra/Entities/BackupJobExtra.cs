using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Backups.Entities;
using Backups.Entities.Repository;
using Backups.Tools;
using BackupsExtra.Entities.Irrelevanters;
using BackupsExtra.Entities.IrrelevantResolvers;
using BackupsExtra.Entities.Loggers;

namespace BackupsExtra.Entities
{
    public class BackupJobExtra : BackupJob
    {
        private static Logger _logger = LoggerFactory.GetLogger("BackupJobExtra");

        private IIrrelevanter _irrelevanter = new DummyIrrelevanter();
        private IIrrelevantResolver _irrelevantResolver = new RestorePointDeleter();

        public BackupJobExtra(IRepository repository)
            : base(repository)
        {
        }

        public IIrrelevanter Irrelevanter
        {
            get => _irrelevanter;
            set => _irrelevanter = value ?? throw new ArgumentNullException();
        }

        public IIrrelevantResolver IrrelevantResolver
        {
            get => _irrelevantResolver;
            set => _irrelevantResolver = value ?? throw new ArgumentNullException();
        }

        public override RestorePoint CreateRestorePoint()
        {
            _logger.Info("Creating restore point");
            base.CreateRestorePoint();
            var existingPoints = RestorePoints.ToImmutableList();
            _logger.Info("Defining irrelevant restore points");
            List<RestorePoint> irrelevant = _irrelevanter.DefineIrrelevant(existingPoints);
            _logger.Info($"Found irrelevant points: {irrelevant.Count}");
            _logger.Info($"Resolving irrelevant points");
            List<RestorePoint> resultChain = _irrelevantResolver.Resolve(existingPoints, irrelevant);
            if (resultChain.Count == 0)
                throw new BackupException("After cleaning restore points there is only 0");
            RestorePoints = resultChain;
            _logger.Info($"Restore point chain is clean");
            _logger.Info($"Restore point was successfully created");
            return resultChain.Last();
        }
    }
}