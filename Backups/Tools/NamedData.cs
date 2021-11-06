using System;
using System.Collections.Immutable;

namespace Backups.Tools
{
    public class NamedData
    {
        public NamedData(string name, byte[] data)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            Data = data.ToImmutableArray();
        }

        public string Name { get; }
        public ImmutableArray<byte> Data { get; }
    }
}