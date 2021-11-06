using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities.StorageTypeImpl
{
    public class StorageTypeSplit : StorageType
    {
        public override IEnumerable<byte[]> PackJobObjects(IEnumerable<IJobObject> jobObjects)
        {
            return jobObjects
                .Select(jobObject => new NamedData(jobObject.FileName, jobObject.GetData()))
                .Select(namedData => new List<NamedData> { namedData }.Zip());
        }
    }
}