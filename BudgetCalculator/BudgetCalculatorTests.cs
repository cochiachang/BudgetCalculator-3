using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BudgetCalculator
{
    [TestClass]
    public class BudgetCalculatorTests
    {
        private BudgetRepository _budgetRepository;
        private Calculator _sut;

        public BudgetCalculatorTests()
        {
            _budgetRepository = new BudgetRepository();
            _sut = new Calculator();
        }

        [TestMethod]
        public void OneDay()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
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
            });

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-01"));

            Assert.AreEqual(20, actual);
        }

        [TestMethod]
        public void OneMonth()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
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
            });

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-30"));

            Assert.AreEqual(600, actual);
        }

        [TestMethod]
        public void TwoDaysInAMonth()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
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
            });

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-02"));
            Assert.AreEqual(40, actual);
        }
        
        [TestMethod]
        public void OutOfRange()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
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
            });

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-02"));
            Assert.AreEqual(0, actual);
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
            var days = CalculateDays(start, end);
            var totalDaysInAMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var budgets = _budgetRepository.GetAll();
            var budgetModels = budgets.Where(model => { return model.YearMonth == start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }
            var budget = budgetModels.First().Budget / totalDaysInAMonth * days;
            return budget;
        }

        private int CalculateDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }
    }
}