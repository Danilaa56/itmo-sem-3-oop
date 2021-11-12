using System;
using Banks.BLL;
using Banks.Entities.Accounts;

namespace Banks.UI.Commands
{
    public class AccountCommand : Command
    {
        private CommandResponse _usage = Response("account <register|change|unregister|list>");

        public override CommandResponse ProcessCommand(string[] args)
        {
            if (args.Length < 2)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "create":
                    return Create(args);
                // case "change":
                //     return Change(args);
                // case "unregister":
                //     return Unregister(args);
                // case "list":
                //     return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Create(string[] args)
        {
            if (args.Length != 5)
                return Response("account create BANK_ID PERSON_ID <credit|debit|deposit>");

            if (!Enum.TryParse(args[4], out BankAccountType accountType))
                return Response("account create BANK_ID PERSON_ID <credit|debit|deposit>");

            int id = AccountLogic.Create(int.Parse(args[2]), int.Parse(args[3]), accountType);
            return Response($"Account was created, id: {id}");
        }

        // private CommandResponse Change(string[] args)
        // {
        //     CommandResponse usage = Response("person change <address|passportId> PERSON_ID [NEW_VALUE]");
        //
        //     if (args.Length < 2 || args.Length > 6)
        //         return usage;
        //
        //     int id = int.Parse(args[3]);
        //     string value = args.Length == 5 ? args[4] : null;
        //     switch (args[2].ToLower())
        //     {
        //         case "address":
        //             PersonLogic.ChangeAddress(id, value);
        //             break;
        //         case "passportid":
        //             PersonLogic.ChangePassportId(id, value);
        //             break;
        //         default:
        //             return usage;
        //     }
        //
        //     return Response($"Successfully changed {args[2].ToLower()}");
        // }
        //
        // private CommandResponse Unregister(string[] args)
        // {
        //     if (args.Length != 3)
        //         return Response("person destroy PERSON_ID");
        //
        //     PersonLogic.Destroy(int.Parse(args[2]));
        //     return Response($"Person with id '{args[2]}' was unregistered");
        // }
        //
        // private CommandResponse List(string[] args)
        // {
        //     List<Person> persons = PersonLogic.List();
        //
        //     var builder = CommandResponse.Builder();
        //     builder.WriteLine($"Persons count: {persons.Count}");
        //     builder.WriteLine(Table(persons));
        //     return builder.Build();
        // }
    }
}