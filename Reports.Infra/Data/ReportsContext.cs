using System;
using Microsoft.EntityFrameworkCore;
using Reports.Core.Entities;

namespace Reports.Infra.Data
{
    public class ReportsContext : DbContext
    {
        public ReportsContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}