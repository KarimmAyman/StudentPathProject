using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Dtos.Accounts;
using StudentPath.BLL.Services.AccountService;
using StudentPath.DAL.Data.DBHelpers;
using StudentPath.DAL.Data.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        private readonly IMemoryCache _memoryCache;
        private readonly StudentPathContext context;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IConfiguration configuration, IAccountService accountService, IMemoryCache memoryCache, StudentPathContext context,ILogger<AccountsController> logger,HttpClient httpClient)
        {
            _configuration = configuration;
            _accountService = accountService;
            _memoryCache = memoryCache;
            this.context = context;
            this._logger = logger;
           
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(
           [FromForm] RegisterDto registerDto,
           [FromForm] string vehicleInfoJson,
           [FromForm] string locationsJson)
        {
            // Deserialize vehicle and location JSON
            if (!string.IsNullOrEmpty(vehicleInfoJson))
            {
                registerDto.Vehicleinfo = JsonSerializer.Deserialize<List<VehicleInfoDto>>(vehicleInfoJson);
                // اربط الصور المرفوعة مع العناصر بناءً على الـ index
                for (int i = 0; i < registerDto.Vehicleinfo.Count; i++)
                {
                    var vehicle = registerDto.Vehicleinfo[i];

                    vehicle.VehiclePicture = Request.Form.Files[$"VehiclePicture_{i}"];
                    vehicle.VehicleRegistrationFront = Request.Form.Files[$"VehicleRegistrationFront_{i}"];
                    vehicle.VehicleRegistrationBack = Request.Form.Files[$"VehicleRegistrationBack_{i}"];
                }
            }

            if (!string.IsNullOrEmpty(locationsJson))
            {
                registerDto.locations = JsonSerializer.Deserialize<List<LocationDto>>(locationsJson);
            }

            // Handle user image (for non-driver users)
            if (registerDto.ImgUrlFile != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserImages");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDto.ImgUrlFile.FileName);
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerDto.ImgUrlFile.CopyToAsync(stream);
                }

                // Store relative path in ImgUrl (used in frontend or email)
                registerDto.ImgUrl = $"/UserImages/{uniqueFileName}";


            }
        

            var response = await _accountService.Register(registerDto, Url);

            if (response.successed)
                return Ok(new { successed = true, message = "Registration successful." });

            return BadRequest(new { successed = false, errors = response.Errors });
        }


     

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await _accountService.Login(loginDto);
            if (response.successed)
            {
                return Ok(new
                {
                    successed = true,
                    message = "Login successful.",
                    token = response.Token,
                    loggedBy = response.LoggedBy
                });
            }

            return Unauthorized(new
            {
                successed = false,
                errors = response.Errors
            });
        }


        [HttpPost("send-otp-for-password-reset")]
        public async Task<IActionResult> SendOtpForPasswordReset([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _accountService.SendOtpForPasswordReset(forgotPasswordDto);
            if (response.successed)
                return Ok(new { successed = true, message = "OTP sent successfully. Please check your email." });

            return BadRequest(new { successed = false, errors = response.Errors });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
        {
            bool isOtpValid = await _accountService.VerifyOtpAsync(request.Email, request.Otp);
            if (!isOtpValid)
            {
                return BadRequest(new { successed = false, errors = new[] { "Invalid or expired OTP." } });
            }

            // حفظ حالة التحقق في الكاش لمدة 15 دقيقة
            _memoryCache.Set($"VerifiedOtp_{request.Email}", true, TimeSpan.FromMinutes(15));

            return Ok(new { successed = true, message = "OTP verified successfully." });
        }


        [HttpPost("reset-password-with-otp")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordOtpDto resetPasswordOtpDto)
        {
            // التحقق مما إذا كان المستخدم قد أتمّ التحقق من OTP
            if (!_memoryCache.TryGetValue($"VerifiedOtp_{resetPasswordOtpDto.Email}", out bool isVerified) || !isVerified)
                return BadRequest(new { successed = false, errors = new[] { "OTP verification expired. Please request a new OTP." } });

            var response = await _accountService.ResetPasswordWithOtp(resetPasswordOtpDto);
            if (response.successed)
            {
                // إزالة حالة التحقق من الكاش بعد إعادة تعيين كلمة المرور
                _memoryCache.Remove($"VerifiedOtp_{resetPasswordOtpDto.Email}");
                return Ok(new { successed = true, message = "Password reset successful. You can now log in with your new password." });
            }

            return BadRequest(new { successed = false, errors = response.Errors });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return Ok(new { successed = true, message = "You have been logged out successfully." });
        }
        [HttpGet("ConfirmEmail")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var response = await _accountService.ConfirmEmail(userId, token);

            if (!response.successed)
            {
                return BadRequest(new { success = false, errors = response.Errors });
            }

            return Ok(new { success = true, message = "Your email has been successfully confirmed." });

        }
    }
}
