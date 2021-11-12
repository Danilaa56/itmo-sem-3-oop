using Banks.Entities;
using Banks.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            // Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        // public DbSet<Transaction> Transactions { get; set; }
        // // public DbSet<Bank> Banks { get; set; }
        // public DbSet<Person> Persons { get; set; }
        // public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Bank> Banks { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<DebitAccount> DebitAccounts { get; set; }
        public DbSet<CreditAccount> CreditAccounts { get; set; }
        public DbSet<DepositAccount> DepositAccounts { get; set; }

        public long CurrentTimeMillis { get; set; } = 0;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DbSetInitializer(new Ini<BanksDbContext>())
            // SetInitializer();
            // base("server=localhost;port=3306;database=wordpress;uid=root;password=") not work
            // optionsBuilder.UseInMemoryDatabase("BanksDB");
            // optionsBuilder.UseMySQL("");
            optionsBuilder.UseSqlite("Filename=banks.db");
        }
    }
}