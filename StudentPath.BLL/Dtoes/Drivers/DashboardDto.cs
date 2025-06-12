using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Drivers
{
    public class DashboardDto
    {
        public decimal Balance { get; set; }
        public decimal EarningsSummary { get; set; }
        public int CompletedTripsCount { get; set; }
        public WeeklyTripStatsDto WeeklyStats { get; set; }
    }

    public class WeeklyTripStatsDto
    {
        public int Sunday { get; set; }
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
    }
}
