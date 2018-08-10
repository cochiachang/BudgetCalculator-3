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

        public int EffectiveDays(Period otherPeriod)
        {
            if (HasNoOverlapping(otherPeriod))
            {
                return 0;
            }

            var effectiveStart = otherPeriod.Start > Start
                ? otherPeriod.Start
                : Start;

            var effectiveEnd = otherPeriod.End < End
                ? otherPeriod.End
                : End;

            return (effectiveEnd - effectiveStart).Days + 1;
        }

        private bool HasNoOverlapping(Period otherPeriod)
        {
            return End < otherPeriod.Start || Start > otherPeriod.End;
        }
    }
}