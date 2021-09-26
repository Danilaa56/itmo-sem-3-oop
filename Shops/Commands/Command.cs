namespace Shops.Commands
{
    public abstract class Command
    {
        public abstract string[] ProcCommand(string[] args);

        protected static string[] Response(string msg)
        {
            return new string[] { msg };
        }
    }
}