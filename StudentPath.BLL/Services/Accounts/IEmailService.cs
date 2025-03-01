
using StudentPath.BLL.Dtos.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.AccountService
{
    public interface  IEmailService
    {
        Task<GeneralRespnose> SendEmailAsync(string email, string subject, string message);
    }
}
