using System;

namespace BudgetCalculator
{
    public class Period
    {
        public Period(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public bool IsSingleMonth()
        {
            return Start.ToString("yyyyMM") == End.ToString("yyyyMM");
        }

        public static int Days(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }

        public int OverlappingDays(BudgetModel budget)
        {
            if (HasNoOverlapping(budget))
            {
                return 0;
            }

            var effectiveStart = this.Start;
            if (budget.FirstDay() > this.Start)
            {
                effectiveStart = budget.FirstDay();
            }

            var effectiveEnd = this.End;
            if (budget.LastDay() < this.End)
            {
                effectiveEnd = budget.LastDay();
            }

            return Period.Days(effectiveStart, effectiveEnd);
        }

        private bool HasNoOverlapping(BudgetModel budget)
        {
            return End < budget.FirstDay() || Start > budget.LastDay();
        }
    }
}