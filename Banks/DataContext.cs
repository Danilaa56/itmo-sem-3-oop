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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_fileName}");
        }
    }
}