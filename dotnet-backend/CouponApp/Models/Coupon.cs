using System.ComponentModel.DataAnnotations;

namespace CouponApp.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public int DiscountAmount { get; set; }
        
        [Required]
        public string DiscountType { get; set; } = string.Empty; // "percentage" or "fixed"
        
        public DateTime? ExpirationDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int CreatedById { get; set; }
        
        // Navigation properties
        public User CreatedBy { get; set; } = new User();
        
        public ICollection<CouponAssignment> Assignments { get; set; } = new List<CouponAssignment>();
    }
}