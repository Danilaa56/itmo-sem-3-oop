using Banks.BLL;

namespace Banks.UI.Commands
{
    public class AccountCommand : Command
    {
        private CommandResponse _usage = Response("account <create|destroy|topup|withdraw|transfer|amount>");

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

            int bankId = int.Parse(args[2]);
            int personId = int.Parse(args[3]);

            int accountId;
            switch (args[4].ToLower())
            {
                case "debit":
                    accountId = AccountLogic.CreateDebit(bankId, personId);
                    break;
                case "credit":
                    accountId = AccountLogic.CreateCredit(bankId, personId);
                    break;
                case "deposit":
                    accountId = AccountLogic.CreateDeposit(bankId, personId);
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

            int accountId = int.Parse(args[2]);
            decimal amount = decimal.Parse(args[3]);

            AccountLogic.TopUp(accountId, amount);

            return Response($"Account was topped up");
        }

        private CommandResponse Withdraw(string[] args)
        {
            CommandResponse usage = Response("account withdraw ACCOUNT_ID AMOUNT");
            if (args.Length != 4)
                return usage;

            int accountId = int.Parse(args[2]);
            decimal amount = decimal.Parse(args[3]);

            AccountLogic.Withdraw(accountId, amount);

            return Response($"Amount was withdrew from the account");
        }

        private CommandResponse Transfer(string[] args)
        {
            CommandResponse usage = Response("account transfer ACCOUNT_SENDER_ID ACCOUNT_RECEIVER_ID AMOUNT");
            if (args.Length != 5)
                return usage;

            int senderId = int.Parse(args[2]);
            int receiverId = int.Parse(args[3]);
            decimal amount = decimal.Parse(args[4]);

            AccountLogic.Transfer(senderId, receiverId, amount);

            return Response($"Account was topped up");
        }

        private CommandResponse Destroy(string[] args)
        {
            if (args.Length != 3)
                return Response("account destroy ACCOUNT_ID");
            AccountLogic.Destroy(int.Parse(args[2]));
            return Response($"Account {args[2]} was was destroyed");
        }

        private CommandResponse Amount(string[] args)
        {
            if (args.Length != 3)
                return Response("account amount ACCOUNT_ID");
            decimal amount = AccountLogic.AmountAt(int.Parse(args[2]));
            return Response($"Amount at account {args[2]}: {amount}");
        }
    }
}