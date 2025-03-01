using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtos.Accounts
{
    public class GeneralRespnose
    {
        public bool successed { get; set; } = false;
        public List<string> Errors { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public GeneralRespnose()
        {
            Errors = new List<string>(); // Initialize the list to avoid null reference exceptions
            successed = false;
        }
    }
}
