using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class DriverDocumentsDto
    {
        // Personal Documents
        [Required]
        public IFormFile NationalIdFront { get; set; }

        [Required]
        public IFormFile NationalIdBack { get; set; }

        [Required]
        public IFormFile CriminalRecord { get; set; }

        [Required]
        [StringLength(20)]
        public string IdNumber { get; set; }

        // License Documents
        [Required]
        public IFormFile DriverLicenseFront { get; set; }

        [Required]
        public IFormFile DriverLicenseBack { get; set; }

        [Required]
        public IFormFile SelfieWithLicense { get; set; }

        [Required]
        [StringLength(20)]
        public string LicenseNumber { get; set; }

        [Required]
        public DateTime LicenseExpiryDate { get; set; }
    }

}
