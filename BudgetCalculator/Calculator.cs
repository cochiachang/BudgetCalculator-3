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
                foreach (var budgetModel in budgets)
                {
                    if (IsMiddleMonth(period, budgetModel))
                    {
                        totalAmount += budgetModel.Amount;
                    }
                }
                var amountOfFirstMonth = AmountOfFirstMonth(period, budgets.FirstOrDefault(b => IsFirstMonth(b, period)));
                totalAmount += amountOfFirstMonth;

                var amountOfLastMonth = AmountOfLastMonth(period, budgets.FirstOrDefault(b => IsLastMonth(b, period)));
                totalAmount += amountOfLastMonth;

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

        private decimal AmountOfLastMonth(Period period, BudgetModel budget)
        {
            if (budget != null)
            {
                var effectiveStart = budget.FirstDay();
                var effectiveEnd = period.End;
                var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                return budget.DailyAmount() * effectiveDays;
            }

            return 0;
        }

        private decimal AmountOfFirstMonth(Period period, BudgetModel budget)
        {
            if (budget != null)
            {
                var effectiveStart = period.Start;
                var effectiveEnd = budget.LastDay();
                var effectiveDays = CalculateDays(effectiveStart, effectiveEnd);
                return budget.DailyAmount() * effectiveDays;
            }

            return 0;
        }

        private static bool IsMiddleMonth(Period period, BudgetModel budgetModel)
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