using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PayrollStrategies
{
    public class BasePayrollStrategy : IPayrollStrategy //На основе участия в постановках за месяц
    {
        public Dictionary<Actor, double> CalculatePayrollActors(Dictionary<Actor, int> actorAppearances)
        {
            Dictionary<Actor, double> actorsSalary = new Dictionary<Actor, double>();
            foreach (var actor in actorAppearances.Keys)
            {
                double actorSalary = actor.Salary * actorAppearances[actor];
                actorsSalary.Add(actor, Math.Round(actorSalary, 2));
            }
            return actorsSalary;
        }
    }
}
