using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.SessionState
{
    public class UpcomingState : ISessionState
    {
        public void Handle(Session currentSession)
        {
            if (currentSession.SessionDate.Date < DateTime.UtcNow) 
            {
                if(currentSession.NumberSoldTickets > 0)
                {
                    currentSession.SetState(new PastState());
                }
                else
                {
                    currentSession.SetState(new CanceledState());
                }
            }
        }
        public string GetStatus()
        {
            return "Предстоит";
        }
    }
}
