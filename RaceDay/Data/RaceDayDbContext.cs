using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RaceDay.Models;

namespace RaceDay.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Organizer> Organizers => Set<Organizer>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Organizer>()
                .HasIndex(o => o.UserID)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.UserID, e.EventID })
                .IsUnique();

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrollmentID)
                .IsUnique();

            modelBuilder.Entity<Organizer>()
                .HasOne(o => o.User)
                .WithOne()
                .HasForeignKey<Organizer>(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrollments)
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrollment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrollmentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Organizer)
                .WithMany(o => o.Results)
                .HasForeignKey(r => r.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}