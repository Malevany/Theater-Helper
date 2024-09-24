using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PremiumDecorator
{
    public class TitleDecorator : PremiumCalculatorDecorator
    {
        public TitleDecorator(PremiumCalculator decoratee) : base(decoratee)
        {

        }
        public override Dictionary<Actor, double> CalculatePremium(List<ActorStatistic> actorsSuccessRate)
        {
            actorsPremium = decoratee.CalculatePremium(actorsSuccessRate);
            foreach (var actor in actorsPremium.Keys)
            {
                double titleCoefficient = 1;
                if (actor.Title == "Заслуженный Артист РФ")
                {
                    titleCoefficient = 1.5;
                }
                else if (actor.Title == "Заслуженный деятель искусств РФ")
                {
                    titleCoefficient = 1.75;
                }
                else if (actor.Title == "Народный артист РФ")
                {
                    titleCoefficient = 2;
                }
                actorsPremium[actor] = Math.Round(actorsPremium[actor] * titleCoefficient, 2);
            }
            return actorsPremium;
        }
    }
}
