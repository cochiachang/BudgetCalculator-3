using System.Collections.Generic;

namespace BudgetCalculator
{
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
}