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
                budget += FirstMonthBudget(period, budgets.FirstOrDefault(b => b.YearMonth == period.Start.ToString("yyyyMM")));

                budget += LastMonthBudget(period, budgets.FirstOrDefault(b => b.YearMonth == period.End.ToString("yyyyMM")));

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

            return SingleMonthBudget(period, budgets.FirstOrDefault(model => model.YearMonth == period.Start.ToString("yyyyMM")));
        }

        private decimal SingleMonthBudget(Period period, BudgetModel budget)
        {
            var days = CalculateDays(period);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            if (budget == null)
            {
                return 0;
            }

            return budget.Budget / totalDaysInAMonth * days;
        }

        private decimal LastMonthBudget(Period period, BudgetModel budget)
        {
            var lastMonthBudget = 0m;
            var endDays = CalculateDays(new Period(new DateTime(period.End.Year, period.End.Month, 1), period.End));
            var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
            if (budget != null)
            {
                lastMonthBudget = budget.Budget / totalEndDaysInAMonth * endDays;
            }

            return lastMonthBudget;
        }

        private decimal FirstMonthBudget(Period period, BudgetModel budget)
        {
            var firstMonthBudget = 0m;
            var totalStartDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var startDays = CalculateDays(new Period(period.Start,
                new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth)));
            if (budget!=null)
            {
                firstMonthBudget = budget.Budget / totalStartDaysInAMonth * startDays;
            }

            return firstMonthBudget;
        }

        private int CalculateDays(Period period)
        {
            return (period.End - period.Start).Days + 1;
        }
    }
}