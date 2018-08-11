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

        public DateTime End { get; private set; }
        public DateTime Start { get; private set; }

        public int OverlappingDays(Period otherPeriod)
        {
            if (HasNoOverlapping(otherPeriod))
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

            return (effectiveEnd - effectiveStart).Days + 1;
        }

        private bool HasNoOverlapping(Period otherPeriod)
        {
            return End < otherPeriod.Start || Start > otherPeriod.End;
        }
    }
}