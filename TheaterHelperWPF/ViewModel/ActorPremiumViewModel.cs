using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class ActorPremiumViewModel : ViewModelBase
    {
        private Actor actor;
        private double actorPremium;

        public ActorPremiumViewModel(Actor actor, double actorPayroll)
        {
            this.actor = actor;
            actorPremium = actorPayroll;
        }
        public string FIO
        {
            get => $"{actor.Surname} {actor.Name[0]}.{actor.Patronymic[0]}";
        }
        public double Premium
        {
            get => actorPremium;
        }
    }
}
