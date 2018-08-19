using System;

namespace BudgetCalculator
{
    public class BudgetModel
    {
        public string YearMonth { get; set; }

        public decimal Budget { get; set; }

        public int Year => int.Parse(YearMonth.Substring(0, 4));
        public int Month => int.Parse(YearMonth.Substring(4, 2));

        public decimal PerDayBudget => Budget / DaysInMonth();

        public int DaysInThisBudget(Period period)
        {
            DateTime start = BudgetPeriod.Start > period.Start ? BudgetPeriod.Start : period.Start;
            DateTime end = BudgetPeriod.End < period.End ? BudgetPeriod.End : period.End;
            
            return (end - start).Days + 1;
        }

        public Period BudgetPeriod => new Period(new DateTime(Year,Month,1), new DateTime(Year,Month,DaysInMonth()));

        private int DaysInMonth()
        {
            return DateTime.DaysInMonth(Year, Month);
        }
    }
}