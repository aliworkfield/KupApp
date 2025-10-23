namespace CouponApp.Models
{
    public class CouponAssignment
    {
        public int Id { get; set; }
        
        public int CouponId { get; set; }
        
        public int UserId { get; set; }
        
        public bool IsUsed { get; set; } = false;
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UsedAt { get; set; }
        
        // Navigation properties
        public Coupon Coupon { get; set; } = new Coupon();
        
        public User User { get; set; } = new User();
    }
}