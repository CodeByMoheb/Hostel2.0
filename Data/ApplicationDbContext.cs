using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Models;
using Hostel2._0.Models.MealManagement;

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
        public DbSet<Notice> Notices { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<StudentMeal> StudentMeals { get; set; }
        public DbSet<MealPayment> MealPayments { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<HostelSubscription> HostelSubscriptions { get; set; }
        public DbSet<HostelApproval> HostelApprovals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Hostel
            modelBuilder.Entity<Hostel>()
                .Property(h => h.MonthlyFee)
                .HasPrecision(18, 2);

            // Configure decimal precision for HostelSubscription
            modelBuilder.Entity<HostelSubscription>()
                .Property(hs => hs.Amount)
                .HasPrecision(18, 2);

            // Configure decimal precision for Meal
            modelBuilder.Entity<Meal>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            // Configure decimal precision for MealPayment
            modelBuilder.Entity<MealPayment>()
                .Property(mp => mp.Amount)
                .HasPrecision(18, 2);

            // Configure decimal precision for MealPlan
            modelBuilder.Entity<MealPlan>()
                .Property(mp => mp.Price)
                .HasPrecision(18, 2);

            // Configure decimal precision for Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure decimal precision for Room
            modelBuilder.Entity<Room>()
                .Property(r => r.MonthlyRent)
                .HasPrecision(18, 2);

            // Configure decimal precision for SubscriptionPlan
            modelBuilder.Entity<SubscriptionPlan>()
                .Property(sp => sp.MonthlyPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SubscriptionPlan>()
                .Property(sp => sp.YearlyPrice)
                .HasPrecision(18, 2);

            // Configure decimal precision for StudentMeal
            modelBuilder.Entity<StudentMeal>()
                .Property(sm => sm.Rate)
                .HasPrecision(18, 2);

            // Configure relationships
            modelBuilder.Entity<Hostel>()
                .HasOne(h => h.Manager)
                .WithOne()
                .HasForeignKey<Hostel>(h => h.ManagerId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hostel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HostelId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Hostel)
                .WithMany(h => h.Students)
                .HasForeignKey(s => s.HostelId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Occupants)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.Hostel)
                .WithMany(h => h.Notices)
                .HasForeignKey(n => n.HostelId);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Hostel)
                .WithMany(h => h.MaintenanceRequests)
                .HasForeignKey(m => m.HostelId);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Room)
                .WithMany(r => r.MaintenanceRequests)
                .HasForeignKey(m => m.RoomId);

            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Student)
                .WithMany(s => s.MaintenanceRequests)
                .HasForeignKey(m => m.StudentId);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Hostel)
                .WithMany(h => h.Documents)
                .HasForeignKey(d => d.HostelId);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Student)
                .WithMany(s => s.Documents)
                .HasForeignKey(d => d.StudentId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Hostel)
                .WithMany(h => h.Payments)
                .HasForeignKey(p => p.HostelId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StudentId);

            modelBuilder.Entity<Meal>()
                .HasOne(m => m.Hostel)
                .WithMany(h => h.Meals)
                .HasForeignKey(m => m.HostelId);

            modelBuilder.Entity<MealPlan>()
                .HasOne(mp => mp.Hostel)
                .WithMany(h => h.MealPlans)
                .HasForeignKey(mp => mp.HostelId);

            modelBuilder.Entity<StudentMeal>()
                .HasOne(sm => sm.Student)
                .WithMany(s => s.StudentMeals)
                .HasForeignKey(sm => sm.StudentId);

            modelBuilder.Entity<StudentMeal>()
                .HasOne(sm => sm.Meal)
                .WithMany(m => m.StudentMeals)
                .HasForeignKey(sm => sm.MealId);

            modelBuilder.Entity<MealPayment>()
                .HasOne(mp => mp.StudentMeal)
                .WithMany(sm => sm.Payments)
                .HasForeignKey(mp => mp.StudentMealId);

            modelBuilder.Entity<HostelSubscription>()
                .HasOne(hs => hs.Hostel)
                .WithMany(h => h.Subscriptions)
                .HasForeignKey(hs => hs.HostelId);

            modelBuilder.Entity<HostelSubscription>()
                .HasOne(hs => hs.SubscriptionPlan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(hs => hs.SubscriptionPlanId);
        }
    }
} 