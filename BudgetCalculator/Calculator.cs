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
            var period = new Period(start, end);

            var budgets = _budgetRepository.GetAll();

            var budget = 0M;

            foreach (var budgetModel in budgets)
            {
                if (IsMiddleMonth(period, budgetModel))
                {
                    DateTime firstDayOfBudgetMonth = DateTime.ParseExact(budgetModel.YearMonth + "01", "yyyyMMdd", null);
                    if (firstDayOfBudgetMonth > period.Start && firstDayOfBudgetMonth < period.End)
                    {
                        budget += budgetModel.Amount;
                    }
                }
            }

            if (period.Start.ToString("yyyyMM") != period.End.ToString("yyyyMM"))
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
                var startDays = CalculateDays(period.Start, new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth));
                var startBudgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
                if (startBudgetModels.Any())
                {
                    budget += startBudgetModels.First().Amount / totalStartDaysInAMonth * startDays;
                }

                var endDays = CalculateDays(new DateTime(period.End.Year, period.End.Month, 1), period.End);
                var totalEndDaysInAMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
                var endBudgetModels = budgets.Where(model => { return model.YearMonth == period.End.ToString("yyyyMM"); });
                if (endBudgetModels.Any())
                {
                    budget += endBudgetModels.First().Amount / totalEndDaysInAMonth * endDays;
                }

                return budget;
            }

            var days = CalculateDays(period.Start, period.End);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Amount / totalDaysInAMonth * days;
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