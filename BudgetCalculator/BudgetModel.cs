using System;

namespace BudgetCalculator
{
    public class BudgetModel
    {
        public decimal Budget { get; set; }
        public string YearMonth { get; set; }
        private decimal DailyAmount => Budget / TotalDaysInMonth;
        private DateTime FirstDay => new DateTime(Year, Month, 1);
        private DateTime LastDay => new DateTime(Year, Month, TotalDaysInMonth);
        private int Month => Convert.ToInt16(YearMonth.Substring(4, 2));
        private int TotalDaysInMonth => DateTime.DaysInMonth(Year, Month);
        private int Year => Convert.ToInt16(YearMonth.Substring(0, 4));

        public decimal EffectiveAmount(Period period)
        {
            return DailyAmount * period.EffectiveDays(CreatePeriodByBudget());
        }

        private Period CreatePeriodByBudget()
        {
            return new Period(FirstDay, LastDay);
        }
    }
}