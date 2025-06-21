using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class PendingDriverDTO
    {
        public string? UserName { get; set; }
        [JsonIgnore]
        public ApprovalStatus Status { get; set; }
        public string StatusDescription
        {
            get
            {
                return Status switch
                {
                    ApprovalStatus.Pending => "Pending",
                    ApprovalStatus.Approved => "Approved",
                    ApprovalStatus.Denied => "Denied",
                    ApprovalStatus.NextStage => "Next Stage",
                    _ => "Unknown"
                };
            }
        }
    }
}
