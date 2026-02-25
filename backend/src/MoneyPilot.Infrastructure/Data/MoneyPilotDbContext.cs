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

        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Expense>()
                    .Property(e => e.Amount)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Budget>()
                    .Property(b => b.MonthlyLimit)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Expense>()
                    .HasOne(e => e.User)
                    .WithMany(u => u.Expenses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Budget>()
                    .HasOne(b => b.User)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                    // Global query filters for soft delete
                    modelBuilder.Entity<Expense>().HasQueryFilter(e => !e.IsDeleted);
                    modelBuilder.Entity<Budget>().HasQueryFilter(b => !b.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            //modelBuilder.Entity<Category>().
            //                               .HasOne(r => r.Category) 
            //                               .WithMany()
            //                               .IsRequired(false);


            //Configuring RecurringTransaction relationships
            modelBuilder.Entity<RecurringTransaction>(entity => {
                //set precision for Amount
                entity.Property(rt => rt.Amount)
                .HasPrecision(18, 2);


                // Store enum as string in database (keeps DB readable)
                //entity.Property(rt => rt.RecurrenceType)
                //    .HasConversion<string>()
                //    .HasMaxLength(20);
                // Store enum as string in database
                entity.Property(rt => rt.RecurrenceType)
                    .HasConversion<string>()
                    .HasMaxLength(20);


                // Store DayOfWeek as string
                entity.Property(rt => rt.DayOfWeek)
                    .HasConversion<string?>()
                    .HasMaxLength(15);

                //configure relationship with AppUser
                entity.HasOne(rt => rt.User)
                .WithMany(u => u.RecurringTransactions)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                //configure relationship with Category
                entity.HasOne(rt => rt.Category)
      .WithMany(c => c.RecurringTransactions)
      .HasForeignKey(rt => rt.CategoryId)
      .IsRequired(false)  // make it optional
      .OnDelete(DeleteBehavior.Restrict);


                //making relationship with Category optional


                // Optional: Add index for performance
                entity.HasIndex(rt => new { rt.UserId, rt.NextOccurrence, rt.IsActive });
            });  



        }
    }
}
