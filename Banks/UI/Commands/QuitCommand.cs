using Banks.UI.Commands;

namespace Shops.Commands
{
    public class QuitCommand : Command
    {
        public override CommandResponse ProcessCommand(string[] args)
        {
            return Response(true);
        }
    }
}