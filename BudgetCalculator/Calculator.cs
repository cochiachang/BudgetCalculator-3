using System;
using System.Linq;

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
    }

    public class Calculator
    {
        private IBudgetRepository _budgetRepository;

        public Calculator(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public decimal CalculateBudget(DateTime start, DateTime end)
        {
            var budgets = _budgetRepository.GetAll();

            var period = new Period(start, end);

            if (period.IsSingleMonth())
            {
                var budget = budgets.FirstOrDefault(model => model.YearMonth == period.Start.ToString("yyyyMM"));
                return AmountOfSingleMonth(period, budget);
            }

            var totalAmount = 0m;
            foreach (var b in _budgetRepository.GetAll())
            {
                var effectiveDays = EffectiveDays(b, period);

                totalAmount += b.DailyAmount() * effectiveDays;
            }

            return totalAmount;
        }

        private int EffectiveDays(BudgetModel b, Period period)
        {
            var effectiveStart = period.Start;
            var effectiveEnd = period.End;

            if (IsFirstMonthBudget(b, period))
            {
                effectiveEnd = b.LastDay();
                effectiveStart = period.Start;
            }
            else if (IsLastMonthBudget(b, period))
            {
                effectiveStart = b.FirstDay();
                effectiveEnd = period.End;
            }
            else
            {
                effectiveStart = b.FirstDay();
                effectiveEnd = b.LastDay();
            }

            return CalculateDays(effectiveStart, effectiveEnd);
        }

        private static bool IsLastMonthBudget(BudgetModel b, Period period)
        {
            return b.YearMonth == period.End.ToString("yyyyMM");
        }

        private static bool IsFirstMonthBudget(BudgetModel model, Period period)
        {
            return model.YearMonth == period.Start.ToString("yyyyMM");
        }

        private decimal AmountOfSingleMonth(Period period, BudgetModel budget)
        {
            if (budget != null)
            {
                var days = CalculateDays(period.Start, period.End);
                var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);

                var amountOfSingleMonth = budget.Budget / totalDaysInAMonth * days;
                return amountOfSingleMonth;
            }

            return 0;
        }

        private static bool IsMiddleMonthOfPeriod(Period period, BudgetModel budgetModel)
        {
            return budgetModel.YearMonth != period.Start.ToString("yyyyMM") &&
                   budgetModel.YearMonth != period.End.ToString("yyyyMM");
        }

        private int CalculateDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }
    }
}