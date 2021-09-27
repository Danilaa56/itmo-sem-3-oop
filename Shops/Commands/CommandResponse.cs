using System;
using System.Collections.Immutable;

namespace Shops
{
    public class CommandResponse
    {
        public CommandResponse(string[] lines, bool shouldExit = false)
        {
            Lines = (lines ?? throw new ArgumentNullException(nameof(lines))).ToImmutableList();
            ShouldExit = shouldExit;
        }

        public bool ShouldExit { get; }
        public ImmutableList<string> Lines { get; }
    }
}