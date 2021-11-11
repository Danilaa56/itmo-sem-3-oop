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

        public static CommandResponseBuilder Builder()
        {
            return new CommandResponseBuilder();
        }

        public class CommandResponseBuilder
        {
            public bool ShouldExit { get; } = false;
            public List<string> Lines { get; } = new List<string>();

            public void WriteLine(object obj)
            {
                Lines.Add(obj is null ? "null" : obj.ToString());
            }

            public void WriteLine(IEnumerable<object> messages)
            {
                foreach (object message in messages)
                {
                    WriteLine(message);
                }
            }

            public CommandResponse Build()
            {
                return new CommandResponse(Lines, ShouldExit);
            }
        }
    }
}