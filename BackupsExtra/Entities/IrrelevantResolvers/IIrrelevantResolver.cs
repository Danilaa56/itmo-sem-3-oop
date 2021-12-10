using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Entities.IrrelevantResolvers
{
    public interface IIrrelevantResolver
    {
        List<RestorePoint> Resolve(
            IEnumerable<RestorePoint> existingPoints,
            IEnumerable<RestorePoint> irrelevantPoints);
    }
}