namespace Shops.Commands
{
    public class QuitCommand : Command
    {
        public override CommandResponse ProcCommand(string[] args)
        {
            return Response(true);
        }
    }
}