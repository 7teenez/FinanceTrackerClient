using System.Collections.Generic;
using System;

namespace FinanceTrackerClient.Models
{
    public class CategoryStat
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }

    public class TimePoint
    {
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }

    public class AnalyticsResult
    {
        public List<CategoryStat> Categories { get; set; }
        public List<TimePoint> Timeline { get; set; }
        public string Period { get; set; } // для експорту
    }
}