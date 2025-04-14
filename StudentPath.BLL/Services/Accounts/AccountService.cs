using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtos.Accounts;
using StudentPath.DAL.Data.DBHelpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using StudentPath.DAL.Data.Models;
using Org.BouncyCastle.Crypto;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Microsoft.AspNetCore.Http;

namespace StudentPath.BLL.Services.AccountService
{
  
    public class AccountService : IAccountService
    {

       
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<CustomRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<User> _signInManager;
        private readonly StudentPathContext _studentPathContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor httpContextAccessor;
       
        public AccountService(UserManager<User> userManager, IConfiguration configuration, RoleManager<CustomRole> roleManager,
            IEmailService emailService, SignInManager<User> signInManager, StudentPathContext studentPathContext,IMemoryCache memoryCache,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _studentPathContext = studentPathContext;
            _memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<GeneralRespnose> Register(RegisterDto registerDto, IUrlHelper urlHelper)
        {
            string baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
            string logoUrl = $"{baseUrl}/Uploads/Aoun-logo.svg";

            var response = new GeneralRespnose();

            // Validate password match
            if (registerDto.Password != registerDto.ConfirmedPassword)
            {
                response.Errors.Add("Passwords do not match.");
                response.PropertyName = nameof(registerDto.ConfirmedPassword);
                return response;
            }
            #region unique userName
            // Check if username or email exists
            if (_userManager.Users.Any(s => s.UserName == registerDto.FullName))
            {
                response.Errors.Add("Username is already taken. Please choose another.");
                response.PropertyName = nameof(registerDto.FullName);
                return response;
            }
            #endregion
            if (_userManager.Users.Any(s => s.Email == registerDto.Email))
            {
                response.Errors.Add("Email already exists.");
                response.PropertyName = nameof(registerDto.Email);
                return response;
            }

            User user;

            if (registerDto.UserType == UserTypeEnum.User)
            {
                user = new User();
            }
            else if (registerDto.UserType == UserTypeEnum.Student)
            {
                user = new StudentPath.DAL.Data.Models.Student();
                ;
            }
            else if (registerDto.UserType == UserTypeEnum.Driver)
            {
                if (registerDto.Vehicleinfo == null || !registerDto.Vehicleinfo.Any())
                {
                    response.Errors.Add("Vehicle information is required for drivers.");
                    response.PropertyName = nameof(registerDto.Vehicleinfo);
                    return response;
                }

                user = new Driver
                {
                    DrivingLicense = registerDto.DrivingLicense,
                    VehicleInfo = registerDto.Vehicleinfo.Select(v => new VehicleInfo
                    {
                        VehicleBrand = v.VehicleBrand,
                        VehicleModel = v.VehicleModel,
                        VehicleColor = v.VehicleColor,
                        PlateNumber = v.PlateNumber,
                        VehiclePicturePath = v.VehiclePicture,
                        VehicleRegistrationFrontPath = v.VehicleRegistrationFront,
                        VehicleRegistrationBackPath = v.VehicleRegistrationBack,
                        SeatingCapacity = v.SeatingCapacity 
                    }).ToList()
                };
            }
            else if (registerDto.UserType == UserTypeEnum.Admin)  // Explicitly check for Admin
            {
                user = new Admin();
            }
            else
            {
                throw new ArgumentException("Invalid user type selected.");
            }

            // Assign general properties
            user.UserName = registerDto.FullName;
            user.Email = registerDto.Email;
            user.UserType = registerDto.UserType;
            user.Gender = registerDto.Gender;
            user.Age = registerDto.Age;
            user.ImgUrl = registerDto.ImgUrl;
            user.PhoneNumber = registerDto.PhoneNumber;

            // Save the user first
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                //// Check if locations exist and assign them
                //if (registerDto.locations != null && registerDto.locations.Any())
                //{
                    user.Locations = registerDto.locations.Select(loc => new Location
                    {
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude,
                        City = loc.City,
                        Country = loc.Country,
                        UserId = user.Id  // Ensure UserId is set
                    }).ToList();

                    // Save the changes (update user to include locations)
                    await _userManager.UpdateAsync(user);
                //}

                    // If Driver, ensure VehicleInfo is saved
                    if (user is Driver driver)
                {
                    if (driver.VehicleInfo != null && driver.VehicleInfo.Any())
                    {
                        foreach (var vehicle in driver.VehicleInfo)
                        {
                            vehicle.DriverId = driver.Id; // Ensure FK is set
                        }
                    }
                }

                await _userManager.UpdateAsync(user); // Save changes explicitly

                await _signInManager.SignInAsync(user, isPersistent: false);

                #region VerifyEmail
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = urlHelper.Action("ConfirmEmail", "Accounts",
                    new { userId = user.Id, token = emailConfirmationToken }, "https");

                var confirmationEmailBody= $@"
    <!DOCTYPE html>
    <html>
    <head>
        <style>
           .container {{
              max-width: 600px;
              margin: 0 auto;
                background-color: #f6f9fc;
               padding: 20px;
                font-family: Arial, sans-serif;
               border-radius: 8px;
                text-align: left;
        }}
           .card {{
                background-color: white;
              padding: 30px;
               border-radius: 8px;
               box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.1);
          }}
           .button {{
             background-color: #83cd20;
              color: white;
               padding: 12px 24px;
              border-radius: 6px;
              text-decoration: none;
              font-weight: bold;
              display: inline-block;
              margin-top: 20px;
               text-align: center;
          }}
           .footer {{
               font-size: 12px;
              color: #666;
             margin-top: 20px;
         }}
           .logo {{
               display: inline-block;
               width: 150px; /* Adjust the size as needed */
                vertical-align: middle;
               margin-right: 10px;
           }}
      </style>
     </head>
       <body>
        <div class='container'>
         <div class='card'>
             <div>
                    <img src='{logoUrl}' class='logo' alt='Student Path Logo' />
               </div>
              <p>Thanks for creating a Student Path account. Verify your email so you can get up and running quickly.</p>
              <a href='{confirmationLink}' class='button'>Verify Email</a>
               <p>Once your email is verified, you can start setting up your account. If you have questions, visit our <a href='https://support.example.com'>support site</a>.</p>
           </div>
            <div class='footer'>
               <p>Student Path, Kafr El-Sheikh, Egypt</p>
          </div>
       </div>
   </body>
   </html>";

                var res= await _emailService.SendEmailAsync(user.Email, "Verify Your Account",confirmationEmailBody);


                //var res = await _emailService.SendEmailAsync(user.Email, "Confirm Your Email Address", confirmationEmailBody);
                if (!res.successed)
                {
                    response.Errors.AddRange(res.Errors);
                    return response;
                }
                #endregion

                response.successed = true;
                return response;
            }

            response.Errors = result.Errors.Select(d => d.Description).ToList();
            return response;
        }


        public async Task<LoginResponce> Login(LoginDto loginDto)
        {
            var response = new LoginResponce();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                response.Errors.Add(user == null ? "Email not found. Please make sure the email is correct." :
                    "Email not confirmed. Please check your inbox.");
                response.PropName = nameof(loginDto.Email);
                return response;
            }

            if (user.IsBanned)
            {
                response.Errors.Add("Your account has been banned.");
                return response;
            }
            if (user.UserType == UserTypeEnum.Driver)
            {
                var driver = user as StudentPath.DAL.Data.Models.Driver;

                if (driver != null)
                {
                    if (driver.Status == ApprovalStatus.Pending)
                    {
                        response.Errors.Add("Your account is pending approval by the admin.");
                        return response;
                    }
                    else if (driver.Status == ApprovalStatus.Denied)
                    {
                        response.Errors.Add("Your account has been denied by the admin.");
                        return response;
                    }
                }
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                #region Claims
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // User ID as Subject
                    new Claim(JwtRegisteredClaimNames.Email, user.Email), // User email
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Token identifier
                };
                var UserRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                #endregion
                await _userManager.AddClaimsAsync(user, claims);
                response.Token = GenerateToken(claims, loginDto.RememberMe);
                response.successed = true;
                return response;
            }

            response.Errors.Add("Wrong Password or Email");
            response.PropName = nameof(loginDto.Password);
            return response;
        }

        private string GenerateToken(IList<Claim> claims, bool RememberMe)
        {
            #region Token
            var SecretKeyString = _configuration.GetSection("JWT:SecretKey").Value;
            var SecretKeyByte = Encoding.ASCII.GetBytes(SecretKeyString);
            SecurityKey securityKey = new SymmetricSecurityKey(SecretKeyByte);

            SigningCredentials signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            DateTime tokenExpiration = RememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(2);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
            (
                claims: claims,
                signingCredentials: signingCredential,
                expires: tokenExpiration,
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value
            ) ;

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(jwtSecurityToken);
            #endregion
        }
        public async Task<GeneralRespnose> ConfirmEmail(string userId, string token)
        {
            GeneralRespnose response = new GeneralRespnose();
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                response.Errors.Add("UserId and Token are required.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Errors.Add("User not found.");
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                response.successed = true;
                return response;

            }
            response.Errors.Add("Email confirmation failed."); ;
            return response;

        }
        [HttpPost("ForgotPassword")]
        public async Task<GeneralRespnose> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new GeneralRespnose();

            // 🔍 Find user by email
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                response.Errors.Add("Email not found. Please make sure the email is correct.");
                response.PropertyName = nameof(forgotPasswordDto.Email);
                return response;
            }

            // 🔑 Generate and encode reset password token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // 📨 Send reset password email (Optional)
            var resetEmailBody = $@"
        Dear {user.UserName},
        Here is your password reset token:
        {encodedToken}
        Use this token in the reset password API request
        Best regards,
        StudentPathPlatform";

            var emailResult = await _emailService.SendEmailAsync(user.Email, "Your Password Reset Token", resetEmailBody);

            if (emailResult.successed)
            {
                response.successed = true;
              /*  response.Data = new { Token = encodedToken };*/ // 👈 Returns token in the API response
                return response;
            }

            response.Errors.AddRange(emailResult.Errors);
            return response;
        }



        public async Task<GeneralRespnose> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new GeneralRespnose();

            // Validate passwords match
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmedNewPassword)
            {
                response.Errors.Add("New password and confirmation password do not match.");
                return response;
            }

            // Check if the user exists
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                response.Errors.Add("Email not found. Please make sure the email is correct.");
                return response;
            }

            // Decode the token correctly
            var decodedToken = WebUtility.UrlDecode(resetPasswordDto.Token);

            // Reset the password
            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (resetResult.Succeeded)
            {
                response.successed = true;
                return response;
            }

            // Collect errors if reset fails
            response.Errors = resetResult.Errors.Select(e => e.Description).ToList();
            return response;
        }

        public async Task<GeneralRespnose> SendOtpForPasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            string baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
            string logoUrl2 = $"{baseUrl}/Uploads/Aoun-logo.svg";
            var response = new GeneralRespnose();

            // Find user by email
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                response.Errors.Add("Email not found. Please make sure the email is correct.");
                return response;
            }

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Store OTP temporarily (database or cache)
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            // Send OTP via email
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        .otp-container {{
            text-align: center;
            font-family: Arial, sans-serif;
        }}
        .otp-box {{
            display: inline-block;
            font-size: 24px;
            font-weight: bold;
            background-color: #f3f4f6;
            padding: 10px 15px;
            margin: 5px;
            border-radius: 8px;
            border: 1px solid #ddd;
        }}
        .logo {{
            display: block;
            margin: 0 auto 20px auto;
            width: 150px; /* Adjust size as needed */
        }}
    </style>
</head>
<body>
    <div class='otp-container'>
        <!-- Aoun Logo -->
        <img src='{logoUrl2}' class='logo' alt='Aoun Logo' />

        <p>Your verification code is:</p>
        <div>
            {string.Join(" ", otp.Select(c => $"<span class='otp-box'>{c}</span>"))}
        </div>
        <p>Enter this code to reset your Password.</p>
    </div>
</body>
</html>";

            var emailResult = await _emailService.SendEmailAsync(user.Email, "Password Reset OTP", emailBody);

            if (emailResult.successed)
            {
                response.successed = true;
                response.PropertyName = otp;  // Store OTP only in Data field
                return response;
            }

            response.Errors.AddRange(emailResult.Errors);
            return response;
        }

        public async Task<GeneralRespnose> ResetPasswordWithOtp(ResetPasswordOtpDto resetPasswordOtpDto)
        {
            var response = new GeneralRespnose();

            // التأكد أن المستخدم قد تحقق من OTP خلال المدة المسموح بها
            if (!_memoryCache.TryGetValue($"VerifiedOtp_{resetPasswordOtpDto.Email}", out bool isVerified) || !isVerified)
            {
                response.Errors.Add("OTP verification expired. Please request a new OTP.");
                return response;
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordOtpDto.Email);
            if (user == null)
            {
                response.Errors.Add("Email not found.");
                return response;
            }

            if (resetPasswordOtpDto.NewPassword != resetPasswordOtpDto.ConfirmedNewPassword)
            {
                response.Errors.Add("Passwords do not match.");
                return response;
            }

            var resetResult = await _userManager.RemovePasswordAsync(user);
            if (resetResult.Succeeded)
            {
                var setPasswordResult = await _userManager.AddPasswordAsync(user, resetPasswordOtpDto.NewPassword);
                if (setPasswordResult.Succeeded)
                {
                    // حذف حالة OTP بعد إعادة تعيين كلمة المرور
                    _memoryCache.Remove($"VerifiedOtp_{resetPasswordOtpDto.Email}");

                    response.successed = true;
                    return response;
                }

                response.Errors = setPasswordResult.Errors.Select(e => e.Description).ToList();
                return response;
            }

            response.Errors = resetResult.Errors.Select(e => e.Description).ToList();
            return response;
        }

        public async Task<GeneralRespnose> ResendEmailVerification(string email, IUrlHelper urlHelper)
        {
            var response = new GeneralRespnose();

            // Find user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.Errors.Add("Email not found.");
                return response;
            }

            // Check if the email is already confirmed
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                response.Errors.Add("Email is already verified.");
                return response;
            }

            // Generate a new email confirmation token
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate the confirmation link using the URL helper (without full domain)
            var confirmationLink = urlHelper.Action("ConfirmEmail", "Accounts",
                new { userId = user.Id, token = emailConfirmationToken }, protocol: "https");

            // Construct the email body content
            var confirmationEmailBody = $"Dear {user.UserName},\n\n" +
                                        "Thank you for registering with us!\n\n" +
                                        "To complete your registration, please confirm your email address by clicking the link below:\n" +
                                        $"{confirmationLink}\n\n" +
                                        "Best regards,\n" +
                                        "[Student Path Platform]\n" +
                                        "[+20 155 134 9812]";

            // Send email with the confirmation link
            var emailResult = await _emailService.SendEmailAsync(user.Email, "Confirm Your Email Address", confirmationEmailBody);

            if (!emailResult.successed)
            {
                response.Errors.AddRange(emailResult.Errors);
                return response;
            }

            response.successed = true;
            return response;
        }


        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            if (user.OtpCode == otp && user.OtpExpiry > DateTime.UtcNow)
            {
                // حفظ التحقق في الكاش لمدة 15 دقيقة بعد التحقق الناجح
                _memoryCache.Set($"VerifiedOtp_{email}", true, TimeSpan.FromMinutes(15));
                return true;
            }

            return false;
        }

        public async Task<GeneralRespnose> Logout()
        {
            var response = new GeneralRespnose();
            try
            {
                await _signInManager.SignOutAsync();
                response.successed = true;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"An error occurred while logging out: {ex.Message}");
            }
            return response;
        }


    }
}
