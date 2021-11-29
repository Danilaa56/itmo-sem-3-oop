using Banks.BLL;

namespace Banks.UI.Commands
{
    public class ResetCommand : Command
    {
        private readonly ApplicationContext _context;

        public ResetCommand(ApplicationContext context)
        {
            _context = context;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length != 1)
                Response("usage: reset");
            _context.Reset();
            return Response("Database was recreated");
        }
    }
}