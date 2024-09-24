using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;
using TheaterHelper.BusinessLogic.SessionState;

namespace TheaterHelper.BusinessLogic
{
    public class Session
    {
        private int id;
        private DateTime sessionDate;
        private int numberSoldTickets;
        private ISessionState currentState;

        public int Id { get { return id; } }
        public DateTime SessionDate { get { return sessionDate; } }
        public int NumberSoldTickets { get { return numberSoldTickets; } }

        public Session(int sessionId, DateTime sessionDate, int numberSoldTickets)
        {
            this.id = sessionId;
            this.sessionDate = sessionDate;
            this.numberSoldTickets = numberSoldTickets;
            currentState = new UpcomingState();
        }
        public void SetState(ISessionState newState)
        {
            currentState = newState;
        }

        public void UpdateState()
        {
            currentState.Handle(this);
        }

        public string GetStatus()
        {
            return currentState.GetStatus();
        }
    }
}
