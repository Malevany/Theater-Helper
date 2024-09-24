using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelper.xUnitTests
{
    public class SessionTests
    {
        [Fact]
        public void SessionWhenCreatedShouldBeInUpcomingState()
        {
            // Arrange
            var session = new Session(1, DateTime.UtcNow.AddDays(1), 0);

            // Act
            var status = session.GetStatus();

            // Assert
            Assert.Equal("Предстоит", status);
        }

        [Fact]
        public void SessionWhenDateIsPastAndTicketsSoldShouldTransitionToPastState()
        {
            // Arrange
            var session = new Session(1, DateTime.UtcNow.AddDays(-1), 10);

            // Act
            session.UpdateState();
            var status = session.GetStatus();

            // Assert
            Assert.Equal("Проведена", status);
        }

        [Fact]
        public void SessionWhenDateIsPastAndNoTicketsSoldShouldTransitionToCanceledState()
        {
            // Arrange
            var session = new Session(1, DateTime.UtcNow.AddDays(-1), 0);

            // Act
            session.UpdateState();
            var status = session.GetStatus();

            // Assert
            Assert.Equal("Отменена", status);
        }
    }
}
