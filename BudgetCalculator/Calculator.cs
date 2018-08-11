using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCalculator
{
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

            var totalAmount = 0M;

            if (period.isCrossMonth())
            {
                foreach (var budget in budgets)
                {
                    if (IsFirstMonth(budget, period))
                    {
                        totalAmount += FirstMonthBudget(period, budget);
                    }
                    if (IsLastMonth(budget, period))
                    {
                        totalAmount += LastMonthBudget(period, budget);
                    }
                    if (budget.YearMonth != period.Start.ToString("yyyyMM") &&
                        budget.YearMonth != period.End.ToString("yyyyMM"))
                    {
                        totalAmount += budget.Budget;
                    }
                }
                return totalAmount;
            }

            return SingleMonthBudget(period, budgets.FirstOrDefault(model => period.idSingleMonth(model)));
        }

        private static bool IsLastMonth(BudgetModel b, Period period)
        {
            return b.YearMonth == period.End.ToString("yyyyMM");
        }

        private static bool IsFirstMonth(BudgetModel b, Period period)
        {
            return b.YearMonth == period.Start.ToString("yyyyMM");
        }

        private decimal SingleMonthBudget(Period period, BudgetModel budget)
        {
            if (budget == null)
            {
                return 0;
            }
            var days = CalculateDays(period);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);

            return budget.Budget / totalDaysInAMonth * days;
        }

        private decimal LastMonthBudget(Period period, BudgetModel budget)
        {
            var lastMonthBudget = 0m;
            if (budget != null)
            {
                var endDays = CalculateDays(new Period(new DateTime(period.End.Year, period.End.Month, 1), period.End));
                var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
                lastMonthBudget = budget.Budget / totalEndDaysInAMonth * endDays;
            }

            return lastMonthBudget;
        }

        private decimal FirstMonthBudget(Period period, BudgetModel budget)
        {
            var firstMonthBudget = 0m;
            if (budget!=null)
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(budget.Year, budget.Month);
                var startDays = CalculateDays(new Period(period.Start,
                    new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth)));
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