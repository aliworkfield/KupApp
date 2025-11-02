using Microsoft.EntityFrameworkCore;
using CouponApp.Models;

namespace CouponApp.Data
{
    public class CouponAppContext : DbContext
    {
        public CouponAppContext(DbContextOptions<CouponAppContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponAssignment> CouponAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Configure Coupon entity
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.DiscountType).IsRequired();
                entity.Property(e => e.Brand).HasDefaultValue("");
                entity.Property(e => e.AssignmentTitle).HasDefaultValue("");
                
                // Configure relationship with User
                entity.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.CreatedCoupons)
                    .HasForeignKey(c => c.CreatedById)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CouponAssignment entity
            modelBuilder.Entity<CouponAssignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configure relationships
                entity.HasOne(ca => ca.Coupon)
                    .WithMany(c => c.Assignments)
                    .HasForeignKey(ca => ca.CouponId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(ca => ca.User)
                    .WithMany(u => u.AssignedCoupons)
                    .HasForeignKey(ca => ca.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Ensure a user can't have the same coupon assigned multiple times
                entity.HasIndex(ca => new { ca.CouponId, ca.UserId }).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}