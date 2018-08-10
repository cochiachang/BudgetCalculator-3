using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
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
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 },
                new BudgetModel { YearMonth = "201806", Budget = 1200 });

            TotalAmountShouldBe(2110, "2017-02-01", "2019-07-01");
        }

        [TestMethod]
        public void CrossMonth_20180201_20180301()
        {
            GivenBudgets(new BudgetModel { YearMonth = "201803", Budget = 310 });
            TotalAmountShouldBe(10, "2018-02-01", "2018-03-01");
        }

        [TestMethod]
        public void CrossMonth_20180201_20180302()
        {
            GivenBudgets(new BudgetModel { YearMonth = "201803", Budget = 310 });
            TotalAmountShouldBe(20, "2018-02-01", "2018-03-02");
        }

        [TestMethod]
        public void CrossMonth_20180201_20180401()
        {
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 });

            TotalAmountShouldBe(330, "2018-02-01", "2018-04-01");
        }

        [TestMethod]
        public void CrossMonth_20180201_20180615()
        {
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 },
                new BudgetModel { YearMonth = "201806", Budget = 1200 });

            TotalAmountShouldBe(1510, "2018-02-01", "2018-06-15");
        }

        [TestMethod]
        public void CrossMonth_20180201_20180701()
        {
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 },
                new BudgetModel { YearMonth = "201806", Budget = 1200 });

            TotalAmountShouldBe(2110, "2018-02-01", "2018-07-01");
        }

        [TestMethod]
        public void CrossMonth_20180331_20180601()
        {
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 },
                new BudgetModel { YearMonth = "201806", Budget = 1200 });

            TotalAmountShouldBe(650, "2018-03-31", "2018-06-01");
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
            GivenBudgets(
                new BudgetModel { YearMonth = "201803", Budget = 310 },
                new BudgetModel { YearMonth = "201804", Budget = 600 });

            TotalAmountShouldBe(600, "2018-04-01", "2018-04-30");
        }

        [TestMethod]
        public void OutOfRange()
        {
            GivenBudgets(new BudgetModel { YearMonth = "201803", Budget = 310 });
            TotalAmountShouldBe(0, "2018-01-01", "2018-01-02");
        }

        [TestMethod]
        public void TwoDaysInAMonth()
        {
            GivenBudgets(new BudgetModel { YearMonth = "201804", Budget = 600 });
            TotalAmountShouldBe(40, "2018-04-01", "2018-04-02");
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