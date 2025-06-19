using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPath.BLL.Dtoes.Bookings;
using StudentPath.BLL.Dtoes.Trips;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.BLL.Services.UserServices;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Security.Claims;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase


    {

        #region Prop
        private readonly IUserService UserService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly StudentPathContext context;
        #endregion

        #region Ctor
        public UserController(IUserService UserService, IWebHostEnvironment webHostEnvironment,StudentPathContext context)
        {
            this.UserService = UserService;
            this.webHostEnvironment = webHostEnvironment;
            this.context = context;
        }
        #endregion

        #region GetAllUsers
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAll()
        {
            var result = await UserService.getUsersAsync();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new
            {
                Status = 200, // or any relevant status code you want
                Success = true, // set to false if needed
                Message = result.Message,
                Data = result.Data
            });
        }
        #endregion


        #region GetUserById
        [HttpGet("ById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Message = "Id cannot be null or empty" });
            }
            var result = await UserService.getUserAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new
            {
                Status = 200, // or any relevant status code you want
                Success = true, // set to false if needed
                Message = result.Message,
                Data = result.Data
            });

        }
        #endregion



        #region CreateUser

        [HttpPost("AddUser")]

        public async Task<IActionResult> Add([FromForm] UserAddDTO User)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage)
                              .ToList()
                });
            }

            string imageUrl = null;
            if (User.ProfileImage != null && User.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(User.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await User.ProfileImage.CopyToAsync(stream);
                }

                imageUrl = $"/Uploads/{uniqueFileName}";
            }
            else
            {

                imageUrl = User.Gender == GenderType.Male
                    ? "/Uploads/default/Male_Photo.jpg"
                    : "/Uploads/default/Female_Photo.jpg";
            }


            User.ImgUrl = imageUrl;

            var result = await UserService.CreateUserAsync(User);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = User.Id }, new
                {
                   
                    Success = true,
                    Message = result.Message,
                    Data = User
                });
            }

            return StatusCode(result.StatusCode, new { Message = result.Message });
        }
        #endregion


        #region UpdateUser
        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] UserUpdatedDTO User)
        {

            if (id == null || id != User.Id)
            {
                return BadRequest(new { Message = "Invalid User ID" });
            }

            var existingUser = await UserService.getUserAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            if (User.ProfileImage != null && User.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(User.ProfileImage.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await User.ProfileImage.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingUser.Data.ImgUrl))
                {
                    string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, existingUser.Data.ImgUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                User.ImgUrl = $"/Uploads/{uniqueFileName}";
            }
            else
            {
                User.ImgUrl = existingUser.Data.ImgUrl;
            }


            var result = await UserService.UpdateUserAsync(User);
            if (result.Success)
            {
                Response.Headers.Add("X-Message", "Updated user successfully");

                return Ok(new
                {
                    Success = true,
                    Message = "Updated user successfully"
                });
            }
            else
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });

            }


        }
        #endregion


        #region DeleteUser

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Message = "Id cannot be null or empty" });
            }

            var result = await UserService.SoftDeleteUserAsync(id);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });

        }

        #endregion


        #region GetUserTransactions

        [HttpGet("UserTransactions")]
        public async Task<IActionResult> GetTransactionsUser()
        {
            // Get UserId from JWT token claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Success = false, Message = "User not authenticated." });

            var transactions = await context.Payments
                                             .Where(t => t.UserId == userId && t.PaymentStatus==PaymentStatus.Paid)
                                             .OrderByDescending(t => t.PaymentDate)
                                             .Select(t => new UserTransactionDTO
                                             {
                                                 PaymentMethod = t.PaymentMethod,
                                                 PaymentDate = t.PaymentDate,
                                                 Amount = t.Amount
                                             })
                                             .ToListAsync();

            if (transactions == null || transactions.Count == 0)
                return NotFound(new { Success = false, Message = "No transactions found for this user." });

            return Ok(new
            {
                Success = true,
                Status = 200,
                Message = "Transactions retrieved successfully.",
                Data = transactions
            });
        }

        #endregion


        #region GetUserBookings


        [HttpGet("UserBookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            // Get UserId from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "User not authenticated."
                });
            }

            // Fetch bookings with related trip and locations
            var bookings = await context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t.FromLocation)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.ToLocation)
                .Where(b => b.UserId == userId&&b.BookingStatus==BookingStatus.Confirmed&&b.PaymentStatus==PaymentStatus.Paid).
                OrderByDescending(b => b.BookingDate)
                .Select(b => new UserBookingDTO
                {
                    FromLocation = new TripLocationDto
                    {
                        Latitude = b.Trip.FromLocation.Latitude,
                        Longitude = b.Trip.FromLocation.Longitude,
                        DisplayName = b.Trip.FromLocation.DisplayName,
                        FullAddress = b.Trip.FromLocation.FullAddress
                    },
                    ToLocation = new TripLocationDto
                    {
                        Latitude = b.Trip.ToLocation.Latitude,
                        Longitude = b.Trip.ToLocation.Longitude,
                        DisplayName = b.Trip.ToLocation.DisplayName,
                        FullAddress = b.Trip.ToLocation.FullAddress
                    },
                    TripStatus = b.Trip.Status,
                    BookingDate = b.BookingDate,
                    TotalSeats = b.NumberOfSeats
                })
                .ToListAsync();

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "No bookings found for this user."
                });
            }

            return Ok(new
            {
                Success = true,
                Status=200,
                Message = "Bookings retrieved successfully.",
                Data = bookings
            });
        }
    }

    #endregion




}

