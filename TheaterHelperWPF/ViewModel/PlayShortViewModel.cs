using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class PlayShortViewModel : ViewModelBase
    {
        private Play play;

        public PlayShortViewModel(Play play)
        {
            this.play = play;
        }
        public string Name
        {
            get => play.Name;
        }
        public int Id
        {
            get => play.Id;
        }
    }
}
