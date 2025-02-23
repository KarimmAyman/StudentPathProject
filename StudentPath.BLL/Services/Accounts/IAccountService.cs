
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.BLL.Dtos.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.AccountService
{
    public interface IAccountService
    {
        Task<GeneralRespnose> Register(RegisterDto registerDto, IUrlHelper urlHelper);
        Task<LoginResponce> Login(LoginDto loginDto);
        Task<GeneralRespnose> ConfirmEmail(string userId, string token);
        Task<GeneralRespnose> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<GeneralRespnose> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<GeneralRespnose> SendOtpForPasswordReset(ForgotPasswordDto forgotPasswordDto);  // Sends OTP to user's email
        //Task<GeneralRespnose> VerifyOtpForPasswordReset(string email, string otpCode); // Verifies OTP before resetting password
        Task<GeneralRespnose> ResetPasswordWithOtp(ResetPasswordOtpDto resetPasswordOtpDto); // Resets password after OTP verification
        Task<GeneralRespnose> ResendEmailVerification(string email, IUrlHelper urlHelper);
        Task Logout();
    }
   

    }

