using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;

namespace Backups.Entities
{
    public class RestorePoint
    {
        public long creationDateUtc { get; }
        public ImmutableArray<string> storageIds { get; }

        public RestorePoint(long creationDateUtc, HashSet<string> storageIds)
        {
            this.creationDateUtc = creationDateUtc;
            if (storageIds is null) throw new ArgumentNullException(nameof(storageIds));
            this.storageIds = storageIds.ToImmutableArray();
        }

        public override string ToString()
        {;
            return JsonSerializer.Serialize(this);
        }
    }
}