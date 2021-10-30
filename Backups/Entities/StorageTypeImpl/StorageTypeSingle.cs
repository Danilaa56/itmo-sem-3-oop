using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Entities.StorageTypeImpl
{
    public class StorageTypeSingle : StorageType
    {
        public override IEnumerable<byte[]> PackJobObjects(IEnumerable<JobObject> jobObjects)
        {
            var packedData = new List<byte[]>();
            var filesData = jobObjects.Select(jobObject => new NamedData(jobObject.FileName, jobObject.GetData()))
                .ToList();
            packedData.Add(ZipUtils.Zip(filesData));
            return packedData;
        }
    }
}