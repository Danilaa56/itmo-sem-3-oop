using System;
using System.Collections.Generic;
using Banks.BLL;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public class BankCommand : Command
    {
        private ICli _cli;
        private CommandResponse _usage = Response("bank <register|unregister|list>");

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
                case "subscribe":
                    return Subscribe(args);
                case "getsubscribers":
                    return GetSubscribers(args);
                // case "unsubscribe":
                // return Unsubscribe(args);
                // case "destroy":
                // return Unregister(args);
                // case "list":
                // return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Register(string[] args)
        {
            if ( /*args.Length != 10 ||*/ args.Length != 2)
            {
                // return Response(
                // "bank register NAME DEBIT_PERCENT_FOR_REMAINS CREDIT_LIMIT CREDIT_COMMISSION MIN_DEPOSIT_PERCENT_FOR_REMAINS DEPOSIT_PERCENT_LEVELS DEPOSIT_TIME ANON_LIMIT");
                return Response("bank register");
            }

            // if (args.Length == 10)
            // {
            //
            //     int id = BankLogic.RegisterBank(
            //         args[2],
            //         decimal.Parse(args[3]),
            //         decimal.Parse(args[4]),
            //         decimal.Parse(args[5]),
            //         decimal.Parse(args[6]),
            //         ,
            //         long.Parse(args[8]),
            //         decimal.Parse(args[9]));
            // }
            if (args.Length == 2)
            {
                _cli.WriteLine("Enter data or empty line to cancel");

                _cli.WriteLine("Name: ");
                if (!_cli.Read(out string name))
                    goto cancelled;
                _cli.WriteLine("Percent for remains for debit accounts: ");
                if (!_cli.Read(out decimal debitPercentForRemains))
                    goto cancelled;
                _cli.WriteLine("Credit limit for credit accounts: ");
                if (!_cli.Read(out decimal creditLimit))
                    goto cancelled;
                _cli.WriteLine("Commission for credit accounts: ");
                if (!_cli.Read(out decimal creditCommission))
                    goto cancelled;

                _cli.WriteLine("Percent for remains for deposit accounts");
                _cli.WriteLine("It is defined with levels and minimal value, if amount on account is too small");
                _cli.WriteLine("Each level consist of AMOUNT and PERCENT starting with this amount");
                _cli.WriteLine("Minimal percent: ");
                if (!_cli.Read(out decimal minDepositPercentForRemains))
                    goto cancelled;
                _cli.WriteLine("Levels count (negative count means zero): ");
                if (!_cli.Read(out int levelsCount))
                    goto cancelled;

                var depositLevels = new Dictionary<decimal, decimal>();
                if (depositLevels == null) throw new ArgumentNullException(nameof(depositLevels));
                for (int i = 0; i < levelsCount; i++)
                {
                    _cli.WriteLine("Amount on account: ");
                    if (!_cli.Read(out decimal key))
                        goto cancelled;
                    _cli.WriteLine("Percent on remains: ");
                    if (!_cli.Read(out decimal percent))
                        goto cancelled;
                    depositLevels[key] = percent;
                }

                _cli.WriteLine("Time to unlock deposit accounts (in seconds): ");
                if (!_cli.Read(out long depositTime))
                    goto cancelled;
                _cli.WriteLine("Transaction limit for untrusted accounts: ");
                if (!_cli.Read(out decimal anonLimit))
                    goto cancelled;

                int id = BankLogic.RegisterBank(
                    name,
                    debitPercentForRemains,
                    creditLimit,
                    creditCommission,
                    minDepositPercentForRemains,
                    depositLevels,
                    depositTime,
                    anonLimit);

                return Response($"Bank was created, id - {id}");

                cancelled:
                return Response("Cancelled");
            }

            throw new Exception("Impossible");
        }

        private CommandResponse Subscribe(string[] args)
        {
            if (args.Length != 4)
                return Response("bank subscribe BANK_ID PERSON_ID");

            BankLogic.Subscribe(int.Parse(args[2]), int.Parse(args[3]));
            return Response("Bank was unregistered");
        }

        private CommandResponse GetSubscribers(string[] args)
        {
            if (args.Length != 3)
                return Response("bank getsubscribers BANK_ID");

            List<Person> persons = BankLogic.GetSubscribers(int.Parse(args[2]));

            return Response(Table(persons));
        }

        // private CommandResponse Unregister(string[] args)
        // {
        //     if (args.Length != 3)
        //         return Response("bank unregister NAME");
        //
        //     Bank bank = _context.CentralBank.Banks().Find(bank => bank.Name.Equals(args[2]));
        //     if (bank is null)
        //         return Response($"There is no bank with such name '{args[2]}'");
        //     _context.CentralBank.UnregisterBank(bank);
        //     return Response("Bank was unregistered");
        // }
        //
        // private CommandResponse List(string[] args)
        // {
        //     ImmutableList<Bank> banks = _context.CentralBank.Banks();
        //     return Response(banks.ConvertAll(bank => bank.Name).Insert(0, "Banks count: " + banks.Count));
        // }
        //
        // private Dictionary<decimal, decimal> ReadLevels(string str)
        // {
        //     string[] parts = str.Split(",");
        //     var levels = new Dictionary<decimal, decimal>();
        //     for (int i = 1; i < parts.Length; i += 2)
        //     {
        //         levels[decimal.Parse(parts[i - 1])] = decimal.Parse(parts[i]);
        //     }
        //
        //     return levels;
        // }
    }
}