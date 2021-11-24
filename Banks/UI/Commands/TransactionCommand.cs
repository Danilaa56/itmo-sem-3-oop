using System;
using Banks.BLL;

namespace Banks.UI.Commands
{
    public class TransactionCommand : Command
    {
        private readonly ApplicationContext _context;
        private readonly CommandResponse _usage = Response("transaction <cancel>");

        public TransactionCommand(ApplicationContext context)
        {
            _context = context;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length < 2)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "cancel":
                    return Cancel(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Cancel(string[] args)
        {
            if (args.Length != 3)
                return Response("transaction cancel TRANSACTION_ID");

            _context.Transaction.Cancel(Guid.Parse(args[2]));

            return Response("Transaction was successfully cancelled");
        }
    }
}