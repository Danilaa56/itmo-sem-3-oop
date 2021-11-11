using Banks.DTO;
using Banks.Entities;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    public class BanksDbContext : DbContext
    {
        public BanksDbContext()
        {
            Database.EnsureCreated();
        }

        // public DbSet<Transaction> Transactions { get; set; }
        // // public DbSet<Bank> Banks { get; set; }
        // public DbSet<Person> Persons { get; set; }
        // public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<PersonBankSubscriber> Subscribers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // DbSetInitializer(new Ini<BanksDbContext>())
            // SetInitializer();
            // base("server=localhost;port=3306;database=wordpress;uid=root;password=") not work
            // optionsBuilder.UseInMemoryDatabase("BanksDB");
            optionsBuilder.UseMySQL("server=localhost;port=3306;database=BanksDB;uid=root;password=Welcometo56");
        }
    }
}