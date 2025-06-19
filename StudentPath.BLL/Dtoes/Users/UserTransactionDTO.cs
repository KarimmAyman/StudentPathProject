using StudentPath.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Users
{
    public class UserTransactionDTO
    {

        [JsonIgnore]
        public PaymentMethodEnum PaymentMethod { get; set; }
        [JsonIgnore]
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public string FormattedDate
        {
            get
            {
                // Define Egypt Time Zone
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Convert UTC TransactionDate to Egypt Local Time
                var localEgyptTime = TimeZoneInfo.ConvertTimeFromUtc(PaymentDate, egyptTimeZone);

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
        public string FormattedPaymentMethod
        {
            get
            {
                return PaymentMethod switch
                {
                    PaymentMethodEnum.CreditCard => "Credit Card",
                    PaymentMethodEnum.Wallet => "Wallet",
                    _ => "Unknown"
                };
            }
        }


    }
}
