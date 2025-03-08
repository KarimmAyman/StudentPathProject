using StudentPath.BLL.Dtoes.Accounts;
using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.HousingDtoes
{
    public class PropertyCreateDto
    {
        [Required]
        public AdvertisingStatusType AdvertisingStatus { get; set; }

        [Required]
        public bool? HasInsurance { get; set; }

        [Required]
        public HousingType HousingType { get; set; }

        [Required]
        public int Rooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public decimal GrossArea { get; set; }

        [Required]
        public decimal NetArea { get; set; }

        public WarmingType? WarmingType { get; set; }

        public int? BuildingAge { get; set; }

        public int? FloorLocation { get; set; }

        public bool? IsFurnished { get; set; }

        public bool? IsAvailableForLoan { get; set; }

        public decimal? Dues { get; set; }

        public PropertyFrontType? Front { get; set; }

        public decimal? RentPrice { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public CurrencyType Currency { get; set; }

        [Required]
        public string UserId { get; set; } // Owner of the property

        // Location as a separate entity (linked properly)
        [Required]
        public LocationDto Location { get; set; }

        // Images
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Features
        public List<int> FeatureIds { get; set; } = new List<int>();
    }
}
