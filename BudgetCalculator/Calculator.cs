using System;
using System.Collections.Generic;
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

        public bool IsQueryCrossMonth()
        {
            return Start.ToString("yyyyMM") != End.ToString("yyyyMM");
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
            decimal amountOfMiddleMonths = 0;
            foreach (var b in _budgetRepository.GetAll())
            {
                if (IsMiddleMonthOfPeriod(period, b))
                {
                    amountOfMiddleMonths += b.Budget;
                }
            }

            if (period.IsQueryCrossMonth())
            {
                var amountOfFirstMonth = AmountOfFirstMonth(period, budgets);

                var amountOfLastMonth = AmountOfLastMonth(period, budgets);

                return amountOfFirstMonth + amountOfLastMonth + amountOfMiddleMonths;
            }

            var days = CalculateDays(period.Start, period.End);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Budget / totalDaysInAMonth * days;
        }

        private decimal AmountOfLastMonth(Period period, List<BudgetModel> budgets)
        {
            var amountOfLastMonth = 0m;
            var endDays = CalculateDays(new DateTime(period.End.Year, period.End.Month, 1), period.End);
            var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
            var endBudgetModels = budgets.Where(model => { return model.YearMonth == period.End.ToString("yyyyMM"); });
            if (endBudgetModels.Any())
            {
                amountOfLastMonth += endBudgetModels.First().Budget / totalEndDaysInAMonth * endDays;
            }

            return amountOfLastMonth;
        }

        private decimal AmountOfFirstMonth(Period period, List<BudgetModel> budgets)
        {
            var amountOfFirstMonth = 0m;
            var totalStartDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var startDays = CalculateDays(period.Start,
                new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth));
            var startBudgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (startBudgetModels.Any())
            {
                amountOfFirstMonth += startBudgetModels.First().Budget / totalStartDaysInAMonth * startDays;
            }

            return amountOfFirstMonth;
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