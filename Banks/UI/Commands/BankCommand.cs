using System.Collections.Generic;
using System.Collections.Immutable;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public class BankCommand : Command
    {
        private Context _context;
        private CommandResponse _usage = Response("bank <register|list|unregister>");

        public BankCommand(Context context)
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
                case "register":
                    return Register(args);
                case "destroy":
                    return Unregister(args);
                case "list":
                    return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Register(string[] args)
        {
            if (args.Length != 10)
            {
                return Response(
                    "bank register NAME DEBIT_PERCENT_FOR_REMAINS CREDIT_LIMIT CREDIT_COMMISSION MIN_DEPOSIT_PERCENT_FOR_REMAINS DEPOSIT_PERCENT_LEVELS DEPOSIT_TIME ANON_LIMIT");
            }

            var bank = new Bank(
                _context.CentralBank,
                args[2],
                decimal.Parse(args[3]),
                decimal.Parse(args[4]),
                decimal.Parse(args[5]),
                decimal.Parse(args[6]),
                ReadLevels(args[7]),
                long.Parse(args[8]),
                decimal.Parse(args[9]));

            return Response("Bank was created");
        }

        private CommandResponse Unregister(string[] args)
        {
            if (args.Length != 3)
                return Response("bank unregister NAME");

            Bank bank = _context.CentralBank.Banks().Find(bank => bank.Name.Equals(args[2]));
            if (bank is null)
                return Response($"There is no bank with such name '{args[2]}'");
            _context.CentralBank.UnregisterBank(bank);
            return Response("Bank was unregistered");
        }

        private CommandResponse List(string[] args)
        {
            ImmutableList<Bank> banks = _context.CentralBank.Banks();
            return Response(banks.ConvertAll(bank => bank.Name).Insert(0, "Banks count: " + banks.Count));
        }

        private Dictionary<decimal, decimal> ReadLevels(string str)
        {
            string[] parts = str.Split(",");
            var levels = new Dictionary<decimal, decimal>();
            for (int i = 1; i < parts.Length; i += 2)
            {
                levels[decimal.Parse(parts[i - 1])] = decimal.Parse(parts[i]);
            }

            return levels;
        }
    }
}