using System.Collections.Generic;

namespace BudgetCalculator
{
    public interface IBudgetRepository
    {
        List<BudgetModel> GetAll();
    }

    public class BudgetRepository : IBudgetRepository
    {
        public List<BudgetModel> GetAll()
        {
            return new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201803",
                    Budget = 300
                },
                new BudgetModel
                {
                    YearMonth = "201804",
                    Budget = 600
                },
                new BudgetModel
                {
                    YearMonth = "201806",
                    Budget = 1200
                }
            };
        }
    }

    public class BudgetModel
    {
        public string YearMonth { get; set; }

        public decimal Budget { get; set; }
    }
}