using System;

namespace BudgetCalculator
{
    public class BudgetModel
    {
        public decimal Budget { get; set; }
        public int Month => Convert.ToInt16(YearMonth.Substring(4, 2));
        public int Year => Convert.ToInt16(YearMonth.Substring(0, 4));
        public string YearMonth { get; set; }

        public decimal DailyAmount()
        {
            var dailyAmount = Budget / TotalDaysInMonth();
            return dailyAmount;
        }

        public DateTime LastDay()
        {
            var effectiveEnd = new DateTime(Year, Month, TotalDaysInMonth());
            return effectiveEnd;
        }

        private int TotalDaysInMonth()
        {
            return DateTime.DaysInMonth(Year, Month);
        }
    }
}