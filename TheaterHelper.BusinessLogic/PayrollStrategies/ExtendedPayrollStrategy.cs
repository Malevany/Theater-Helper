using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PayrollStrategies
{
    public class ExtendedPayrollStrategy : IPayrollStrategy //На основе участия в постановках за месяц с учетом звания артиста
    {
        public Dictionary<Actor, double> CalculatePayrollActors(Dictionary<Actor, int> actorAppearances)
        {
            Dictionary<Actor, double> actorsSalary = new Dictionary<Actor, double>();
            foreach (var actor in actorAppearances.Keys)
            {
                double titleCoefficient = 1; 
                if (actor.Title == "Старший артист")
                {
                    titleCoefficient = 1.1;
                }
                else if (actor.Title == "Заслуженный Артист РФ")
                {
                    titleCoefficient = 1.5;
                }
                else if(actor.Title == "Заслуженный деятель искусств РФ")
                {
                    titleCoefficient = 1.75;
                }
                else if(actor.Title == "Народный артист РФ")
                {
                    titleCoefficient = 2;
                }
                double actorSalary = actor.Salary * actorAppearances[actor] * titleCoefficient;
                actorsSalary.Add(actor, Math.Round(actorSalary, 2));
            }
            return actorsSalary;
        }
    }
}
