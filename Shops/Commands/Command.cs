using System;

namespace Shops.Commands
{
    public abstract class Command
    {
        public abstract CommandResponse ProcCommand(string[] args);

        protected static CommandResponse Response(bool shouldExit = false)
        {
            return new CommandResponse(Array.Empty<string>(), shouldExit);
        }

        protected static CommandResponse Response(string msg, bool shouldExit = false)
        {
            return new CommandResponse(new string[] { msg }, shouldExit);
        }

        protected static CommandResponse Response(string[] msgLines, bool shouldExit = false)
        {
            return new CommandResponse(msgLines, shouldExit);
        }
    }
}