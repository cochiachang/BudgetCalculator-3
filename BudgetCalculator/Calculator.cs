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

        public bool isCrossMonth()
        {
            return Start.ToString("yyyyMM") != End.ToString("yyyyMM");
        }
    }

    public class Calculator
    {
        private IBudgetRepository _budgetRepository;

        public void SetData(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public decimal CalculateBudget(DateTime start, DateTime end)
        {
            var period = new Period(start, end);
            var budgets = _budgetRepository.GetAll();

            var budget = 0M;

            if (period.isCrossMonth())
            {
                budget += FirstMonthBudget(period, budgets);

                budget += LastMonthBudget(period, budgets);

                foreach (var budgetModel in budgets)
                {
                    if (budgetModel.YearMonth != period.Start.ToString("yyyyMM") &&
                        budgetModel.YearMonth != period.End.ToString("yyyyMM"))
                    {
                        budget += budgetModel.Budget;
                    }
                }
                return budget;
            }

            return SingleMonthBudget(period, budgets);
        }

        private decimal SingleMonthBudget(Period period, List<BudgetModel> budgets)
        {
            var days = CalculateDays(period);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Budget / totalDaysInAMonth * days;
        }

        private decimal LastMonthBudget(Period period, List<BudgetModel> budgets)
        {
            var lastMonthBudget = 0m;
            var endDays = CalculateDays(new Period(new DateTime(period.End.Year, period.End.Month, 1), period.End));
            var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
            var endBudgetModels = budgets.Where(model => { return model.YearMonth == period.End.ToString("yyyyMM"); });
            if (endBudgetModels.Any())
            {
                lastMonthBudget = endBudgetModels.First().Budget / totalEndDaysInAMonth * endDays;
            }

            return lastMonthBudget;
        }

        private decimal FirstMonthBudget(Period period, List<BudgetModel> budgets)
        {
            var firstMonthBudget = 0m;
            var totalStartDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var startDays = CalculateDays(new Period(period.Start,
                new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth)));
            var startBudgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (startBudgetModels.Any())
            {
                firstMonthBudget = startBudgetModels.First().Budget / totalStartDaysInAMonth * startDays;
            }

            return firstMonthBudget;
        }

        private int CalculateDays(Period period)
        {
            return (period.End - period.Start).Days + 1;
        }
    }
}