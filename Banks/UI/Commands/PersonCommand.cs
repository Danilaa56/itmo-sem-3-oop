using System.Collections.Immutable;
using System.Linq;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public class PersonCommand : Command
    {
        private Context _context;
        private CommandResponse _usage = Response("person <create|destroy|list>");

        public PersonCommand(Context context)
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
                case "create":
                    return Create(args);
                case "destroy":
                    return Destroy(args);
                case "list":
                    return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Create(string[] args)
        {
            if (args.Length < 4 || args.Length > 6)
                return Response("person create PERSON_NAME PERSON_SURNAME [ADDRESS] [PASSPORT_ID]");

            Person.PersonBuilder builder = Person.Builder(args[2], args[3]);

            if (args.Length >= 5)
                builder.Address(args[4]);
            if (args.Length >= 6)
                builder.PassportId(args[5]);

            _context.PeopleRegistry.Register(builder.Build());
            return Response("Person was created");
        }

        private CommandResponse Destroy(string[] args)
        {
            if (args.Length != 3)
                return Response("person destroy INDEX");

            _context.PeopleRegistry.Unregister(int.Parse(args[2]));
            return Response($"Person '{args[2]}' was destroyed");
        }

        private CommandResponse List(string[] args)
        {
            ImmutableList<Person> persons = _context.PeopleRegistry.Persons();
            return Response(
                persons.ConvertAll(person =>
                        person.Name + "\t" + person.Surname + "\t" + person.Address + "\t" + person.PassportId)
                    .Insert(0, "People count: " + persons.Count).ToArray());
        }
    }
}