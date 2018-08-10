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

        public int EffectiveDays(Period otherPeriod)
        {
            var effectiveStart = otherPeriod.Start > Start
                ? otherPeriod.Start
                : Start;

            var effectiveEnd = otherPeriod.End < End
                ? otherPeriod.End
                : End;

            return (effectiveEnd - effectiveStart).Days + 1;
        }
    }
}