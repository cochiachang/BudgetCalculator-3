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
            foreach (var budget in budgets)
            {
                var overlappingDays = period.OverlappingDays(budget);
                totalAmount += budget.DailyAmount() * overlappingDays;
            }

            return totalAmount;
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
            var days = Period.Days(period.Start, period.End);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Amount / totalDaysInAMonth * days;
        }
    }
}