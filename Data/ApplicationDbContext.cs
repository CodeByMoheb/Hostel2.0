using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hostel> Hostels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<HostelApproval> HostelApprovals { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ManagedHostels)
                .WithOne(h => h.Manager)
                .HasForeignKey(h => h.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Hostel relationships
            modelBuilder.Entity<Hostel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hostel)
                .HasForeignKey(r => r.HostelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hostel>()
                .HasMany(h => h.Students)
                .WithOne(s => s.Hostel)
                .HasForeignKey(s => s.HostelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Room relationships
            modelBuilder.Entity<Room>()
                .HasMany(r => r.MaintenanceRequests)
                .WithOne(m => m.Room)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.Students)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure HostelApproval relationships
            modelBuilder.Entity<HostelApproval>()
                .HasOne(h => h.Hostel)
                .WithOne()
                .HasForeignKey<HostelApproval>("HostelId")
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Activity relationships
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Hostel)
                .WithMany()
                .HasForeignKey(a => a.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure AttendanceRecord relationships
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            modelBuilder.Entity<Hostel>()
                .Property(h => h.MonthlyFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Room>()
                .Property(r => r.MonthlyRent)
                .HasPrecision(18, 2);
        }
    }
} 