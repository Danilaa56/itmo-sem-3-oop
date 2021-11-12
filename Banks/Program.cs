using System;
using System.Collections.Generic;
using Banks.Tools;
using Banks.UI;
using Banks.UI.Commands;

namespace Banks
{
    internal class Program
    {
        private static ICli _cli = new Cli();

        private static Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        private static void Main()
        {
            using (var db = new DataContext())
            {
                db.Database.EnsureCreated();
                // Console.WriteLine(db.Banks.ToList().First().Subscribers.Count);
            }

            _commands["bank"] = new BankCommand(_cli);
            _commands["person"] = new PersonCommand();
            // _commands["time"] = new TimeCommand(_context);
            _commands["quit"] = new QuitCommand();

            PrintWelcomeMessage();

            while (true)
            {
                string[] args = ReadCommand();
                if (args.Length == 0)
                    continue;
                string commandName = args[0].ToLower();
                if (_commands.TryGetValue(commandName, out Command command))
                {
                    try
                    {
                        CommandResponse response = command.ProcessCommand(args);
                        foreach (string responseLine in response.Lines)
                            _cli.WriteLine(responseLine);
                        if (response.ShouldExit)
                            break;
                    }
                    catch (BankException e)
                    {
                        _cli.WriteLine(e.Message);
                        _cli.WriteLine(e.StackTrace);
                    }
                }
                else
                {
                    _cli.WriteLine("Unknown command. Try to use one of them:");
                    PrintCommands();
                }
            }

            // _context.Dispose();
        }

        private static string[] ReadCommand()
        {
            string line = null;
            while (line == null)
                line = Console.ReadLine();
            char[] chars = line.ToCharArray();

            var list = new List<string>();

            char[] charsCache = new char[chars.Length];
            int cacheIndex = 0;

            bool passSpace = false;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == ' ' && !passSpace)
                {
                    if (cacheIndex != 0)
                    {
                        list.Add(new string(charsCache, 0, cacheIndex));
                        cacheIndex = 0;
                    }
                }
                else if (c == '"')
                {
                    passSpace = !passSpace;
                }
                else
                {
                    charsCache[cacheIndex++] = c;
                }
            }

            if (cacheIndex != 0)
                list.Add(new string(charsCache, 0, cacheIndex));

            return list.ToArray();
        }

        private static void PrintWelcomeMessage()
        {
            _cli.WriteLine("Welcome to...");
            _cli.WriteLine("================================================================================");
            _cli.WriteLine("==                                                                            ==");
            _cli.WriteLine("==     =====       ======   ===   ==   ==  ==    ======                       ==");
            _cli.WriteLine("==     ==  ==     ==   ==   ====  ==   == ==    ==    ==                      ==");
            _cli.WriteLine("==     ==  ==    ==    ==   ====  ==   ====     ==                            ==");
            _cli.WriteLine("==     ======    ========   === = ==   ===       ======                       ==");
            _cli.WriteLine("==     ==   ==   ==    ==   ==   ===   ====           ==                      ==");
            _cli.WriteLine("==     ==   ==   ==    ==   ==   ===   == ==    ==    ==                      ==");
            _cli.WriteLine("==     ======   ===    ==   ==    ==   ==  ==    ======                       ==");
            _cli.WriteLine("==                                                                            ==");
            _cli.WriteLine("================================================================================");
            _cli.WriteLine("These commands are available:");
            PrintCommands();
        }

        private static void PrintCommands()
        {
            foreach (string command in _commands.Keys)
            {
                _cli.WriteLine(" - " + command);
            }
        }
    }
}