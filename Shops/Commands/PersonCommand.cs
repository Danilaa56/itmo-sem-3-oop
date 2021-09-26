using System.Collections.Immutable;
using System.Linq;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Commands
{
    public class PersonCommand : Command
    {
        private Context _context;
        private string[] _usage = Response("person <create|destroy|list|money>");

        public PersonCommand(Context context)
        {
            _context = context;
        }

        public override string[] ProcCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            try
            {
                switch (args[1].ToLower())
                {
                    case "create":
                        return Create(args);
                    case "destroy":
                        return Destroy(args);
                    case "list":
                        return List(args);
                    case "money":
                        return Money(args);
                }
            }
            catch (ShopException e)
            {
                return Response(e.Message);
            }

            return _usage;
        }

        private string[] Create(string[] args)
        {
            if (args.Length < 4 || args.Length > 5)
                return Response("person create PERSON_ID PERSON_NAME [MONEY]");

            decimal money = args.Length == 4 ? 0 : decimal.Parse(args[4]);

            var person = new Person(args[2], args[3], money);
            _context.PeopleRegistry.Register(person);
            return Response("Person was created");
        }

        private string[] Destroy(string[] args)
        {
            if (args.Length != 3)
                return Response("shop destroy PERSON_ID");

            _context.PeopleRegistry.Unregister(args[2]);
            return Response($"Person '{args[2]}' was destroyed");
        }

        private string[] List(string[] args)
        {
            ImmutableList<Person> people = _context.PeopleRegistry.GetPeople();
            return people.ConvertAll(person => person.Id + "\t" + person.Name + "\t" + person.Money)
                .Insert(0, "People count: " + people.Count).ToArray();
        }

        private string[] Money(string[] args)
        {
            if (args.Length == 2)
                return Response("person money <get|set>");
            switch (args[2].ToLower())
            {
                case "get":
                    if (args.Length == 3)
                        return Response("person money get PERSON_ID");
                    return Response($"{_context.PeopleRegistry.GetById(args[3]).Money}");
                case "set":
                    if (args.Length < 5)
                        return Response("person money set PERSON_ID VALUE");
                    _context.PeopleRegistry.GetById(args[3]).Money = decimal.Parse(args[4]);
                    return Response("Amount of money was set");
            }

            return Response("person money <get|set>");
        }
    }
}