using Banks.BLL;

namespace Banks.UI.Commands
{
    public class TimeCommand : Command
    {
        private readonly ApplicationContext _context;
        private readonly CommandResponse _usage = Response("time <rotate>");

        public TimeCommand(ApplicationContext context)
        {
            _context = context;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "rotate":
                    return Rotate(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Rotate(string[] args)
        {
            if (args.Length != 3)
                return Response("time rotate DAYS");

            int days = int.Parse(args[2]);
            _context.Time.Rotate(TimeLogic.Day * days);

            return Response($"{days} seconds passed");
        }
    }
}