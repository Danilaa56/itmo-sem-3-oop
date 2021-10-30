using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public RestorePoint(long creationDateUtc, ICollection<string> storageIds)
        {
            CreationDateUtc = creationDateUtc;
            if (storageIds is null) throw new ArgumentNullException(nameof(storageIds));
            StorageIds = storageIds.ToImmutableArray();
        }

        public long CreationDateUtc { get; }
        public ImmutableArray<string> StorageIds { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}