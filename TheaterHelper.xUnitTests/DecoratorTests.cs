using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic.PremiumDecorator;
using TheaterHelper.BusinessLogic;
using System.Numerics;

namespace TheaterHelper.xUnitTests
{
    public class DecoratorTests
    {
        [Fact]
        public void Appearances_decorator_calculate_premium_should_apply_appearances_multiplier()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new AppearancesDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Заслуженный Артист РФ");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1500, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.8, 10),
                new ActorStatistic(actor2, 0.9, 8)
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 10, premiums[actor1]);
            Assert.Equal(5000.0 * 8, premiums[actor2]); 
        }

        [Fact]
        public void Success_rate_decorator_calculate_premium_should_apply_success_rate_multiplier()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new SuccessRateDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Заслуженный Артист РФ");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1500, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.8, 10),
                new ActorStatistic(actor2, 0.9, 8)
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 0.8, premiums[actor1]);
            Assert.Equal(5000.0 * 0.9, premiums[actor2]);
        }


        [Fact]
        public void Title_decorator_calculate_premium_should_apply_title_coefficient()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new TitleDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Старший артист");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1000, "Заслуженный Артист РФ");
            var actor3 = new Actor(3, "Андреев", "Андрей", "Андреевич", 1000, "Заслуженный деятель искусств РФ");
            var actor4 = new Actor(4, "Сидоров", "Сидр", "Сидорович", 1000, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 1.0, 5),
                new ActorStatistic(actor2, 1.0, 4),
                new ActorStatistic(actor3, 1.0, 3),
                new ActorStatistic(actor4, 1.0, 2),
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0, premiums[actor1]);
            Assert.Equal(5000.0 * 1.5, premiums[actor2]);
            Assert.Equal(5000.0 * 1.75, premiums[actor3]);
            Assert.Equal(5000.0 * 2.0, premiums[actor4]);
        }

        [Fact]
        public void Appearances_success_rate_decorator_calculate_premium_should_apply_appearances_and_success_rate_multiplier()
        {
            //Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new AppearancesDecorator(premiumCalculator);
            premiumCalculator = new SuccessRateDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Заслуженный Артист РФ");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1500, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.8, 10),
                new ActorStatistic(actor2, 0.9, 8)
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 10 * 0.8, premiums[actor1]);
            Assert.Equal(5000.0 * 8  * 0.9, premiums[actor2]); 
        }

        [Fact]
        public void Success_rate_title_decorator_calculate_premium_should_apply_success_rate_and_title_multiplier()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new SuccessRateDecorator(premiumCalculator);
            premiumCalculator = new TitleDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Старший артист");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1000, "Заслуженный Артист РФ");
            var actor3 = new Actor(3, "Андреев", "Андрей", "Андреевич", 1000, "Заслуженный деятель искусств РФ");
            var actor4 = new Actor(4, "Сидоров", "Сидр", "Сидорович", 1000, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.25, 5),
                new ActorStatistic(actor2, 0.50, 4),
                new ActorStatistic(actor3, 0.75, 3),
                new ActorStatistic(actor4, 1.00, 2),
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 0.25 * 1.00, premiums[actor1]);
            Assert.Equal(5000.0 * 0.50 * 1.50, premiums[actor2]);
            Assert.Equal(5000.0 * 0.75 * 1.75, premiums[actor3]);
            Assert.Equal(5000.0 * 1.00 * 2.00, premiums[actor4]);
        }

        [Fact]
        public void Appearances_title_decorator_calculate_premium_should_apply_appearances_and_title_multiplier()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new AppearancesDecorator(premiumCalculator);
            premiumCalculator = new TitleDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Старший артист");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1000, "Заслуженный Артист РФ");
            var actor3 = new Actor(3, "Андреев", "Андрей", "Андреевич", 1000, "Заслуженный деятель искусств РФ");
            var actor4 = new Actor(4, "Сидоров", "Сидр", "Сидорович", 1000, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.25, 5),
                new ActorStatistic(actor2, 0.50, 4),
                new ActorStatistic(actor3, 0.75, 3),
                new ActorStatistic(actor4, 1.00, 2),
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 5 * 1.00, premiums[actor1]);
            Assert.Equal(5000.0 * 4 * 1.50, premiums[actor2]);
            Assert.Equal(5000.0 * 3 * 1.75, premiums[actor3]);
            Assert.Equal(5000.0 * 2 * 2.00, premiums[actor4]);
        }
        [Fact]
        public void Appearances_success_rate_title_decorator_calculate_premium_should_apply_appearances_and_success_rate_and_title_multiplier()
        {
            // Arrange
            var premiumCalculator = new PremiumCalculator();
            premiumCalculator = new AppearancesDecorator(premiumCalculator);
            premiumCalculator = new SuccessRateDecorator(premiumCalculator);
            premiumCalculator = new TitleDecorator(premiumCalculator);

            var actor1 = new Actor(1, "Иванов", "Иван", "Иванович", 1000, "Старший артист");
            var actor2 = new Actor(2, "Петров", "Петр", "Петрович", 1000, "Заслуженный Артист РФ");
            var actor3 = new Actor(3, "Андреев", "Андрей", "Андреевич", 1000, "Заслуженный деятель искусств РФ");
            var actor4 = new Actor(4, "Сидоров", "Сидр", "Сидорович", 1000, "Народный артист РФ");

            var actorStatistics = new List<ActorStatistic>
            {
                new ActorStatistic(actor1, 0.25, 5),
                new ActorStatistic(actor2, 0.50, 4),
                new ActorStatistic(actor3, 0.75, 3),
                new ActorStatistic(actor4, 1.00, 2),
            };

            // Act
            var premiums = premiumCalculator.CalculatePremium(actorStatistics);

            // Assert
            Assert.Equal(5000.0 * 5 * 0.25 * 1.00, premiums[actor1]);
            Assert.Equal(5000.0 * 4 * 0.50 * 1.50, premiums[actor2]);
            Assert.Equal(5000.0 * 3 * 0.75 * 1.75, premiums[actor3]);
            Assert.Equal(5000.0 * 2 * 1.00 * 2.00, premiums[actor4]);
        }
    }
}
