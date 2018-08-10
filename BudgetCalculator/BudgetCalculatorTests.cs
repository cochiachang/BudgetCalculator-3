using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCalculator
{
    [TestClass]
    public class BudgetCalculatorTests
    {
        private IBudgetRepository _budgetRepository;
        private Calculator _sut;

        public BudgetCalculatorTests()
        {
            _budgetRepository = Substitute.For<IBudgetRepository>();
            _sut = new Calculator(_budgetRepository);
        }

        [TestMethod]
        public void CrossMonth_20170201_20190701()
        {
            Add030406toRepo();
            var actual = _sut.CalculateBudget(DateTime.Parse("2017-02-01"), DateTime.Parse("2019-07-01"));
            Assert.AreEqual(2110, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180301()
        {
            Add03toRepo();

            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-03-01"));
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180302()
        {
            Add03toRepo();

            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-03-02"));
            Assert.AreEqual(20, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180401()
        {
            Add0304toRepo();
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-04-01"));
            Assert.AreEqual(330, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180615()
        {
            Add030406toRepo();
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-06-15"));
            Assert.AreEqual(1510, actual);
        }

        [TestMethod]
        public void CrossMonth_20180201_20180701()
        {
            Add030406toRepo();
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-02-01"), DateTime.Parse("2018-07-01"));
            Assert.AreEqual(2110, actual);
        }

        [TestMethod]
        public void CrossMonth_20180331_20180601()
        {
            Add030406toRepo();
            var actual = _sut.CalculateBudget(DateTime.Parse("2018-03-31"), DateTime.Parse("2018-06-01"));
            Assert.AreEqual(650, actual);
        }

        [TestMethod]
        public void OneDay()
        {
            GivenBudgets(new BudgetModel { YearMonth = "201804", Budget = 600 });
            TotalAmountShouldBe(expected: 20m, start: "2018-04-01", end: "2018-04-01");
        }

        [TestMethod]
        public void OneMonth()
        {
            Add0304toRepo();

            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-30"));

            Assert.AreEqual(600, actual);
        }

        [TestMethod]
        public void OutOfRange()
        {
            Add03toRepo();

            var actual = _sut.CalculateBudget(DateTime.Parse("2018-01-01"), DateTime.Parse("2018-01-02"));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TwoDaysInAMonth()
        {
            Add04toRepo();

            var actual = _sut.CalculateBudget(DateTime.Parse("2018-04-01"), DateTime.Parse("2018-04-02"));
            Assert.AreEqual(40, actual);
        }

        private void Add030406toRepo()
        {
            _budgetRepository.GetAll().Returns(new List<BudgetModel>
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

        private void Add0304toRepo()
        {
            _budgetRepository.GetAll().Returns(new List<BudgetModel>
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

        private void Add03toRepo()
        {
            _budgetRepository.GetAll().Returns(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201803",
                    Budget = 310
                }
            });
        }

        private void Add04toRepo()
        {
            _budgetRepository.GetAll().Returns(new List<BudgetModel>
            {
                new BudgetModel
                {
                    YearMonth = "201804",
                    Budget = 600
                }
            });
        }

        private void GivenBudgets(params BudgetModel[] budgets)
        {
            _budgetRepository.GetAll().Returns(budgets.ToList());
        }

        private void TotalAmountShouldBe(decimal expected, string start, string end)
        {
            Assert.AreEqual(expected, _sut.CalculateBudget(DateTime.Parse(start), DateTime.Parse(end)));
        }
    }
}