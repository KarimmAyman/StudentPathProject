using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtos.Accounts;
using StudentPath.BLL.Services.AccountService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;

        public AccountsController(IConfiguration configuration, IAccountService accountService) {
            _configuration = configuration;
            _accountService = accountService;
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {



            // This provides the UrlHelper instance
            var urlHelper = Url;

            // Pass the urlHelper to the Register method
            var response = await _accountService.Register(registerDto, urlHelper);

            if (response.successed)
            {

                // return CreatedAtAction(nameof(Register), response);
                return Ok(new { message = "Register successful" });

            }


            return BadRequest(response.Errors);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {



            var response = await _accountService.Login(loginDto);

            if (response.successed)
            {
                return Ok(new { message = "Login successful" });
            }

            // Return unauthorized if login failed
            return Unauthorized(response.Errors);
        }
        [HttpGet("ConfirmEmail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {

            var responce = await _accountService.ConfirmEmail(userId, token);
            if (responce.successed)

            {
                return Ok(new { message = "Email confirmed successfully" });
            }
            return BadRequest(responce.Errors);
        }
        #region forgetpasswordAndSendLink
        [HttpPost("ForgotPassword")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {


            var UrlHepler = Url;
            var result = await _accountService.ForgotPassword(forgotPasswordDto);
            if (result.successed)
            {
                return Ok(new { message = "Password reset email sent successfully. Please check your inbox." });
            }
            return BadRequest(result.Errors);
        }
        #endregion
        #region ResetPasswordByClickLink

        [HttpPost("ResetPassword")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {


            var UrlHepler = Url;
            var result = await _accountService.ResetPassword(resetPasswordDto);
            if (result.successed)
            {
                return Ok(new { message = "Your password has been reset successfully." });
            }
            return BadRequest(result.Errors);
        }
        #endregion
        #region showResetToken

        [HttpGet("ShowResetToken")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ShowResetToken(string token, string email)
        {

            return Ok(new
            {

                Token = token,


                Email = email
            });
        }
        #endregion
        [HttpPost("send-otp-for-password-reset")]
        public async Task<IActionResult> SendOtpForPasswordReset([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _accountService.SendOtpForPasswordReset(forgotPasswordDto);
            if (response.successed)
            {
                return Ok(new { message = "OTP sent successfully. Please check your email." });
            }
            return BadRequest(response);
        }

      
        /// <summary>
        /// Resets the user's password using the verified OTP.
        /// </summary>
        [HttpPost("reset-password-with-otp")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordOtpDto resetPasswordOtpDto)
        {
            var response = await _accountService.ResetPasswordWithOtp(resetPasswordOtpDto);
            if (response.successed)
            {
                return Ok(new { message = "Password reset successful. You can now log in with your new password." });
            }
            return BadRequest(response);
        }

        [HttpPost("resend-email-verification")]
        public async Task<IActionResult> ResendEmailVerification([FromBody] ForgotPasswordDto resendEmailDto)
        {
            // Use the Url property to generate the confirmation link
            var response = await _accountService.ResendEmailVerification(resendEmailDto.Email, Url);

            if (response.successed)
            {
                return Ok(new { message = "Verification email has been resent successfully." });
            }

            return BadRequest(new { errors = response.Errors });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Call the Logout method in your service
            await _accountService.Logout();

            return Ok(new { message = "You have been logged out successfully." });
        }




    }
}
