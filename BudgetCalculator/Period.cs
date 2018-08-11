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

        public int OverlappingDays(Period otherPeriod)
        {
            if (End < otherPeriod.Start || Start > otherPeriod.End)
            {
                return 0;
            }

            var effectiveStart = Start;
            if (otherPeriod.Start > Start)
            {
                effectiveStart = otherPeriod.Start;
            }

            var effectiveEnd = End;
            if (otherPeriod.End < End)
            {
                effectiveEnd = otherPeriod.End;
            }

            return Days(effectiveStart, effectiveEnd);
        }
    }
}