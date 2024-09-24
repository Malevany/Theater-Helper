using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PremiumDecorator
{
    public class AppearancesDecorator : PremiumCalculatorDecorator
    {
        public AppearancesDecorator(PremiumCalculator decoratee) : base(decoratee)
        {

        }
        public override Dictionary<Actor, double> CalculatePremium(List<ActorStatistic> actorsStatistics)
        {
            actorsPremium = decoratee.CalculatePremium(actorsStatistics);
            foreach (var actorStat in actorsStatistics)
            {
                actorsPremium[actorStat.Actor] = Math.Round(actorsPremium[actorStat.Actor] * actorStat.ActorAppearances, 2) ;
            }
            return actorsPremium;
        }
    }

    public class CopyOfAppearancesDecorator : PremiumCalculatorDecorator
    {
        public CopyOfAppearancesDecorator(PremiumCalculator decoratee) : base(decoratee)
        {

        }
        public override Dictionary<Actor, double> CalculatePremium(List<ActorStatistic> actorsStatistics)
        {
            actorsPremium = decoratee.CalculatePremium(actorsStatistics);
            foreach (var actorStat in actorsStatistics)
            {
                actorsPremium[actorStat.Actor] = Math.Round(actorsPremium[actorStat.Actor] * actorStat.ActorAppearances, 2);
            }
            return actorsPremium;
        }
    }
}
