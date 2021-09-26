namespace Shops.Commands
{
    public class QuitCommand : Command
    {
        public override string[] ProcCommand(string[] args)
        {
            System.Environment.Exit(0);
            return Response("ok");
        }
    }
}