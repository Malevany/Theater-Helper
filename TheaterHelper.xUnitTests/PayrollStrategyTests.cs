using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic.PayrollStrategies;
using TheaterHelper.BusinessLogic;
using System.Numerics;

namespace TheaterHelper.xUnitTests
{
    public class PayrollStrategyTests
    {
        [Fact]
        public void BasePayrollStrategy_ShouldCalculateCorrectSalaries()
        {
            // Arrange
            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 2000, "");

            var actorAppearances = new Dictionary<Actor, int>
            {
                { actor1, 5 },
                { actor2, 3 }
            };

            var payrollStrategy = new BasePayrollStrategy();

            // Act
            var salaries = payrollStrategy.CalculatePayrollActors(actorAppearances);

            // Assert
            Assert.Equal(5000, salaries[actor1]);
            Assert.Equal(6000, salaries[actor2]);
        }

        [Fact]
        public void ExtendedPayrollStrategy_ShouldCalculateCorrectSalariesWithTitles()
        {
            // Arrange
            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Старший артист");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1000, "Заслуженный Артист РФ");
            var actor3 = new Actor(3, "Андреев", "Андрей", "Андреевич", 1000, "Заслуженный деятель искусств РФ");
            var actor4 = new Actor(4, "Сидоров", "Сидр", "Сидорович", 1000, "Народный артист РФ");
            

            var actorAppearances = new Dictionary<Actor, int>
            {
                { actor1, 5 },
                { actor2, 4 },
                { actor3, 3 },
                { actor4, 2 }
            };

            var payrollStrategy = new ExtendedPayrollStrategy();

            // Act
            var salaries = payrollStrategy.CalculatePayrollActors(actorAppearances);

            // Assert
            Assert.Equal(5500, salaries[actor1]); // 1000 * 5 * 1.1
            Assert.Equal(6000, salaries[actor2]); // 1000 * 4 * 1.5
            Assert.Equal(5250, salaries[actor3]); // 1000 * 3 * 1.75
            Assert.Equal(4000, salaries[actor4]); // 1000 * 2 * 2
        }
    }
}