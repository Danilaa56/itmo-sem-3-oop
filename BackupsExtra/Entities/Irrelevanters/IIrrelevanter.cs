using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.Entities.Irrelevanters
{
    public interface IIrrelevanter
    {
        List<RestorePoint> DefineIrrelevant(IEnumerable<RestorePoint> restorePoints);
    }
}