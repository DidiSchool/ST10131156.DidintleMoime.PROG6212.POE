// Data/RaceDayDbContext.cs
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RaceDayWebAPI.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.ParticipantID, e.CategoryID })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
