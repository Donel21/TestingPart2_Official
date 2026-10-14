using Microsoft.EntityFrameworkCore;
using TestingPart2_Official.Models;

namespace TestingPart2_Official.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventRoute> Routes { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // [User] is reserved in SQL Server — must be bracketed in SQL
            modelBuilder.Entity<User>().ToTable("User");

            // Route table is singular per ERD / SQL script
            modelBuilder.Entity<EventRoute>().ToTable("Route");

            // ---------- USER ----------
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_User_Role",
                    "[Role] IN ('Organiser','Participant')"));

            // ---------- EVENT ----------
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- CATEGORY ----------
            modelBuilder.Entity<Category>()
                .Property(c => c.DistanceKm).HasPrecision(5, 2);

            modelBuilder.Entity<Category>()
                .Property(c => c.EntryFee).HasPrecision(10, 2);

            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.EventID, c.CategoryName })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .ToTable(t => t.HasCheckConstraint("CK_Category_Distance", "[DistanceKm] > 0"));

            modelBuilder.Entity<Category>()
                .ToTable(t => t.HasCheckConstraint("CK_Category_EntryFee", "[EntryFee] >= 0"));

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- ROUTE ----------
            modelBuilder.Entity<EventRoute>()
                .Property(r => r.RouteDistanceKm).HasPrecision(5, 2);

            modelBuilder.Entity<EventRoute>()
                .HasIndex(r => r.EventID)
                .IsUnique();   // one route per event

            modelBuilder.Entity<EventRoute>()
                .ToTable(t => t.HasCheckConstraint("CK_Route_Distance", "[RouteDistanceKm] > 0"));

            modelBuilder.Entity<EventRoute>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Routes)
                .HasForeignKey(r => r.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- ENROLLMENT ----------
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.ParticipantID, e.CategoryID })
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Enrollment_Status",
                    "[Status] IN ('Pending','Confirmed','Cancelled')"));

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Participant)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- RESULT ----------
            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrollmentID)
                .IsUnique();

            modelBuilder.Entity<Result>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Result_Position",
                    "[Position] IS NULL OR [Position] > 0"));

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrollment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrollmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}