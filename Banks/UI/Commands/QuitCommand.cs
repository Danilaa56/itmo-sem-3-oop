using Banks.BLL;

namespace Banks.UI.Commands
{
    public class QuitCommand : Command
    {
        private ApplicationContext _context;

        public QuitCommand(ApplicationContext context)
        {
            _context = context;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            _context.Save();
            return Response(true);
        }
    }
}