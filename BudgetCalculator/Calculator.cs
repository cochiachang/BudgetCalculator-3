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
            var amountOfFirstMonth = 0m;
            foreach (var b in _budgetRepository.GetAll())
            {
                if (IsMiddleMonthOfPeriod(period, b))
                {
                    amountOfMiddleMonths += b.Budget;
                }

                if (IsFirstMonthBudget(b, period))
                {
                    amountOfFirstMonth = AmountOfFirstMonth(period, b);
                }
            }

            if (period.IsQueryCrossMonth())
            {
                //var amountOfFirstMonth = AmountOfFirstMonth(period, budgets.FirstOrDefault(b => IsFirstMonthBudget(b, period)));

                var amountOfLastMonth = AmountOfLastMonth(period, budgets);

                return amountOfFirstMonth + amountOfLastMonth + amountOfMiddleMonths;
            }

            var amountOfSingleMonth = AmountOfSingleMonth(period, budgets);
            return amountOfSingleMonth;
        }

        private static bool IsFirstMonthBudget(BudgetModel model, Period period)
        {
            return model.YearMonth == period.Start.ToString("yyyyMM");
        }

        private decimal AmountOfSingleMonth(Period period, List<BudgetModel> budgets)
        {
            var budgetModels = budgets.Where(model => { return model.YearMonth == period.Start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            var days = CalculateDays(period.Start, period.End);
            var totalDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);

            var amountOfSingleMonth = budgetModels.First().Budget / totalDaysInAMonth * days;
            return amountOfSingleMonth;
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

        private decimal AmountOfFirstMonth(Period period, BudgetModel budgetOfStartMonth)
        {
            if (budgetOfStartMonth != null)
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(period.Start.Year, period.Start.Month);
                var endDateOfFirstMonth = new DateTime(period.Start.Year, period.Start.Month, totalStartDaysInAMonth);
                var startDays = CalculateDays(period.Start, endDateOfFirstMonth);
                var dailyAmount = budgetOfStartMonth.Budget / totalStartDaysInAMonth;
                return dailyAmount * startDays;
            }

            return 0;
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