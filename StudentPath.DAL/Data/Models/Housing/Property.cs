using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        public string AdvertisingStatus { get; set; } // Sale, Rent

        [Required]
        public string HousingType { get; set; } // Apartment, House

        public string RoomsBathrooms { get; set; } // 3+1, 2+1, etc.

        public string GrossNetM2 { get; set; } // 150m² / 125m²

        public string WarmingType { get; set; } // Natural Gas, etc.

        public int BuildingAge { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public int FloorLocation { get; set; }

        public bool IsFurnished { get; set; }

        public decimal Dues { get; set; } // Monthly dues

        public string Front { get; set; } // Northwest, etc.

        public decimal RentalIncome { get; set; } // Potential income

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; } //fore selling

        [Required]
        public string Currency { get; set; } // USD, EUR, etc.

        [ForeignKey("Owner")]
        public string OwnerId { get; set; } // IdentityUser uses string IDs
        public User Owner { get; set; }


        public List<PropertyImage> PropertyImages { get; set; }

        public List<PropertyFeature> PropertyFeatures { get; set; }
    }
}
