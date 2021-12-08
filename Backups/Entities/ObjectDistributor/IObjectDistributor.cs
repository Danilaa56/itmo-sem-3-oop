using System.Collections.Generic;
using Backups.Entities.DataObjects;

namespace Backups.Entities.ObjectDistributor
{
    public interface IObjectDistributor
    {
        List<StorageBlank> DistributeJobObjects(IEnumerable<IJobObject> jobObjects);
    }
}