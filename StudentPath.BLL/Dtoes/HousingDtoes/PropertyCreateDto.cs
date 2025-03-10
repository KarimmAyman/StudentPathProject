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
        public AdvertisingStatusType AdvertisingStatus { get; set; }
        public bool? HasInsurance { get; set; }
        public HousingType HousingType { get; set; }
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal GrossArea { get; set; }
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
        public decimal Price { get; set; }
        public CurrencyType Currency { get; set; }
        public string UserId { get; set; }

        // Related data
        public List<LocationCreateDto> Locations { get; set; } = new List<LocationCreateDto>();
        public List<PropertyImageCreateDto> Images { get; set; } = new List<PropertyImageCreateDto>();
        public List<int>? FeatureIds { get; set; }
    }
}
