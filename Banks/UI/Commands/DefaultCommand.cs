using Banks.BLL;

namespace Banks.UI.Commands
{
    public class DefaultCommand : Command
    {
        public override CommandResponse ProcessCommand(string[] args)
        {
            ServiceLogic.Default();
            return Response("Database was recreated and defaults were set up");
        }
    }
}