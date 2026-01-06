using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using TacticalSentry.Core.Entities;

namespace TacticalSentry.Infrastructure.Data
{
    public class TacticalDbContext : DbContext
    {
        public DbSet<OperatorUser> Users { get; set; }
        public DbSet<SecurityLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tactical.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperatorUser>().HasData(
                new OperatorUser
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Password = "123", 
                    ClearanceLevel = 5,
                    ServiceNumber = "CMD-001"
                }
            );
        }
    }
}