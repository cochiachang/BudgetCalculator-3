using System;
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
            var budgets = _budgetRepository.GetAll();

            var period = new Period(start, end);

            if (period.IsSingleMonth())
            {
                var budget = budgets.FirstOrDefault(model => model.YearMonth == period.Start.ToString("yyyyMM"));
                return AmountOfSingleMonth(period, budget);
            }

            var totalAmount = 0m;
            foreach (var b in _budgetRepository.GetAll())
            {
                var effectiveDays = period.EffectiveDays(b);

                totalAmount += b.DailyAmount() * effectiveDays;
            }

            return totalAmount;
        }

        private decimal AmountOfSingleMonth(Period period, BudgetModel budget)
        {
            if (budget != null)
            {
                var days = Period.CalculateDays(period.Start, period.End);
                var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);

                var amountOfSingleMonth = budget.Budget / totalDaysInAMonth * days;
                return amountOfSingleMonth;
            }

            return 0;
        }

        private static bool IsMiddleMonthOfPeriod(Period period, BudgetModel budgetModel)
        {
            return budgetModel.YearMonth != period.Start.ToString("yyyyMM") &&
                   budgetModel.YearMonth != period.End.ToString("yyyyMM");
        }
    }
}