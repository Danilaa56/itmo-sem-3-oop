#nullable enable
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Tools;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Bank> Banks { get; set; } = null!;

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<DebitAccount> DebitAccounts { get; set; } = null!;
        public DbSet<CreditAccount> CreditAccounts { get; set; } = null!;
        public DbSet<DepositAccount> DepositAccounts { get; set; } = null!;

        public DbSet<TransactionCorrection> CorrectionTransactions { get; set; } = null!;
        public DbSet<TransactionCreate> CreateTransactions { get; set; } = null!;
        public DbSet<TransactionDestroy> DestroyTransactions { get; set; } = null!;
        public DbSet<TransactionTopUp> TopUpTransactions { get; set; } = null!;
        public DbSet<TransactionTransfer> TransferTransactions { get; set; } = null!;
        public DbSet<TransactionWithdraw> WithdrawTransactions { get; set; } = null!;

        public DbSet<Environment> Environments { get; set; } = null!;

        public List<BankAccount> BankAccounts
        {
            get
            {
                var list = new List<BankAccount>();
                list.AddRange(DebitAccounts.Include(acc => acc.Account));
                list.AddRange(CreditAccounts.Include(acc => acc.Account));
                list.AddRange(DepositAccounts.Include(acc => acc.Account));
                list.Sort((ta1, ta2) => ta1.Id - ta2.Id);
                return list;
            }
        }

        public List<Transaction> Transactions
        {
            get
            {
                var list = new List<Transaction>();
                list.AddRange(CorrectionTransactions.Include(ta => ta.Account));
                list.AddRange(CreateTransactions.Include(ta => ta.Account));
                list.AddRange(DestroyTransactions.Include(ta => ta.Account));
                list.AddRange(TopUpTransactions.Include(ta => ta.Account));
                list.AddRange(TransferTransactions.Include(ta => ta.Account));
                list.AddRange(WithdrawTransactions.Include(ta => ta.Account));
                list.Sort((ta1, ta2) => ta1.Id - ta2.Id);
                return list;
            }
        }

        public int NextTransactionId()
        {
            Transaction? lastTransaction = Transactions.LastOrDefault();
            if (lastTransaction is null)
                return 1;
            return lastTransaction.Id + 1;
        }

        public decimal AmountAt(int accountId)
        {
            return AmountAt(AccountById(accountId));
        }

        public decimal AmountAt(Account account)
        {
            if (!AmountsByAccounts().TryGetValue(account, out decimal amount))
            {
                throw new BankException("There is no account with such id");
            }

            return amount;
        }

        public Dictionary<Account, decimal> AmountsByAccounts()
        {
            var accountToMoney = new Dictionary<Account, decimal>();
            Transactions.ForEach(transaction =>
            {
                if (!transaction.IsCancelled)
                    transaction.Process(accountToMoney);
            });

            return accountToMoney;
        }

        public Account AccountById(int id)
        {
            return Accounts.FirstOrDefault(account => account.Id == id) ??
                   throw new BankException("There is no account with such id");
        }

        public Environment Environment()
        {
            if (!Environments.Any())
            {
                var environment = new Environment();
                Environments.Add(environment);
                SaveChanges();
                return environment;
            }

            return Environments.First();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // base("server=localhost;port=3306;database=wordpress;uid=root;password=") not work
            // optionsBuilder.UseInMemoryDatabase("BanksDB");
            // optionsBuilder.UseMySQL("");
            optionsBuilder.UseSqlite("Filename=banks.db");
        }
    }
}