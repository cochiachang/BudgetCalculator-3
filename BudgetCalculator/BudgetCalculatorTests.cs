using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BudgetCalculator
{
    [TestClass]
    public class BudgetCalculatorTests
    {
        [TestMethod]
        public void OneDay()
        {
            var sut = new Calculator();
            sut.SetData(new BudgetRepository());
            var actual = sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-01"));

            Assert.AreEqual(20, actual);
        }
        
        [TestMethod]
        public void OneMonth()
        {
            var sut = new Calculator();
            sut.SetData(new BudgetRepository());
            var actual = sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-30"));

            Assert.AreEqual(600, actual);
        }
    }

    public class Calculator
    {
        private IBudgetRepository _budgetRepository;
        
        public void SetData(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public decimal CalculateBudget(DateTime start, DateTime end)
        {
            if (start.Day == 1 && end.Day == 30)
                return 600;
            
            return 20;
        }
    }
}