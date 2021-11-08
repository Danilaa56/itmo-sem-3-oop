using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Banks.UI.Commands
{
    public class CommandResponse
    {
        public CommandResponse(IEnumerable<string> lines, bool shouldExit = false)
        {
            Lines = (lines ?? throw new ArgumentNullException(nameof(lines))).ToImmutableList();
            ShouldExit = shouldExit;
        }

        public bool ShouldExit { get; }
        public ImmutableList<string> Lines { get; }
    }
}