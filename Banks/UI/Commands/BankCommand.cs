using System;
using System.Collections.Generic;
using Banks.BLL;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public class BankCommand : Command
    {
        private ICli _cli;

        private CommandResponse _usage =
            Response("bank <register|getsubscribers|unregister|subscribe|unsubscribe|list|change>");

        public BankCommand(ICli cli)
        {
            _cli = cli;
        }

        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "register":
                    return Register(args);
                case "getsubscribers":
                    return GetSubscribers(args);
                case "unregister":
                    return Unregister(args);
                case "subscribe":
                    return Subscribe(args);
                case "unsubscribe":
                    return Unsubscribe(args);
                case "list":
                    return List(args);
                case "change":
                    return Change(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Register(string[] args)
        {
            if (args.Length != 2)
            {
                return Response("bank register");
            }

            if (args.Length == 2)
            {
                _cli.WriteLine("Enter data or empty line to cancel");

                _cli.WriteLine("Name: ");
                if (!_cli.Read(out string name))
                    return Cancel();
                _cli.WriteLine("Percent for remains for debit accounts: ");
                if (!_cli.Read(out decimal debitPercentForRemains))
                    return Cancel();
                _cli.WriteLine("Credit limit for credit accounts: ");
                if (!_cli.Read(out decimal creditLimit))
                    return Cancel();
                _cli.WriteLine("Commission for credit accounts: ");
                if (!_cli.Read(out decimal creditCommission))
                    return Cancel();

                _cli.WriteLine("Percent for remains for deposit accounts");
                _cli.WriteLine("It is defined with levels and minimal value, if amount on account is too small");
                _cli.WriteLine("Each level consist of AMOUNT and PERCENT starting with this amount");
                _cli.WriteLine("Minimal percent: ");
                if (!_cli.Read(out decimal minDepositPercentForRemains))
                    return Cancel();
                _cli.WriteLine("Levels count (negative count means zero): ");
                if (!_cli.Read(out int levelsCount))
                    return Cancel();

                var depositLevels = new Dictionary<decimal, decimal>();
                if (depositLevels == null) throw new ArgumentNullException(nameof(depositLevels));
                for (int i = 0; i < levelsCount; i++)
                {
                    _cli.WriteLine("Amount on account: ");
                    if (!_cli.Read(out decimal key))
                        return Cancel();
                    _cli.WriteLine("Percent on remains: ");
                    if (!_cli.Read(out decimal percent))
                        return Cancel();
                    depositLevels[key] = percent;
                }

                _cli.WriteLine("Time to unlock deposit accounts (in seconds): ");
                if (!_cli.Read(out long depositTime))
                    return Cancel();
                _cli.WriteLine("Transaction limit for untrusted accounts: ");
                if (!_cli.Read(out decimal anonLimit))
                    return Cancel();

                int id = BankLogic.RegisterBank(
                    name,
                    debitPercentForRemains,
                    creditLimit,
                    creditCommission,
                    minDepositPercentForRemains,
                    depositLevels,
                    depositTime * 1000,
                    anonLimit);

                return Response($"Bank was created, id - {id}");
            }

            throw new Exception("Impossible");
        }

        private CommandResponse Unregister(string[] args)
        {
            if (args.Length != 2)
            {
                return Response("bank unregister");
            }

            BankLogic.UnregisterBank(int.Parse(args[1]));
            return Response("Bank was unregistered");
        }

        private CommandResponse List(string[] args)
        {
            if (args.Length != 2)
                return Response("bank list");

            List<Bank> banks = BankLogic.List();

            var builder = CommandResponse.Builder();
            builder.WriteLine($"Banks count: {banks.Count}");
            builder.WriteLine(Table(banks));
            return builder.Build();
        }

        private CommandResponse Subscribe(string[] args)
        {
            if (args.Length != 4)
                return Response("bank subscribe BANK_ID PERSON_ID");

            BankLogic.Subscribe(int.Parse(args[2]), int.Parse(args[3]));
            return Response("Person was subscribed for the bank updates");
        }

        private CommandResponse Unsubscribe(string[] args)
        {
            if (args.Length != 4)
                return Response("bank unsubscribe BANK_ID PERSON_ID");

            BankLogic.Unsubscribe(int.Parse(args[2]), int.Parse(args[3]));
            return Response("Person was unsubscribed from the banks update");
        }

        private CommandResponse GetSubscribers(string[] args)
        {
            if (args.Length != 3)
                return Response("bank getsubscribers BANK_ID");

            List<Person> persons = BankLogic.GetSubscribers(int.Parse(args[2]));

            return Response(Table(persons));
        }

        private CommandResponse Cancel()
        {
            return Response("Cancelled");
        }

        private CommandResponse Change(string[] args)
        {
            var usage = Response(
                "bank change <debitpercent|creditlimit|creditcommission|mindepositpercent|deposittime|anonlimit> BANK_ID NEW_VALUE");
            if (args.Length != 5)
            {
                return usage;
            }

            int bankId = int.Parse(args[3]);

            switch (args[2])
            {
                case "debitpercent":
                    BankLogic.ChangeDebitPercent(bankId, decimal.Parse(args[4]));
                    break;
                case "creditlimit":
                    BankLogic.ChangeCreditLimit(bankId, decimal.Parse(args[4]));
                    break;
                case "creditcommission":
                    BankLogic.ChangeCreditCommission(bankId, decimal.Parse(args[4]));
                    break;
                case "mindepositpercent":
                    BankLogic.ChangeMinDepositPercent(bankId, decimal.Parse(args[4]));
                    break;
                case "deposittime":
                    BankLogic.ChangeDepositTime(bankId, long.Parse(args[4]) * 1000);
                    break;
                case "anonlimit":
                    BankLogic.ChangeAnonLimit(bankId, decimal.Parse(args[4]));
                    break;
                default:
                    return usage;
            }

            return Response("Bank was successfully changed");
        }
    }
}