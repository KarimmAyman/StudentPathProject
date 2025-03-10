using StudentPath.DAL.Data.Models.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.HousingDtoes
{
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        public string AdvertisingStatus { get; set; }  // تحويل الـ enum إلى string
        public bool? HasInsurance { get; set; }
        public string HousingType { get; set; }         // تحويل الـ enum إلى string
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal GrossArea { get; set; }
        public decimal NetArea { get; set; }
        public string WarmingType { get; set; }         // تحويل الـ enum إلى string (nullable)
        public int? BuildingAge { get; set; }
        public int? FloorLocation { get; set; }
        public bool? IsFurnished { get; set; }
        public bool? IsAvailableForLoan { get; set; }
        public decimal? Dues { get; set; }
        public string Front { get; set; }               // تحويل الـ enum إلى string (nullable)
        public decimal? RentPrice { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }            // تحويل الـ enum إلى string
        public string UserId { get; set; }

        // Related data
        public List<PropertyLocationDto> Locations { get; set; } = new List<PropertyLocationDto>();
        public List<PropertyImageDto> Images { get; set; } = new List<PropertyImageDto>();
        public List<FeatureDto> Features { get; set; } = new List<FeatureDto>();
    }

}
