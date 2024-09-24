using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class TroupeShortViewModel
    {
        private Troupe troupe;

        public TroupeShortViewModel(Troupe troupe)
        {
            this.troupe = troupe;
        }

        public int Id => troupe.Id;
        public string TroupeNumber => $"Труппа под номером {troupe.Id}";
    }
}
