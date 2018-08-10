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

        public static int CalculateDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }

        public int EffectiveDays(BudgetModel b)
        {
            var effectiveStart = this.Start;
            var effectiveEnd = this.End;

            if (b.YearMonth == this.Start.ToString("yyyyMM"))
            {
                effectiveEnd = b.LastDay();
            }
            else if (b.YearMonth == this.End.ToString("yyyyMM"))
            {
                effectiveStart = b.FirstDay();
            }
            else
            {
                effectiveStart = b.FirstDay();
                effectiveEnd = b.LastDay();
            }

            return Period.CalculateDays(effectiveStart, effectiveEnd);
        }
    }
}