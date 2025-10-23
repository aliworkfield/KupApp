using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CouponApp.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        
        public UserRole Role { get; set; } = UserRole.User;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [JsonIgnore]
        public ICollection<Coupon> CreatedCoupons { get; set; } = new List<Coupon>();
        
        [JsonIgnore]
        public ICollection<CouponAssignment> AssignedCoupons { get; set; } = new List<CouponAssignment>();
    }
}