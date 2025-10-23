using Microsoft.EntityFrameworkCore;
using CouponApp.Data;
using CouponApp.Models;
using CouponApp.DTOs;

namespace CouponApp.Services
{
    public class CouponService
    {
        private readonly CouponAppContext _context;

        public CouponService(CouponAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coupon>> GetAllCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon?> GetCouponById(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task<Coupon> CreateCoupon(CouponCreateDto couponDto, int createdById)
        {
            var coupon = new Coupon
            {
                Code = couponDto.Code,
                Description = couponDto.Description,
                DiscountAmount = couponDto.DiscountAmount,
                DiscountType = couponDto.DiscountType,
                ExpirationDate = couponDto.ExpirationDate,
                CreatedById = createdById
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon?> UpdateCoupon(int id, CouponUpdateDto couponDto)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return null;

            if (couponDto.Code != null)
                coupon.Code = couponDto.Code;
            if (couponDto.Description != null)
                coupon.Description = couponDto.Description;
            if (couponDto.DiscountAmount.HasValue)
                coupon.DiscountAmount = couponDto.DiscountAmount.Value;
            if (couponDto.DiscountType != null)
                coupon.DiscountType = couponDto.DiscountType;
            if (couponDto.ExpirationDate.HasValue)
                coupon.ExpirationDate = couponDto.ExpirationDate.Value;
            if (couponDto.IsActive.HasValue)
                coupon.IsActive = couponDto.IsActive.Value;

            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CouponAssignment> AssignCouponToUser(CouponAssignmentCreateDto assignmentDto)
        {
            var assignment = new CouponAssignment
            {
                CouponId = assignmentDto.CouponId,
                UserId = assignmentDto.UserId
            };

            _context.CouponAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<IEnumerable<CouponAssignment>> GetUserCoupons(int userId)
        {
            return await _context.CouponAssignments
                .Include(ca => ca.Coupon)
                .Where(ca => ca.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CouponAssignment>> GetUnusedUserCoupons(int userId)
        {
            return await _context.CouponAssignments
                .Include(ca => ca.Coupon)
                .Where(ca => ca.UserId == userId && !ca.IsUsed)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetManagerCoupons(int managerId)
        {
            return await _context.Coupons
                .Where(c => c.CreatedById == managerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetUnassignedCoupons()
        {
            // Get all coupon IDs that have been assigned
            var assignedCouponIds = await _context.CouponAssignments
                .Select(ca => ca.CouponId)
                .Distinct()
                .ToListAsync();

            // Return coupons that are not in the assigned list
            return await _context.Coupons
                .Where(c => !assignedCouponIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<bool> MarkCouponAsUsed(int assignmentId)
        {
            var assignment = await _context.CouponAssignments.FindAsync(assignmentId);
            if (assignment == null) return false;

            assignment.IsUsed = true;
            assignment.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}