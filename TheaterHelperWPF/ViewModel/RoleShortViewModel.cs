using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelperWPF.ViewModel
{
    public class RoleShortViewModel : ViewModelBase
    {
        private Role role;

        public RoleShortViewModel(Role role)
        {
            this.role = role;
        }

        public int Id
        {
            get => role.Id;
        }
        public string Name
        {
            get => role.Name;
        }
        public string Importance
        {
            get => role.RoleImportance;
        }
    }
}
