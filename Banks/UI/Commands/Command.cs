using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Banks.Entities;

namespace Banks.UI.Commands
{
    public abstract class Command
    {
        public abstract CommandResponse ProcessCommand(string[] args);

        protected static CommandResponse Response(bool shouldExit = false)
        {
            return new CommandResponse(Array.Empty<string>(), shouldExit);
        }

        protected static CommandResponse Response(string msg, bool shouldExit = false)
        {
            return new CommandResponse(new string[] { msg }, shouldExit);
        }

        protected static CommandResponse Response(IEnumerable<string> msgLines, bool shouldExit = false)
        {
            return new CommandResponse(msgLines, shouldExit);
        }

        protected static string[] Table(IEnumerable<Person> persons)
        {
            return Table(
                new string[] { "Id", "Name", "Surname", "Address", "Passport Id" },
                persons.Select(person => new string[]
                {
                    person.Id.ToString(), person.Name, person.Surname, person.Address, person.PassportId,
                }));
        }

        protected static string[] Table(IEnumerable<Bank> banks)
        {
            return Table(
                new string[]
                {
                    "Id", "Name", "Debit percent", "Credit commission", "Credit limit",
                    "Min deposit percent", "Deposit time", "Anon limit",
                },
                banks.Select(bank => new string[]
                {
                    bank.Id.ToString(),
                    bank.Name,
                    bank.DebitPercentForRemains.ToString(),
                    bank.CreditCommission.ToString(),
                    bank.CreditLimit.ToString(),
                    bank.MinDepositPercentForRemains.ToString(),
                    (bank.DepositTimeMs / 1000).ToString(),
                    bank.AnonLimit.ToString(),
                }));
        }

        protected static string[] Table(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> cells)
        {
            var lines = new List<IEnumerable<string>> { headers };
            lines.AddRange(cells);
            return Table(lines);
        }

        protected static string[] Table(IEnumerable<IEnumerable<string>> cells)
        {
            string[][] table = cells.Select(enumerable => enumerable.ToArray()).ToArray();
            foreach (string[] column in table)
            {
                for (int j = 0; j < column.Length; j++)
                {
                    column[j] ??= "null";
                }
            }

            string spacing = "   ";

            int[] columnWidths = new int[table[0].Length];
            for (int i = 0; i < columnWidths.Length; i++)
            {
                columnWidths[i] = table.Select(column => column[i]).Max(str => str.Length);
            }

            int width = columnWidths.Sum() + 2 + ((columnWidths.Length + 1) * spacing.Length);

            string[] lines = new string[table.Length + 2];
            var stringBuilder = new StringBuilder(width);

            for (int i = 0; i < width; i++)
                stringBuilder.Append('=');

            lines[0] = stringBuilder.ToString();
            lines[^1] = lines[0];

            for (int i = 0; i < table.Length; i++)
            {
                stringBuilder.Clear();
                stringBuilder.Append('=');
                for (int j = 0; j < table[i].Length; j++)
                {
                    stringBuilder.Append(spacing);
                    stringBuilder.Append(table[i][j]);
                    for (int k = table[i][j].Length; k < columnWidths[j]; k++)
                    {
                        stringBuilder.Append(' ');
                    }
                }

                stringBuilder.Append(spacing).Append('=');
                lines[i + 1] = stringBuilder.ToString();
            }

            return lines;
        }
    }
}