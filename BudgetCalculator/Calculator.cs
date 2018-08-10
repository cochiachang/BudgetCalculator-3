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

            var totalAmount = 0m;
            foreach (var b in _budgetRepository.GetAll())
            {
                totalAmount += b.EffectiveAmount(period);
            }

            return totalAmount;
        }
    }
}