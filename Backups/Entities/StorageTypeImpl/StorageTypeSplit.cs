using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities.StorageTypeImpl
{
    public class StorageTypeSplit : StorageType
    {
        public override IEnumerable<byte[]> PackJobObjects(IEnumerable<JobObject> jobObjects)
        {
            return jobObjects
                .Select(jobObject => new NamedData(jobObject.FileName, jobObject.GetData()))
                .Select(namedData => ZipUtils.Zip(new List<NamedData>() { namedData }));
        }
    }
}