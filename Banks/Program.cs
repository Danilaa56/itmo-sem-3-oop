using System;
using System.Collections.Generic;
using Banks.BLL;
using Banks.Tools;
using Banks.UI;
using Banks.UI.Commands;

namespace Banks
{
    internal class Program
    {
        private static readonly ICli Cli = new Cli();
        private static readonly ApplicationContext Context = new ApplicationContext("banks.db");

        private static readonly Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        private static void Main()
        {
            Commands["account"] = new AccountCommand(Context);
            Commands["bank"] = new BankCommand(Context, Cli);
            Commands["default"] = new DefaultCommand(Context);
            Commands["person"] = new PersonCommand(Context);
            Commands["quit"] = new QuitCommand(Context);
            Commands["reset"] = new ResetCommand(Context);
            Commands["time"] = new TimeCommand(Context);
            Commands["transaction"] = new TransactionCommand(Context);

            PrintWelcomeMessage();

            while (true)
            {
                string[] args = ReadCommand();
                if (args.Length == 0)
                    continue;
                string commandName = args[0].ToLower();
                if (Commands.TryGetValue(commandName, out Command command))
                {
                    try
                    {
                        CommandResponse response = command.ProcessCommand(args);
                        foreach (string responseLine in response.Lines)
                            Cli.WriteLine(responseLine);
                        if (response.ShouldExit)
                            break;
                    }
                    catch (BankException e)
                    {
                        Cli.WriteLine(e.Message);
                        Cli.WriteLine(e.StackTrace);
                    }
                }
                else
                {
                    Cli.WriteLine("Unknown command. Try to use one of them:");
                    PrintCommands();
                }
            }
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
            Cli.WriteLine("Welcome to...");
            Cli.WriteLine("================================================================================");
            Cli.WriteLine("==                                                                            ==");
            Cli.WriteLine("==             =====       ======   ===   ==   ==  ==    ======               ==");
            Cli.WriteLine("==             ==  ==     ==   ==   ====  ==   == ==    ==    ==              ==");
            Cli.WriteLine("==             ==  ==    ==    ==   ====  ==   ====     ==                    ==");
            Cli.WriteLine("==             ======    ========   === = ==   ===       ======               ==");
            Cli.WriteLine("==             ==   ==   ==    ==   ==   ===   ====           ==              ==");
            Cli.WriteLine("==             ==   ==   ==    ==   ==   ===   == ==    ==    ==              ==");
            Cli.WriteLine("==             ======   ===    ==   ==    ==   ==  ==    ======               ==");
            Cli.WriteLine("==                                                                            ==");
            Cli.WriteLine("================================================================================");
            Cli.WriteLine("These commands are available:");
            PrintCommands();
        }

        private static void PrintCommands()
        {
            foreach (string command in Commands.Keys)
            {
                Cli.WriteLine(" - " + command);
            }
        }
    }
}