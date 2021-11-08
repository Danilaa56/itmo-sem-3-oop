using System;
using System.Collections.Generic;

namespace Banks.UI.Commands
{
    public abstract class Command
    {
        public abstract CommandResponse ProcessCommand(string[] args);

        protected static CommandResponse Response(bool shouldExit = false)
        {
            return new CommandResponse(Array.Empty<string>(), shouldExit);
        }

        protected static CommandResponse Response(string msg, bool shouldExit = false)
        {
            return new CommandResponse(new string[] { msg }, shouldExit);
        }

        protected static CommandResponse Response(IEnumerable<string> msgLines, bool shouldExit = false)
        {
            return new CommandResponse(msgLines, shouldExit);
        }
    }
}