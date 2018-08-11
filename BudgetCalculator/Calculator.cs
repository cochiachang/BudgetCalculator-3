using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCalculator
{
    public class Calculator
    {
        private IBudgetRepository _budgetRepository;

        public Calculator(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public decimal CalculateBudget(DateTime start, DateTime end)
        {
            var period = new Period(start, end);

            var budgets = _budgetRepository.GetAll();

            var totalAmount = 0M;

            if (period.IsSingleMonth())
            {
                return AmountOfSingleMonth(period, budgets);
            }
            else
            {
                foreach (var budget in budgets)
                {
                    var effectiveStart = period.Start;
                    var effectiveEnd = period.End;
                    if (IsFirstMonth(budget, period))
                    {
                        effectiveStart = period.Start;
                        effectiveEnd = budget.LastDay();
                    }
                    else if (IsLastMonth(budget, period))
                    {
                        effectiveStart = budget.FirstDay();
                        effectiveEnd = period.End;
                    }
                    else
                    {
                        effectiveStart = budget.FirstDay();
                        effectiveEnd = budget.LastDay();
                    }
                    var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                    var effectiveAmount = budget.DailyAmount() * effectiveDays;
                    totalAmount += effectiveAmount;
                }

                return totalAmount;
            }
        }

        private static bool IsLastMonth(BudgetModel model, Period period)
        {
            return model.YearMonth == period.End.ToString("yyyyMM");
        }

        private static bool IsFirstMonth(BudgetModel model, Period period)
        {
            return model.YearMonth == period.Start.ToString("yyyyMM");
        }

        private decimal AmountOfSingleMonth(Period period, List<BudgetModel> budgets)
        {
            var days = CalculateDays(period.Start, period.End);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Amount / totalDaysInAMonth * days;
        }

        private int CalculateDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }
    }
}