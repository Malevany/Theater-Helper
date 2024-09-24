using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PremiumDecorator
{
    public class SuccessRateDecorator : PremiumCalculatorDecorator
    {
        public SuccessRateDecorator(PremiumCalculator decoratee) : base(decoratee) 
        {

        }
        public override Dictionary<Actor, double> CalculatePremium(List<ActorStatistic> actorsStatistics)
        {
            actorsPremium = decoratee.CalculatePremium(actorsStatistics);
            foreach(var actorStat in actorsStatistics)
            {
                actorsPremium[actorStat.Actor] = Math.Round(actorsPremium[actorStat.Actor] * actorStat.ActorSuccessRate, 2);
            }
            return actorsPremium;
        }
    }
}
