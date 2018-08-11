using System.Collections.Generic;

namespace BudgetCalculator
{
    public interface IBudgetRepository
    {
        List<BudgetModel> GetAll();
    }
}