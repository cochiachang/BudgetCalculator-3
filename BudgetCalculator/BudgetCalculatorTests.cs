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
            Add04toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-01"));

            Assert.AreEqual(20, actual);
        }

        private void Add04toRepo()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201804",
                    Budget = 600
                }
            });
        }

        private void Add03toRepo()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201803",
                    Budget = 310
                }
            });
        }

        private void Add0304toRepo()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201803",
                    Budget = 310
                },
                new BudgetModel
                {
                    YearMonth = "201804",
                    Budget = 600
                }
            });
        }

        private void Add030406toRepo()
        {
            _budgetRepository.SetBudgets(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201803",
                    Budget = 310
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
        }

        [TestMethod]
        public void OneMonth()
        {
            Add0304toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-30"));

            Assert.AreEqual(600, actual);
        }

        [TestMethod]
        public void TwoDaysInAMonth()
        {
            Add04toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-02"));
            Assert.AreEqual(40, actual);
        }

        [TestMethod]
        public void OutOfRange()
        {
            Add03toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-02"));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180301()
        {
            Add03toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-03-01"));
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180302()
        {
            Add03toRepo();

            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-03-02"));
            Assert.AreEqual(20, actual);
        }
        
        [TestMethod]
        public void CrossMonth_20180201_20180401()
        {
            Add0304toRepo();
            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-04-01"));
            Assert.AreEqual(330, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180701()
        {
            Add030406toRepo();
            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-07-01"));
            Assert.AreEqual(2110, actual);
        }

        [TestMethod]
        public void CrossMonth_20170201_20190701()
        {
            Add030406toRepo();
            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2017-02-01"), DateTime.Parse("2019-07-01"));
            Assert.AreEqual(2110, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180615()
        {
            Add030406toRepo();
            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-06-15"));
            Assert.AreEqual(1510, actual);
        }

        [TestMethod]
        public void CrossMonth_20180331_20180601()
        {
            Add030406toRepo();
            _sut.SetData(_budgetRepository);
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-03-31"), DateTime.Parse("2018-06-01"));
            Assert.AreEqual(650, actual);
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
            var budgets = _budgetRepository.GetAll();

            var budget = 0M;
            
            foreach (var budgetModel in budgets)
            {
                if (budgetModel.YearMonth != start.ToString("yyyyMM") &&
                    budgetModel.YearMonth != end.ToString("yyyyMM"))
                {
                    DateTime d = DateTime.ParseExact(budgetModel.YearMonth + "01", "yyyyMMdd", null);
                    if (d > start && d < end)
                    {
                        budget += budgetModel.Budget;
                    }
                }
            }
            
            if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
            {
                var totalStartDaysInAMonth = DateTime.DaysInMonth(start.Year, start.Month);
                var startDays = CalculateDays(start, new DateTime(start.Year,start.Month,totalStartDaysInAMonth));
                var startBudgetModels = budgets.Where(model => { return model.YearMonth == start.ToString("yyyyMM"); });
                if (startBudgetModels.Any())
                {
                    budget += startBudgetModels.First().Budget / totalStartDaysInAMonth * startDays;
                }
                
                var endDays = CalculateDays(new DateTime(end.Year,end.Month,1),end);
                var totalEndDaysInAMonth = DateTime.DaysInMonth(end.Year, end.Month);
                var endBudgetModels = budgets.Where(model => { return model.YearMonth == end.ToString("yyyyMM"); });
                if (endBudgetModels.Any())
                {
                    budget += endBudgetModels.First().Budget / totalEndDaysInAMonth * endDays;
                }

                return budget;
            }
            var days = CalculateDays(start, end);
            var totalDaysInAMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var budgetModels = budgets.Where(model => { return model.YearMonth == start.ToString("yyyyMM"); });
            if (!budgetModels.Any())
            {
                return 0;
            }

            return budgetModels.First().Budget / totalDaysInAMonth * days;
        }

        private int CalculateDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1;
        }
    }
}