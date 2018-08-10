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
            var budget = 0M;

            var amountOfMiddleMonths = 0m;
            foreach (var budgetModel in budgets)
            {
                if (IsMiddleMonthOfPeriod(period, budgetModel))
                {
                    DateTime firstDayOfBudgetMonth =
                        DateTime.ParseExact(budgetModel.YearMonth + "01", "yyyyMMdd", null);
                    if (firstDayOfBudgetMonth > start && firstDayOfBudgetMonth < end)
                    {
                        amountOfMiddleMonths += budgetModel.Budget;
                    }
                }
            }

            if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(start.Year, start.Month);
                var startDays = CalculateDays(start, new DateTime(start.Year, start.Month, totalStartDaysInAMonth));
                var startBudgetModels = budgets.Where(model => { return model.YearMonth == start.ToString("yyyyMM"); });
                if (startBudgetModels.Any())
                {
                    budget += startBudgetModels.First().Budget / totalStartDaysInAMonth * startDays;
                }

                var endDays = CalculateDays(new DateTime(end.Year, end.Month, 1), end);
                var totalEndDaysInAMonth = DateTime.DaysInMonth(end.Year, end.Month);
                var endBudgetModels = budgets.Where(model => { return model.YearMonth == end.ToString("yyyyMM"); });
                if (endBudgetModels.Any())
                {
                    budget += endBudgetModels.First().Budget / totalEndDaysInAMonth * endDays;
                }

                return budget + amountOfMiddleMonths;
            }

            var days = CalculateDays(start, end);
            var totalDaysInAMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == start.ToString("yyyyMM"); });
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