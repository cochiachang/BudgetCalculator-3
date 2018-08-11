using System;
using System.Collections.Generic;

namespace BudgetCalculator
{
    public interface IBudgetRepository
    {
        List<BudgetModel> GetAll();
    }

    public class BudgetRepository : IBudgetRepository
    {
        private List<BudgetModel> _bugets;

        public List<BudgetModel> GetAll()
        {
            return _bugets;
        }

        public void SetBudgets(List<BudgetModel> bugets)
        {
            _bugets = bugets;
        }
    }

    public class BudgetModel
    {
        public string YearMonth { get; set; }

        public decimal Amount { get; set; }

        public int Year => Convert.ToInt16(YearMonth.Substring(0, 4));

        public int Month => Convert.ToInt16(YearMonth.Substring(4, 2));
    }
}