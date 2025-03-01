using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Accounts
{
    public class ResetPasswordOtpDto
    {
        public string Email { get; set; }
       
        public string NewPassword { get; set; }
        public string ConfirmedNewPassword { get; set; }
    }
}
