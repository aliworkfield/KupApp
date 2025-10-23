namespace CouponApp.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
    }
    
    public class CouponCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
    }
    
    public class CouponUpdateDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? DiscountAmount { get; set; }
        public string? DiscountType { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
    }
    
    public class CouponAssignmentDto
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public int UserId { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public string CouponCode { get; set; } = string.Empty;
    }
    
    public class CouponAssignmentCreateDto
    {
        public int CouponId { get; set; }
        public int UserId { get; set; }
    }
}