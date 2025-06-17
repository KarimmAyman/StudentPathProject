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
                // Define Egypt Time Zone
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Convert UTC TransactionDate to Egypt Local Time
                var localEgyptTime = TimeZoneInfo.ConvertTimeFromUtc(TransactionDate, egyptTimeZone);

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
    }



    public class WithdrawWalletDto
    {
        public decimal Amount { get; set; }
    }

}
