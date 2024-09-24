using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PayrollStrategies
{
    public interface IPayrollStrategy
    {
        Dictionary<Actor, double> CalculatePayrollActors(Dictionary<Actor, int> actorAppearances);
    }
}
