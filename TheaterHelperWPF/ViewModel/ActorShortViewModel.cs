using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class ActorShortViewModel : ViewModelBase
    {
        private Actor actor;

        public ActorShortViewModel(Actor actor)
        {
            this.actor = actor;
        }

        public int Id
        {
            get => actor.Id;
        }
        public string FIO
        {
            get => $"{actor.Surname} {actor.Name[0]}.{actor.Patronymic[0]}";
        }
        public string Title
        {
            get => actor.Title;
        }
    }
}
