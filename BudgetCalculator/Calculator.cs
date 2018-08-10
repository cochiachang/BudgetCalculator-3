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
            decimal amountOfMiddleMonths = 0;
            foreach (var b in _budgetRepository.GetAll())
            {
                if (IsMiddleMonthOfPeriod(period, b))
                {
                    amountOfMiddleMonths += b.Budget;
                }
            }

            var budget = 0M;

            var periodStart = period.Start;
            var periodEnd = period.End;
            if (periodStart.ToString("yyyyMM") != periodEnd.ToString("yyyyMM"))
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(periodStart.Year, periodStart.Month);
                var startDays = CalculateDays(periodStart, new DateTime(periodStart.Year, periodStart.Month, totalStartDaysInAMonth));
                var startBudgetModels = budgets.Where(model => { return model.YearMonth == periodStart.ToString("yyyyMM"); });
                if (startBudgetModels.Any())
                {
                    budget += startBudgetModels.First().Budget / totalStartDaysInAMonth * startDays;
                }

                var endDays = CalculateDays(new DateTime(periodEnd.Year, periodEnd.Month, 1), periodEnd);
                var totalEndDaysInAMonth = DateTime.DaysInMonth(periodEnd.Year, periodEnd.Month);
                var endBudgetModels = budgets.Where(model => { return model.YearMonth == periodEnd.ToString("yyyyMM"); });
                if (endBudgetModels.Any())
                {
                    budget += endBudgetModels.First().Budget / totalEndDaysInAMonth * endDays;
                }

                return budget + amountOfMiddleMonths;
            }

            var days = CalculateDays(periodStart, periodEnd);
            var totalDaysInAMonth = DateTime.DaysInMonth(periodStart.Year, periodStart.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == periodStart.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Budget / totalDaysInAMonth * days;
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