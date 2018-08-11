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
                    if (IsFirstMonth(budget, period))
                    {
                        var effectiveStart = period.Start;
                        var effectiveEnd = budget.LastDay();
                        var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                        var effectiveAmount = budget.DailyAmount() * effectiveDays;
                        totalAmount += effectiveAmount;
                    }
                    else if (IsLastMonth(budget, period))
                    {
                        var effectiveStart = budget.FirstDay();
                        var effectiveEnd = period.End;
                        var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                        var effectiveAmount = budget.DailyAmount() * effectiveDays;
                        totalAmount += effectiveAmount;
                    }
                    else
                    {
                        var effectiveStart = budget.FirstDay();
                        var effectiveEnd = budget.LastDay();
                        var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                        var effectiveAmount = budget.DailyAmount() * effectiveDays;
                        totalAmount += effectiveAmount;
                    }
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