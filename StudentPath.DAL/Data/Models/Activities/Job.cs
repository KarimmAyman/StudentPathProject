using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Activities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Title of the job (e.g., "Senior UX Designer").
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Short description of the job type/contract 
        /// (e.g. "Contract Base", "Full-Time", "Part-Time").
        /// </summary>
        [MaxLength(100)]
        public string? ContractType { get; set; }

        /// <summary>
        /// Company or organization offering the job.
        /// </summary>
        [MaxLength(150)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Website for the company or job listing (e.g., "https://instagram.com").
        /// </summary>
        [DataType(DataType.Url)]
        public string? CompanyWebsite { get; set; }

        /// <summary>
        /// Company or contact phone number (e.g., "(400) 555-012").
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        public string? CompanyPhone { get; set; }

        /// <summary>
        /// Company or contact email (e.g., "[email protected]").
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string? CompanyEmail { get; set; }

        /// <summary>
        /// Primary location for the job (e.g., "Australia", "New York, USA").
        /// </summary>
        [MaxLength(150)]
        public string? Location { get; set; }

        /// <summary>
        /// Minimum salary offered (if applicable).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinSalary { get; set; }

        /// <summary>
        /// Maximum salary offered (if applicable).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxSalary { get; set; }

        /// <summary>
        /// Indicates the salary unit or period (e.g. "Month", "Year").
        /// </summary>
        [MaxLength(50)]
        public string? SalaryPeriod { get; set; }

        /// <summary>
        /// Full job description (as seen in the "Job Description" section).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Responsibilities (as seen in the "Responsibilities" section).
        /// </summary>
        public string? Responsibilities { get; set; }

        /// <summary>
        /// Date the job was posted.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date the job listing will expire or close.
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Any required experience, degree, or qualification 
        /// (e.g. "Graduation", "5-10 years").
        /// </summary>
        [MaxLength(100)]
        public string? Experience { get; set; }

        /// <summary>
        /// For additional categorization of the job 
        /// (e.g., "Design", "IT", "Services", etc.).
        /// </summary>
        [MaxLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Type of job (e.g. "Full-Time", "Part-Time", "Remote").
        /// </summary>
        [MaxLength(100)]
        public string? JobType { get; set; }

        /// <summary>
        /// Indicates whether this job is "Featured" (e.g., for display in a special section).
        /// </summary>
        public bool IsFeatured { get; set; } = false;

        /// <summary>
        /// Indicates whether the job is currently active (still accepting applications).
        /// </summary>
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Reference to the user (e.g., Admin or Employer) who created the job.
        /// </summary>
        [ForeignKey(nameof(CreatedByUser))]
        public string? CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// Computed or convenience property for the number of days remaining.
        /// (Not mapped to the DB by default.)
        /// </summary>
        [NotMapped]
        public int? DaysRemaining
        {
            get
            {
                if (ExpiryDate.HasValue)
                {
                    var diff = (ExpiryDate.Value.Date - DateTime.UtcNow.Date).Days;
                    return diff >= 0 ? diff : 0;
                }
                return null;
            }
        }
    }
}
