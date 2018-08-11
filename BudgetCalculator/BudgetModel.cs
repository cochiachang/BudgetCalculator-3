using System;

namespace BudgetCalculator
{
    public class BudgetModel
    {
        public decimal Amount { get; set; }
        public int Month => Convert.ToInt16(YearMonth.Substring(4, 2));
        public int Year => Convert.ToInt16(YearMonth.Substring(0, 4));
        public string YearMonth { get; set; }
        private decimal DailyAmount => Amount / DaysInMonth;

        private int DaysInMonth => DateTime.DaysInMonth(Year, Month);

        private DateTime FirstDay => new DateTime(Year, Month, 1);

        private DateTime LastDay => new DateTime(Year, Month, DaysInMonth);

        private Period CreatePeriod => new Period(FirstDay, LastDay);

        public decimal EffectiveTotalAmount(Period period)
        {
            return DailyAmount * period.OverlappingDays(CreatePeriod);
        }
    }
}