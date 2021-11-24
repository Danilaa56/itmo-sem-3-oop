using System;
using System.Collections.Generic;
using Banks.BLL;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public class PersonCommand : Command
    {
        private readonly CommandResponse _usage = Response("person <register|change|unregister|list>");
        private readonly ApplicationContext _context;

        public PersonCommand(ApplicationContext context)
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
                case "register":
                    return Register(args);
                case "change":
                    return Change(args);
                case "unregister":
                    return Unregister(args);
                case "list":
                    return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Register(string[] args)
        {
            if (args.Length is < 4 or > 6)
                return Response("person register PERSON_NAME PERSON_SURNAME [ADDRESS] [PASSPORT_ID]");

            string address = args.Length >= 5 ? args[4] : null;
            string passportId = args.Length >= 6 ? args[5] : null;

            Guid newPersonId = _context.Person.Create(args[2], args[3], address, passportId);
            return Response($"Person was created, id: {newPersonId}");
        }

        private CommandResponse Change(string[] args)
        {
            CommandResponse usage = Response("person change <address|passportId> PERSON_ID [NEW_VALUE]");

            if (args.Length < 2 || args.Length > 6)
                return usage;

            var id = Guid.Parse(args[3]);
            string value = args.Length == 5 ? args[4] : null;
            switch (args[2].ToLower())
            {
                case "address":
                    _context.Person.ChangeAddress(id, value);
                    break;
                case "passportid":
                    _context.Person.ChangePassportId(id, value);
                    break;
                default:
                    return usage;
            }

            return Response($"Successfully changed {args[2].ToLower()}");
        }

        private CommandResponse Unregister(string[] args)
        {
            if (args.Length != 3)
                return Response("person unregister PERSON_ID");

            _context.Person.Destroy(Guid.Parse(args[2]));
            return Response($"Person with id '{args[2]}' was unregistered");
        }

        private CommandResponse List(string[] args)
        {
            if (args.Length != 2)
                return Response("person list");
            List<Person> persons = _context.Person.List();

            var builder = CommandResponse.Builder();
            builder.WriteLine($"Persons count: {persons.Count}");
            builder.WriteLine(Table(persons));
            return builder.Build();
        }
    }
}