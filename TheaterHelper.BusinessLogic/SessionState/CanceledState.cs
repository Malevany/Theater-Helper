using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.SessionState
{
    public class CanceledState : ISessionState
    {
        public void Handle(Session currentSession)
        {
            
        }
        public string GetStatus()
        {
            return "Отменена";
        }
    }
}
