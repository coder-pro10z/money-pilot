using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyPilot.Domain.Entities;

namespace MoneyPilot.Infrastructure.Data
{
    public class MoneyPilotDbContext: DbContext
    {
        public MoneyPilotDbContext(DbContextOptions<MoneyPilotDbContext> options) : base(options)
        {
        }
        public DbSet<AppUser> Users { get; set; }
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
            // Configure relationships and constraints if needed
            //modelBuilder.Entity<Expense>()
            //    .HasOne(e => e.Category)
            //    .WithMany(c => c.Expenses)
            //    .HasForeignKey(e => e.CategoryId);
            //modelBuilder.Entity<Expense>()
            //    .HasOne(e => e.User)
            //    .WithMany(u => u.Expenses)
            //    .HasForeignKey(e => e.UserId);
            //modelBuilder.Entity<Budget>()
            //    .HasOne(b => b.Category)
            //    .WithMany(c => c.Budgets)
            //    .HasForeignKey(b => b.CategoryId);
            //modelBuilder.Entity<Budget>()
            //    .HasOne(b => b.User)
            //    .WithMany(u => u.Budgets)
            //    .HasForeignKey(b => b.UserId);
        }
    }
}
