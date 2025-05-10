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
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        // Subscription & Approval DbSets
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<HostelSubscription> HostelSubscriptions { get; set; }
        public DbSet<HostelApproval> HostelApprovals { get; set; }
        
        // Meal Management DbSets
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<StudentMeal> StudentMeals { get; set; }
        public DbSet<MealPayment> MealPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints
            builder.Entity<Hostel>()
                .HasOne(h => h.Manager)
                .WithOne()
                .HasForeignKey<Hostel>(h => h.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Room>()
                .HasOne(r => r.Hostel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notice>()
                .HasOne(n => n.Hostel)
                .WithMany(h => h.Notices)
                .HasForeignKey(n => n.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
                .HasOne(p => p.Hostel)
                .WithMany(h => h.Payments)
                .HasForeignKey(p => p.HostelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Subscription & Approval relationships
            builder.Entity<HostelSubscription>()
                .HasOne(hs => hs.Hostel)
                .WithMany()
                .HasForeignKey(hs => hs.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<HostelSubscription>()
                .HasOne(hs => hs.SubscriptionPlan)
                .WithMany(sp => sp.HostelSubscriptions)
                .HasForeignKey(hs => hs.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<HostelApproval>()
                .HasOne(ha => ha.Hostel)
                .WithMany()
                .HasForeignKey(ha => ha.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<HostelApproval>()
                .HasOne(ha => ha.Admin)
                .WithMany()
                .HasForeignKey(ha => ha.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Meal Management relationships
            builder.Entity<Meal>()
                .HasOne(m => m.Hostel)
                .WithMany()
                .HasForeignKey(m => m.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<MealPlan>()
                .HasOne(mp => mp.Hostel)
                .WithMany()
                .HasForeignKey(mp => mp.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<StudentMeal>()
                .HasOne(sm => sm.Student)
                .WithMany()
                .HasForeignKey(sm => sm.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<StudentMeal>()
                .HasOne(sm => sm.Meal)
                .WithMany()
                .HasForeignKey(sm => sm.MealId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<MealPayment>()
                .HasOne(mp => mp.Student)
                .WithMany()
                .HasForeignKey(mp => mp.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<MealPayment>()
                .HasOne(mp => mp.Hostel)
                .WithMany()
                .HasForeignKey(mp => mp.HostelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 