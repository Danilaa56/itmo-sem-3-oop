using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    public class DataContext : DbContext
    {
        private readonly string _fileName;

        public DataContext(string fileName)
        {
            _fileName = fileName;
            Database.EnsureCreated();
            SaveChanges();
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

        // public decimal AmountAt(int accountId)
        // {
        //     return AmountAt(AccountById(accountId));
        // }

        // public decimal AmountAt(Account account)
        // {
        //     if (!AmountsByAccounts().TryGetValue(account, out decimal amount))
        //     {
        //         throw new BankException("There is no account with such id");
        //     }
        //
        //     return amount;
        // }

        // public Dictionary<Account, decimal> AmountsByAccounts()
        // {
        //     var accountToMoney = new Dictionary<Account, decimal>();
        //     Transactions.ForEach(transaction =>
        //     {
        //         if (!transaction.IsCancelled)
        //             transaction.Process(accountToMoney);
        //     });
        //
        //     return accountToMoney;
        // }

        // public Environment Environment()
        // {
        //     if (!Environments.Any())
        //     {
        //         var environment = new Environment();
        //         Environments.Add(environment);
        //         SaveChanges();
        //         return environment;
        //     }
        //
        //     return Environments.First();
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_fileName}");
        }
    }
}