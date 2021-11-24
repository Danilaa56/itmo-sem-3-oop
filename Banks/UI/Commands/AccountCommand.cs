using System;
using Banks.BLL;

namespace Banks.UI.Commands
{
    public class AccountCommand : Command
    {
        private readonly ApplicationContext _context;
        private readonly CommandResponse _usage = Response("account <create|destroy|topup|withdraw|transfer|amount>");

        public AccountCommand(ApplicationContext context)
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
                case "amount":
                    return Amount(args);
                case "create":
                    return Create(args);
                case "destroy":
                    return Destroy(args);
                case "topup":
                    return TopUp(args);
                case "transfer":
                    return Transfer(args);
                case "withdraw":
                    return Withdraw(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Create(string[] args)
        {
            CommandResponse usage = Response("account create BANK_ID PERSON_ID <credit|debit|deposit>");
            if (args.Length != 5)
                return usage;

            var bankId = Guid.Parse(args[2]);
            var personId = Guid.Parse(args[3]);

            Guid accountId;
            switch (args[4].ToLower())
            {
                case "debit":
                    accountId = _context.Account.CreateDebit(bankId, personId);
                    break;
                case "credit":
                    accountId = _context.Account.CreateCredit(bankId, personId);
                    break;
                case "deposit":
                    accountId = _context.Account.CreateDeposit(bankId, personId);
                    break;
                default:
                    return usage;
            }

            return Response($"Account was created, id: {accountId}");
        }

        private CommandResponse TopUp(string[] args)
        {
            CommandResponse usage = Response("account topup ACCOUNT_ID AMOUNT");
            if (args.Length != 4)
                return usage;

            var accountId = Guid.Parse(args[2]);
            decimal amount = decimal.Parse(args[3]);

            _context.Account.TopUp(accountId, amount);

            return Response($"Account was topped up");
        }

        private CommandResponse Withdraw(string[] args)
        {
            CommandResponse usage = Response("account withdraw ACCOUNT_ID AMOUNT");
            if (args.Length != 4)
                return usage;

            var accountId = Guid.Parse(args[2]);
            decimal amount = decimal.Parse(args[3]);

            _context.Account.Withdraw(accountId, amount);

            return Response($"Amount was withdrew from the account");
        }

        private CommandResponse Transfer(string[] args)
        {
            CommandResponse usage = Response("account transfer ACCOUNT_SENDER_ID ACCOUNT_RECEIVER_ID AMOUNT");
            if (args.Length != 5)
                return usage;

            var senderId = Guid.Parse(args[2]);
            var receiverId = Guid.Parse(args[3]);
            decimal amount = decimal.Parse(args[4]);

            _context.Account.Transfer(senderId, receiverId, amount);

            return Response($"Account was topped up");
        }

        private CommandResponse Destroy(string[] args)
        {
            if (args.Length != 3)
                return Response("account destroy ACCOUNT_ID");
            _context.Account.Destroy(Guid.Parse(args[2]));
            return Response($"Account {args[2]} was destroyed");
        }

        private CommandResponse Amount(string[] args)
        {
            if (args.Length != 3)
                return Response("account amount ACCOUNT_ID");
            decimal amount = _context.Account.AmountAt(Guid.Parse(args[2]));
            return Response($"Amount at account {args[2]}: {amount}");
        }
    }
}