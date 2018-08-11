using System;

namespace BudgetCalculator
{
    public class BudgetModel
    {
        public string YearMonth { get; set; }

        public decimal Budget { get; set; }

        public int Year => int.Parse(YearMonth.Substring(0, 4));
        public int Month => int.Parse(YearMonth.Substring(4, 2));
    }
}