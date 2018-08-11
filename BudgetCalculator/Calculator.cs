using System;

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
                totalAmount += budget.EffectiveTotalAmount(period);
            }

            return totalAmount;
        }
    }
}