using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheaterHelperWPF.ViewModel
{
    public class TroupeDetailsInstructionViewModel : ViewModelBase
    {
        TroupeDetailsViewModel tdVm;
        public TroupeDetailsInstructionViewModel(TroupeDetailsViewModel tdVm) 
        {
            this.tdVm = tdVm;
        }
        public ICommand CloseInstruction
        {
            get => new RelayCommand(
                (_) => CloseInstructionImpl()
                );
        }
        private void CloseInstructionImpl()
        {
            tdVm.CurrentTroupeDetailsViewModel = null;
        }
    }
}
