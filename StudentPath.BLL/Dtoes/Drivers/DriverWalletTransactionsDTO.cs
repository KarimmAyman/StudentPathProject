using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class DriverWalletTransactionsDTO
    {
     
            public decimal Amount { get; set; }
           [JsonIgnore]
            public DateTime TransactionDate { get; set; }
            public string Operation { get; set; }

        public string FormattedDate
        {
            get
            {
                // Convert UTC to Egypt time (handles daylight saving automatically)
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                var localEgyptTime = TimeZoneInfo.ConvertTimeFromUtc(TransactionDate, egyptTimeZone);

                if (localEgyptTime.Date == DateTime.UtcNow.Date)
                {
                    return $"Today {localEgyptTime.ToString("HH:mm")}";
                }
                else if (localEgyptTime.Date == DateTime.UtcNow.Date.AddDays(-1))
                {
                    return $"Yesterday {localEgyptTime:HH:mm}";
                }
                return localEgyptTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }



    public class WithdrawWalletDto
    {
        public decimal Amount { get; set; }
    }

}
