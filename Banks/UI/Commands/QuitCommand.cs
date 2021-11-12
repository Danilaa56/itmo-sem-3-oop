namespace Banks.UI.Commands
{
    public class QuitCommand : Command
    {
        public override CommandResponse ProcessCommand(string[] args)
        {
            return Response(true);
        }
    }
}