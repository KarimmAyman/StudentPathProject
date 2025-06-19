using StudentPath.BLL.Dtoes.Trips;
using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Bookings
{
    public class UserBookingDTO
    {

        
            public TripLocationDto FromLocation { get; set; }
            public TripLocationDto ToLocation { get; set; }

              [JsonIgnore]
            public TripStatus TripStatus { get; set; }

             [JsonIgnore]
            public DateTime BookingDate { get; set; }
            public int TotalSeats { get; set; }


        public string FormattedDate
        {
            get
            {
                // Define Egypt Time Zone
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Convert UTC TransactionDate to Egypt Local Time
                var localEgyptTime = TimeZoneInfo.ConvertTimeFromUtc(BookingDate, egyptTimeZone);

                // Get Egypt's current local date for comparison
                var nowEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                if (localEgyptTime.Date == nowEgypt.Date)
                {
                    return $"Today {localEgyptTime:HH:mm}";
                }
                else if (localEgyptTime.Date == nowEgypt.Date.AddDays(-1))
                {
                    return $"Yesterday {localEgyptTime:HH:mm}";
                }
                else
                {
                    return localEgyptTime.ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        public string FormattedTripStatus
        {
            get
            {
                return TripStatus switch
                {
                    TripStatus.Planned => "Planned",
                    TripStatus.Completed => "Completed",
                    TripStatus.Active => "Active",
                    TripStatus.Canceled => "Canceled",
                    _ => "Unknown"
                };
            }
        }
    }


    }

