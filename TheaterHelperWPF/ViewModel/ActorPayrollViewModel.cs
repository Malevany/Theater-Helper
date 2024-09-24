using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class ActorPayrollViewModel : ViewModelBase
    {
        private Actor actor;
        private double actorPayroll;

        public ActorPayrollViewModel(Actor actor, double actorPayroll)
        {
            this.actor = actor;
            this.actorPayroll = actorPayroll;
        }
        public string FIO
        {
            get => $"{actor.Surname} {actor.Name[0]}.{actor.Patronymic[0]}";
        }
        public double Payroll
        {
            get => actorPayroll;
        }
    }
}
