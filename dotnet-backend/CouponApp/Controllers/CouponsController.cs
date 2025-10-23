using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CouponApp.Services;
using CouponApp.Models;
using CouponApp.DTOs;

namespace CouponApp.Controllers
{
    [ApiController]
    [Route("coupons")]
    public class CouponsController : ControllerBase
    {
        private readonly CouponService _couponService;
        private readonly UserService _userService;

        public CouponsController(CouponService couponService, UserService userService)
        {
            _couponService = couponService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CouponDto>> CreateCoupon(CouponCreateDto couponDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(couponDto.Code) || string.IsNullOrEmpty(couponDto.DiscountType))
            {
                return BadRequest("Code and discount type are required");
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Check if coupon code already exists
            // This would normally be done in the service layer
            // For simplicity, we're doing it here

            var coupon = await _couponService.CreateCoupon(couponDto, userId);

            var couponDtoResult = new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountAmount = coupon.DiscountAmount,
                DiscountType = coupon.DiscountType,
                ExpirationDate = coupon.ExpirationDate,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt,
                CreatedById = coupon.CreatedById
            };

            return CreatedAtAction(nameof(GetCoupon), new { id = coupon.Id }, couponDtoResult);
        }

        [HttpGet("my-coupons")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetMyCoupons()
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var assignments = await _couponService.GetUserCoupons(userId);
                var coupons = assignments.Select(a => new CouponDto
                {
                    Id = a.Coupon.Id,
                    Code = a.Coupon.Code,
                    Description = a.Coupon.Description,
                    DiscountAmount = a.Coupon.DiscountAmount,
                    DiscountType = a.Coupon.DiscountType,
                    ExpirationDate = a.Coupon.ExpirationDate,
                    IsActive = a.Coupon.IsActive,
                    CreatedAt = a.Coupon.CreatedAt,
                    CreatedById = a.Coupon.CreatedById
                });

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching coupons: {ex.Message}");
            }
        }

        [HttpGet("my-unused-coupons")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetMyUnusedCoupons()
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var assignments = await _couponService.GetUnusedUserCoupons(userId);
                var coupons = assignments.Select(a => new CouponDto
                {
                    Id = a.Coupon.Id,
                    Code = a.Coupon.Code,
                    Description = a.Coupon.Description,
                    DiscountAmount = a.Coupon.DiscountAmount,
                    DiscountType = a.Coupon.DiscountType,
                    ExpirationDate = a.Coupon.ExpirationDate,
                    IsActive = a.Coupon.IsActive,
                    CreatedAt = a.Coupon.CreatedAt,
                    CreatedById = a.Coupon.CreatedById
                });

                return Ok(coupons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching unused coupons: {ex.Message}");
            }
        }

        [HttpGet("my-created")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetMyCreatedCoupons()
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var coupons = await _couponService.GetManagerCoupons(userId);
                var couponDtos = coupons.Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountType = c.DiscountType,
                    ExpirationDate = c.ExpirationDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedById = c.CreatedById
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching created coupons: {ex.Message}");
            }
        }

        [HttpGet("unassigned")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetUnassignedCoupons()
        {
            try
            {
                var coupons = await _couponService.GetUnassignedCoupons();
                var couponDtos = coupons.Select(c => new CouponDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount,
                    DiscountType = c.DiscountType,
                    ExpirationDate = c.ExpirationDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    CreatedById = c.CreatedById
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching unassigned coupons: {ex.Message}");
            }
        }

        [HttpPost("assign")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<object>> AssignCoupon(CouponAssignmentCreateDto assignmentDto)
        {
            // Validate input
            if (assignmentDto.CouponId <= 0 || assignmentDto.UserId <= 0)
            {
                return BadRequest("Coupon ID and User ID are required");
            }

            // In a full implementation, you would check if the coupon and user exist
            // For simplicity, we're just creating the assignment

            var assignment = await _couponService.AssignCouponToUser(assignmentDto);
            return Ok(new { message = "Coupon assigned successfully" });
        }

        [HttpPost("use/{assignmentId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<object>> UseCoupon(int assignmentId)
        {
            // Validate input
            if (assignmentId <= 0)
            {
                return BadRequest("Assignment ID is required");
            }

            var result = await _couponService.MarkCouponAsUsed(assignmentId);
            if (!result)
            {
                return NotFound("Coupon assignment not found");
            }

            return Ok(new { message = "Coupon marked as used" });
        }

        // Additional endpoints that might be needed
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCoupons();
            var couponDtos = coupons.Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                Description = c.Description,
                DiscountAmount = c.DiscountAmount,
                DiscountType = c.DiscountType,
                ExpirationDate = c.ExpirationDate,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                CreatedById = c.CreatedById
            });

            return Ok(couponDtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CouponDto>> GetCoupon(int id)
        {
            var coupon = await _couponService.GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }

            var couponDto = new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountAmount = coupon.DiscountAmount,
                DiscountType = coupon.DiscountType,
                ExpirationDate = coupon.ExpirationDate,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt,
                CreatedById = coupon.CreatedById
            };

            return Ok(couponDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CouponDto>> UpdateCoupon(int id, CouponUpdateDto couponDto)
        {
            var coupon = await _couponService.UpdateCoupon(id, couponDto);
            if (coupon == null)
            {
                return NotFound();
            }

            var couponDtoResult = new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountAmount = coupon.DiscountAmount,
                DiscountType = coupon.DiscountType,
                ExpirationDate = coupon.ExpirationDate,
                IsActive = coupon.IsActive,
                CreatedAt = coupon.CreatedAt,
                CreatedById = coupon.CreatedById
            };

            return Ok(couponDtoResult);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteCoupon(int id)
        {
            var success = await _couponService.DeleteCoupon(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { message = "Coupon deleted successfully" });
        }
    }
}