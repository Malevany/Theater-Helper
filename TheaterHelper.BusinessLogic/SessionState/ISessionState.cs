using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.SessionState
{
    public interface ISessionState
    {
        void Handle(Session currentSession);
        string GetStatus();
    }
}
