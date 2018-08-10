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

            decimal amountOfMiddleMonths = 0;
            var amountOfFirstMonth = 0m;
            var amountOfLastMonth = 0m;
            foreach (var b in _budgetRepository.GetAll())
            {
                if (IsMiddleMonthOfPeriod(period, b))
                {
                    amountOfMiddleMonths += b.Budget;
                }

                if (IsFirstMonthBudget(b, period))
                {
                    amountOfFirstMonth = AmountOfFirstMonth(period, b);
                }

                if (IsLastMonthBudget(b, period))
                {
                    amountOfLastMonth = AmountOfLastMonth(period, b);
                }
            }
            return amountOfFirstMonth + amountOfLastMonth + amountOfMiddleMonths;
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

        private decimal AmountOfLastMonth(Period period, BudgetModel firstOrDefault)
        {
            if (firstOrDefault != null)
            {
                var endDays = CalculateDays(new DateTime(period.End.Year, period.End.Month, 1), period.End);
                var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
                return firstOrDefault.Budget / totalEndDaysInAMonth * endDays;
            }

            return 0m;
        }

        private decimal AmountOfFirstMonth(Period period, BudgetModel budget)
        {
            if (budget != null)
            {
                var totalDaysInMonth = budget.TotalDaysInMonth();
                var effectiveEnd = new DateTime(period.Start.Year, period.Start.Month, totalDaysInMonth);
                var effectiveStart = period.Start;
                var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                var dailyAmount = budget.Budget / totalDaysInMonth;
                return dailyAmount * effectiveDays;
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