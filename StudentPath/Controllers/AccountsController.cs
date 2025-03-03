using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtos.Accounts;
using StudentPath.BLL.Services.AccountService;
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

        public AccountsController(IConfiguration configuration, IAccountService accountService, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _accountService = accountService;
            _memoryCache = memoryCache;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var response = await _accountService.Register(registerDto,Url);
            if (response.successed)
                return Ok(new { successed = true, message = "Registration successful." });

            return BadRequest(new { successed = false, errors = response.Errors });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await _accountService.Login(loginDto);
            if (response.successed)
                return Ok(new { successed = true, message = "Login successful.", token = response.Token });

            return Unauthorized(new { successed = false, errors = response.Errors });
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
