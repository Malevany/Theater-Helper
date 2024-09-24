using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PremiumDecorator
{
    public class PremiumCalculator
    {
        protected double basePremium = 5000;
        protected Dictionary<Actor, double> actorsPremium;
        public virtual Dictionary<Actor, double> CalculatePremium(List<ActorStatistic> actorStatistics)
        {
            actorsPremium = actorStatistics.ToDictionary(a => a.Actor, a => basePremium);
            return actorsPremium;
        }
    }
}
