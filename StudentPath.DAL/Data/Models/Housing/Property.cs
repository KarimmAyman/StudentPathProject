using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudentPath.DAL.Data.Models.Housing
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        // ENUM instead of string for predefined values
        [Required]
        public AdvertisingStatusType AdvertisingStatus { get; set; } // Sale, Rent

        [Required]

        public HousingType HousingType { get; set; } // Apartment, House, etc.

        // Normalized Room & Area Fields
        public int Rooms { get; set; } // Number of rooms
        public int Bathrooms { get; set; } // Number of bathrooms

        [Required]
        public decimal GrossArea { get; set; } // m²

        [Required]
        public decimal NetArea { get; set; } // m²

        public WarmingType? WarmingType { get; set; } // Natural Gas, etc.

        public int? BuildingAge { get; set; } // Nullable for unknown age

       
        public int? FloorLocation { get; set; } // Nullable if ground floor

        public bool? IsFurnished { get; set; } // Nullable for optional input
        public bool? IsAvailableForLoan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Dues { get; set; } // Monthly dues (Nullable if not applicable)

        public PropertyFrontType? Front { get; set; } // Enum for direction

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RentalIncome { get; set; } // Nullable for non-rent properties

        public string? Description { get; set; }

        // Price Information
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Sale price

        [Required]
        public CurrencyType Currency { get; set; } // Enum for currency type

        // Owner Information (Foreign Key)
        [ForeignKey("User")]
        public string UserId { get; set; } // IdentityUser uses string IDs
        public virtual User User { get; set; }

        // Navigation Properties
        public virtual List<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public virtual List<PropertyFeature> PropertyFeatures { get; set; } = new List<PropertyFeature>();
        public virtual ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }

    // ENUMS FOR BETTER DATA VALIDATION & READABILITY
    public enum AdvertisingStatusType
    {
        Sale,
        Rent
    }

    public enum HousingType
    {
        Apartment,
        House,
        Villa,
        Duplex,
        Penthouse,
        Loft,
        Studio
    }

    public enum WarmingType
    {
        NaturalGas,
        Central,
        Electric,
        Solar,
        Underfloor
    }

    public enum PropertyFrontType
    {
        North,
        South,
        East,
        West,
        Northeast,
        Northwest,
        Southeast,
        Southwest
    }

    public enum CurrencyType
    {
        USD,
        EUR,
        GBP,
        EGP,
        SAR,
        AED
    }
}
