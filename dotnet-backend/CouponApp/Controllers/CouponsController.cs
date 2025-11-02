using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CouponApp.Services;
using CouponApp.Models;
using CouponApp.DTOs;
using ClosedXML.Excel;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons()
        {
            try
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
                    CreatedById = c.CreatedById,
                    Brand = c.Brand,
                    AssignmentTitle = c.AssignmentTitle
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching coupons: {ex.Message}");
            }
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
                CreatedById = coupon.CreatedById,
                Brand = coupon.Brand,
                AssignmentTitle = coupon.AssignmentTitle
            };

            return CreatedAtAction(nameof(GetCoupon), new { id = coupon.Id }, couponDtoResult);
        }

        [HttpPost("upload-excel")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> UploadCouponsFromExcel(IFormFile file)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            var coupons = new List<Coupon>();
            
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed();

                        foreach (var row in rows.Skip(1)) // Skip header row
                        {
                            var couponDto = new CouponCreateDto
                            {
                                Code = row.Cell(1).Value.ToString() ?? "",
                                Description = row.Cell(2).Value.ToString() ?? "",
                                DiscountAmount = int.TryParse(row.Cell(3).Value.ToString(), out var discount) ? discount : 0,
                                DiscountType = row.Cell(4).Value.ToString() ?? "fixed",
                                ExpirationDate = DateTime.TryParse(row.Cell(5).Value.ToString(), out var date) ? date : (DateTime?)null,
                                Brand = row.Cell(6).Value.ToString() ?? "",
                                AssignmentTitle = row.Cell(7).Value.ToString() ?? ""
                            };

                            // Validate required fields
                            if (!string.IsNullOrEmpty(couponDto.Code) && !string.IsNullOrEmpty(couponDto.DiscountType))
                            {
                                var coupon = await _couponService.CreateCoupon(couponDto, userId);
                                coupons.Add(coupon);
                            }
                        }
                    }
                }

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
                    CreatedById = c.CreatedById,
                    Brand = c.Brand,
                    AssignmentTitle = c.AssignmentTitle
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing Excel file: {ex.Message}");
            }
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
                    CreatedById = a.Coupon.CreatedById,
                    Brand = a.Coupon.Brand,
                    AssignmentTitle = a.Coupon.AssignmentTitle
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
                    CreatedById = a.Coupon.CreatedById,
                    Brand = a.Coupon.Brand,
                    AssignmentTitle = a.Coupon.AssignmentTitle
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
                    CreatedById = c.CreatedById,
                    Brand = c.Brand,
                    AssignmentTitle = c.AssignmentTitle
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
                    CreatedById = c.CreatedById,
                    Brand = c.Brand,
                    AssignmentTitle = c.AssignmentTitle
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching unassigned coupons: {ex.Message}");
            }
        }

        [HttpGet("unassigned-by-title/{assignmentTitle}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetUnassignedCouponsByAssignmentTitle(string assignmentTitle)
        {
            try
            {
                var coupons = await _couponService.GetUnassignedCouponsByAssignmentTitle(assignmentTitle);
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
                    CreatedById = c.CreatedById,
                    Brand = c.Brand,
                    AssignmentTitle = c.AssignmentTitle
                });

                return Ok(couponDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching unassigned coupons: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
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
                CreatedById = coupon.CreatedById,
                Brand = coupon.Brand,
                AssignmentTitle = coupon.AssignmentTitle
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
                CreatedById = coupon.CreatedById,
                Brand = coupon.Brand,
                AssignmentTitle = coupon.AssignmentTitle
            };

            return Ok(couponDtoResult);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CouponDto>> DeleteCoupon(int id)
        {
            var coupon = await _couponService.GetCouponById(id);
            if (coupon == null)
            {
                return NotFound();
            }

            var success = await _couponService.DeleteCoupon(id);
            if (!success)
            {
                return StatusCode(500, "Error deleting coupon");
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
                CreatedById = coupon.CreatedById,
                Brand = coupon.Brand,
                AssignmentTitle = coupon.AssignmentTitle
            };

            return Ok(couponDto);
        }

        [HttpPost("assign")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<CouponAssignmentDto>> AssignCoupon(CouponAssignmentCreateDto assignmentDto)
        {
            try
            {
                var assignment = await _couponService.AssignCouponToUser(assignmentDto);
                
                var assignmentDtoResult = new CouponAssignmentDto
                {
                    Id = assignment.Id,
                    CouponId = assignment.CouponId,
                    UserId = assignment.UserId,
                    IsUsed = assignment.IsUsed,
                    UsedAt = assignment.UsedAt,
                    AssignedAt = assignment.AssignedAt
                };

                return Ok(assignmentDtoResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error assigning coupon: {ex.Message}");
            }
        }

        // New endpoint for bulk assignment of coupons to users
        [HttpPost("assign-bulk")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<CouponAssignmentDto>>> AssignCouponsBulk([FromBody] IEnumerable<CouponAssignmentCreateDto> assignments)
        {
            try
            {
                var assignmentTuples = assignments.Select(a => (a.CouponId, a.UserId));
                var createdAssignments = await _couponService.AssignCouponsToUsersBulk(assignmentTuples);
                
                var assignmentDtos = createdAssignments.Select(a => new CouponAssignmentDto
                {
                    Id = a.Id,
                    CouponId = a.CouponId,
                    UserId = a.UserId,
                    IsUsed = a.IsUsed,
                    UsedAt = a.UsedAt,
                    AssignedAt = a.AssignedAt
                });

                return Ok(assignmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error bulk assigning coupons: {ex.Message}");
            }
        }

        // New endpoint for bulk assignment by assignment title
        [HttpPost("assign-by-title")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<IEnumerable<CouponAssignmentDto>>> AssignCouponsByAssignmentTitle([FromBody] BulkCouponAssignmentDto bulkAssignmentDto)
        {
            try
            {
                // Get all users
                var users = await _userService.GetUsers();
                var userIds = users.Select(u => u.Id);
                
                var createdAssignments = await _couponService.AssignCouponsByAssignmentTitle(bulkAssignmentDto.AssignmentTitle, userIds);
                
                var assignmentDtos = createdAssignments.Select(a => new CouponAssignmentDto
                {
                    Id = a.Id,
                    CouponId = a.CouponId,
                    UserId = a.UserId,
                    IsUsed = a.IsUsed,
                    UsedAt = a.UsedAt,
                    AssignedAt = a.AssignedAt
                });

                return Ok(assignmentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error bulk assigning coupons by title: {ex.Message}");
            }
        }

        [HttpPost("use/{assignmentId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> UseCoupon(int assignmentId)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Verify that the coupon assignment belongs to the user
            // We need to get the assignment through the service
            var allAssignments = await _couponService.GetUserCoupons(userId);
            var assignment = allAssignments.FirstOrDefault(ca => ca.Id == assignmentId);

            if (assignment == null)
            {
                return NotFound("Coupon assignment not found or does not belong to user");
            }

            if (assignment.IsUsed)
            {
                return BadRequest("Coupon has already been used");
            }

            var success = await _couponService.MarkCouponAsUsed(assignmentId);
            if (!success)
            {
                return StatusCode(500, "Error marking coupon as used");
            }

            return Ok(new { message = "Coupon marked as used successfully" });
        }
    }
}