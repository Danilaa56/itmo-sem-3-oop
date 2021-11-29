using Banks.BLL;

namespace Banks.UI.Commands
{
    public class DefaultCommand : Command
    {
        private readonly ApplicationContext _context;

        public DefaultCommand(ApplicationContext context)
        {
            _context = context;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            _context.Default();
            return Response("Database was recreated and defaults were set up");
        }
    }
}