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
using StudentPath.BLL.Dtoes.Drivers;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Services.DriverServices;

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
        private readonly IDriverService _driverService;

        public AccountService(UserManager<User> userManager, IConfiguration configuration, RoleManager<CustomRole> roleManager,
            IEmailService emailService, SignInManager<User> signInManager, StudentPathContext studentPathContext,IMemoryCache memoryCache,IHttpContextAccessor httpContextAccessor ,IDriverService driverService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _studentPathContext = studentPathContext;
            _memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
            _driverService = driverService;
        }

        public async Task<GeneralRespnose> Register(RegisterDto registerDto, IUrlHelper urlHelper)
        {
            string baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
            string logoUrl = $"{baseUrl}/Uploads/Aoun-logo.svg";
            var response = new GeneralRespnose();

            if (registerDto.Password != registerDto.ConfirmedPassword)
            {
                response.Errors.Add("Passwords do not match.");
                response.PropertyName = nameof(registerDto.ConfirmedPassword);
                return response;
            }

            // Validate uniqueness
            if (_userManager.Users.Any(s => s.UserName == registerDto.FullName))
            {
                response.Errors.Add("Username is already taken.");
                response.PropertyName = nameof(registerDto.FullName);
                return response;
            }
            if (_userManager.Users.Any(s => s.Email == registerDto.Email))
            {
                response.Errors.Add("Email already exists.");
                response.PropertyName = nameof(registerDto.Email);
                return response;
            }
            if (_userManager.Users.Any(s => s.PhoneNumber == registerDto.PhoneNumber))
            {
                response.Errors.Add("Phone number already exists.");
                response.PropertyName = nameof(registerDto.PhoneNumber);
                return response;
            }

            if (registerDto.UserType == UserTypeEnum.Driver)
            {
                // Step 1: Create the Identity user
                var identityDriver = new Driver
                {
                    UserName = registerDto.FullName,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserType = UserTypeEnum.Driver,
                    Gender = registerDto.Gender,
                    Age = registerDto.Age,
                    ImgUrl = registerDto.ImgUrl
                };

                var identityResult = await _userManager.CreateAsync(identityDriver, registerDto.Password);
                if (!identityResult.Succeeded)
                {
                    response.Errors = identityResult.Errors.Select(e => e.Description).ToList();
                    return response;
                }

                // Step 2: Populate the DriverAddDTO and use the existing CreateDriverAsync
                var driverDto = new DriverAddDTO
                {
                    Id = identityDriver.Id, // IMPORTANT: use existing Identity user ID
                    UserName = registerDto.FullName,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    DateOfBirth = DateTime.UtcNow.AddYears(-registerDto.Age),
                    Gender = registerDto.Gender,
                    IdNumber = registerDto.IdNumber,
                    LicenseNumber = registerDto.DrivingLicense,
                    LicenseExpiryDate = (DateTime)registerDto.LicenseExpiryDate,
                    IdFront = registerDto.IdFront,
                    IdBack = registerDto.IdBack,
                    CriminalRecord = registerDto.CriminalRecord,
                    LicenseFront = registerDto.LicenseFront,
                    LicenseBack = registerDto.LicenseBack,
                    LicenseSelfie = registerDto.LicenseSelfie,
                    Locations = registerDto.locations?.Select(loc => new LocationDto
                    {
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude,
                        City = loc.City,
                        Country = loc.Country
                    }).ToList(),
                    VehicleAddDTOs = registerDto.Vehicleinfo?.Select(v => new VehicleAddDTO
                    {
                        VehicleBrand = v.VehicleBrand,
                        VehicleModel = v.VehicleModel,
                        VehicleColor = v.VehicleColor,
                        PlateNumber = v.PlateNumber,
                        SeatingCapacity = v.SeatingCapacity,
                        ProductionYear = v.ProductionYear,
                        VehiclePicture = v.VehiclePicture,
                        VehicleRegistrationFront = v.VehicleRegistrationFront,
                        VehicleRegistrationBack = v.VehicleRegistrationBack
                    }).ToList()
                };


                // back in your Register method, right after CreateDriverAsync:
                await _driverService.CreateDriverAsync(driverDto);

                // now re‐normalize the user and save via UserManager:
                identityDriver.NormalizedUserName = _userManager.NormalizeName(identityDriver.UserName);
                identityDriver.NormalizedEmail = _userManager.NormalizeEmail(identityDriver.Email);
                identityDriver.PasswordHash = _userManager.PasswordHasher.HashPassword(identityDriver, registerDto.Password);
                var updateRes = await _userManager.UpdateAsync(identityDriver);

                await _signInManager.SignInAsync(identityDriver, isPersistent: false);

                // Email confirmation
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityDriver);
                var confirmationLink = urlHelper.Action("ConfirmEmail", "Accounts", new { userId = identityDriver.Id, token }, "https");

                var emailBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                .container {{
                    max-width: 600px;
                    margin: auto;
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
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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
                    width: 150px;
                    margin-bottom: 20px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='card'>
                    <img src='{logoUrl}' class='logo' alt='Student Path Logo' />
                    <p>Thanks for creating a Student Path account. Please verify your email:</p>
                    <a href='{confirmationLink}' class='button'>Verify Email</a>
                </div>
                <div class='footer'>
                    <p>Student Path, Kafr El-Sheikh, Egypt</p>
                </div>
            </div>
        </body>
        </html>";

                var emailRes = await _emailService.SendEmailAsync(identityDriver.Email, "Verify Your Account", emailBody);
                if (!emailRes.successed)
                {
                    response.Errors.AddRange(emailRes.Errors);
                    return response;
                }

                response.successed = true;
                return response;
            }

            // =========================
            // Non-driver registration
            // =========================
            User user;

            if (registerDto.UserType == UserTypeEnum.Student)
                user = new StudentPath.DAL.Data.Models.Student();
            else if (registerDto.UserType == UserTypeEnum.Admin)
                user = new Admin();
            else
                user = new User();

            user.UserName = registerDto.FullName;
            user.Email = registerDto.Email;
            user.UserType = registerDto.UserType;
            user.Gender = registerDto.Gender;
            user.Age = registerDto.Age;
            user.ImgUrl = registerDto.ImgUrl;
            user.PhoneNumber = registerDto.PhoneNumber;

            var identityUserResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!identityUserResult.Succeeded)
            {
                response.Errors = identityUserResult.Errors.Select(e => e.Description).ToList();
                return response;
            }

            if (registerDto.locations != null)
            {
                user.Locations = registerDto.locations.Select(loc => new Location
                {
                    Latitude = loc.Latitude,
                    Longitude = loc.Longitude,
                    City = loc.City,
                    Country = loc.Country,
                    UserId = user.Id
                }).ToList();

                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = urlHelper.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token = confirmationToken }, "https");

            var confirmEmailBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                .container {{
                    max-width: 600px;
                    margin: auto;
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
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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
                    width: 150px;
                    margin-bottom: 20px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='card'>
                    <img src='{logoUrl}' class='logo' alt='Student Path Logo' />
                    <p>Thanks for creating a Student Path account. Please verify your email:</p>
                    <a href='{confirmLink}' class='button'>Verify Email</a>
                </div>
                <div class='footer'>
                    <p>Student Path, Kafr El-Sheikh, Egypt</p>
                </div>
            </div>
        </body>
        </html>";
            var emailSendResult = await _emailService.SendEmailAsync(user.Email, "Verify Your Email", confirmEmailBody);

            if (!emailSendResult.successed)
            {
                response.Errors.AddRange(emailSendResult.Errors);
                return response;
            }

            response.successed = true;
            return response;
        }



        public async Task<LoginResponce> Login(LoginDto loginDto)
        {
            var response = new LoginResponce();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // 1) Does the email even exist?
            if (user == null)
            {
                response.Errors.Add("Email not found. Please make sure the email is correct.");
                response.PropName = nameof(loginDto.Email);
                return response;
            }

            // 2) Is the email confirmed?
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                response.Errors.Add("Email not confirmed. Please check your inbox.");
                response.PropName = nameof(loginDto.Email);
                return response;
            }

            // 3) Is the user banned?
            if (user.IsBanned)
            {
                response.Errors.Add("You cannot login because your account has been banned.");
                response.PropName = nameof(loginDto.Email);
                return response;
            }

            // 4) Is the user deleted?
            if (user.IsDeleted)
            {
                response.Errors.Add("Your account has been deleted and cannot be used to login.");
                response.PropName = nameof(loginDto.Email);
                return response;
            }

            // 5) If a driver, check approval status
            if (user.UserType == UserTypeEnum.Driver)
            {
                var driver = user as Driver;
                if (driver != null)
                {
                    if (driver.Status == ApprovalStatus.Pending)
                    {
                        response.Errors.Add("Your account is pending approval by the admin.");
                        return response;
                    }
                    if (driver.Status == ApprovalStatus.Denied)
                    {
                        response.Errors.Add("Your account has been denied by the admin.");
                        return response;
                    }
                }
                // Set loggedBy as "driver"
                response.LoggedBy = "driver";
            }
            // 6) If admin, set loggedBy to "admin"
            else if (user.UserType == UserTypeEnum.Admin)
            {
                response.LoggedBy = "admin";
            }
            // 7) For regular users, set loggedBy as "user"
            else
            {
                response.LoggedBy = "user";
            }

            // 8) Finally, check password
            var pwdOk = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!pwdOk)
            {
                response.Errors.Add("Wrong password or email.");
                response.PropName = nameof(loginDto.Password);
                return response;
            }

            // 9) Build claims & issue JWT
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Add user roles to the claims
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Add the claims to the user
            await _userManager.AddClaimsAsync(user, claims);

            // Generate the token
            response.Token = GenerateToken(claims, loginDto.RememberMe);
            response.successed = true;

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
