using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Activities
{
    public class JobUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ContractType { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? Location { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryPeriod { get; set; }
        public string? Description { get; set; }
        public string? Responsibilities { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Experience { get; set; }
        public string? Category { get; set; }
        public string? JobType { get; set; }
    }
}
