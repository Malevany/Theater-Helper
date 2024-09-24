using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic.PremiumDecorator
{
    public abstract class PremiumCalculatorDecorator : PremiumCalculator
    {
        protected readonly PremiumCalculator decoratee;

        public PremiumCalculatorDecorator(PremiumCalculator decoratee)
        {
            this.decoratee = decoratee;
        }
    }
}
