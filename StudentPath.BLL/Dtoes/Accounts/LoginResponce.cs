using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtos.Accounts{
   public class LoginResponce
    {
        public string Token { get; set; }
        public bool successed { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
        public string PropName { get; set; } = string.Empty;
        public string LoggedBy { get; set; } // <-- Add this
    }
}
