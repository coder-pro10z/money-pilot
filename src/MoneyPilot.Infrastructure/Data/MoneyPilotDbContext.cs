using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Infrastructure.Data
{
    public class MoneyPilotDbContext: IdentityDbContext<AppUser>
    {
        public MoneyPilotDbContext(DbContextOptions<MoneyPilotDbContext> options) : base(options)
        {
        }
        //public DbSet<AppUser> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>()
        .Property(e => e.Amount)
        .HasPrecision(18, 2);

            modelBuilder.Entity<Budget>()
                .Property(b => b.MonthlyLimit)
                .HasPrecision(18, 2);
         
        }
    }
}
