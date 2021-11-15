using Banks.BLL;

namespace Banks.UI.Commands
{
    public class ResetCommand : Command
    {
        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length != 1)
                Response("usage: reset");
            ServiceLogic.Reset();
            return Response("Database was recreated");
        }
    }
}