using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class PlayViewModel : ViewModelBase
    {
        private Play play;
        private List<Role> playRoles;
        private ObservableCollection<RoleShortViewModel> playRolesShortVM;
        private RoleShortViewModel selectedRole;
        private Role newRole;
        private string roleName = "";
        private string roleImportance;
        private ObservableCollection<string> roleImportances = new ObservableCollection<string>();
        private MainWindowViewModel mwVm;
        private Play updatedPlay;
        private IDataAccess dataBaseSQL = new DataBaseSQLAccess();
        private DataBaseSQLSaver dbSaver = new DataBaseSQLSaver();

        private string playName;
        private TimeOnly playDuration;

        private string message;
        private bool messageVisibility;
        private string messageColor;

        private bool rolesVisibility = true;
        private string roleMessage;
        private bool roleMessageVisibility = false;
        private string roleMessageColor;

        private string buttonContent;

        public PlayViewModel(Play play, MainWindowViewModel mwVm)
        {
            this.play = play;
            this.mwVm = mwVm;

            if (play.Id == -1)
            {
                buttonContent = "Добавить постановку";
                rolesVisibility = false;
            }
            else buttonContent = "Сохранить изменения";

            playName = play.Name;
            playDuration = play.Duration;
            roleImportances.Add("Главная");
            roleImportances.Add("Второстепенная");
            roleImportances.Add("Эпизодическая");
            roleImportances.Add("Фоновая");
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var playRolesTask = dataBaseSQL.GetRolesForPlay(playName);
            playRoles = await playRolesTask;
            IEnumerable<RoleShortViewModel> playRolesVMs =
                from playRole in playRoles
                select new RoleShortViewModel(playRole);
            playRolesShortVM = new ObservableCollection<RoleShortViewModel>(playRolesVMs);
            OnPropertyChanged(nameof(Roles));
            if (newRole != null)
            {
                selectedRole = playRolesShortVM.First(r => r.Id == newRole.Id);
                OnPropertyChanged(nameof(SelectedRole));
            }
            OnPropertyChanged();
        }

        public string PlayName
        {
            get => playName;
            set
            {
                if (playName == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    playName = value;
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Название постановки'", "Red");
                OnPropertyChanged();
            }
        }
        public string PlayDuration
        {
            get => playDuration.ToShortTimeString();
            set
            {

                if (playDuration.ToShortTimeString() == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputTimeOnlyCorrect(value))
                {
                    playDuration = TimeOnly.Parse(value);
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Длительность постановки'", "Red");
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RoleShortViewModel> Roles
        {
            get => playRolesShortVM;
        }


        public RoleShortViewModel SelectedRole
        {
            get => selectedRole;
            set
            {
                if (selectedRole == value || value == null) return;
                selectedRole = value;
                roleName = value.Name;
                roleImportance = value.Importance;
                OnPropertyChanged(nameof(RoleName));
                OnPropertyChanged(nameof(RoleImportance));
            }
        }
        public ObservableCollection<string> RoleImportances
        {
            get => roleImportances;
        }
        public string RoleName
        {
            get => roleName;
            set
            {
                if (roleName == value)
                {
                    HideRoleMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    roleName = value;
                    HideRoleMessage();
                }
                else ShowRoleMessage("Неккоректно заполнено поле 'Роль'", "Red");
                OnPropertyChanged();
            }
        }
        public string RoleImportance
        {
            get => roleImportance;
            set
            {
                if (roleImportance == value)
                {
                    HideRoleMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    roleImportance = value;
                    HideRoleMessage();
                }
                else ShowRoleMessage("Неккоректно заполнено поле 'Важность роли'", "Red");
                OnPropertyChanged();
            }
        }

        public string Message => message;
        public bool IsMessageVisible => messageVisibility;
        public string MessageColor => messageColor;

        public bool IsRolesVisible => rolesVisibility;
        public string RoleMessage => roleMessage;
        public bool IsRoleMessageVisible => roleMessageVisibility;
        public string RoleMessageColor => roleMessageColor;

        public string ButtonContent
        {
            get => buttonContent;
        }

        private bool IsRoleSelected()
        {
            return selectedRole != null;
        }

        private void ShowMessage(string message, string messageColor)
        {
            this.messageColor = messageColor;
            this.message = message;
            messageVisibility = true;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(IsMessageVisible));
            OnPropertyChanged(nameof(MessageColor));
        }
        private void HideMessage()
        {
            messageVisibility = false;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(IsMessageVisible));
        }

        private void ShowRoleMessage(string message, string messageColor)
        {
            this.roleMessageColor = messageColor;
            this.roleMessage = message;
            roleMessageVisibility = true;
            OnPropertyChanged(nameof(RoleMessage));
            OnPropertyChanged(nameof(IsRoleMessageVisible));
            OnPropertyChanged(nameof(RoleMessageColor));
        }
        private void HideRoleMessage()
        {
            roleMessageVisibility = false;
            OnPropertyChanged(nameof(RoleMessage));
            OnPropertyChanged(nameof(IsRoleMessageVisible));
            OnPropertyChanged(nameof(RoleMessageColor));
        }

        public ICommand AddRole
        {
            get
            {
                return new RelayCommand(
                    (_) => AddRoleImpl()
                    );
            }
        }
        private void AddRoleImpl()
        {
            selectedRole = null;
            roleName = null;
            roleImportance = null;
            OnPropertyChanged(nameof(SelectedRole));
            OnPropertyChanged(nameof(RoleName));
            OnPropertyChanged(nameof(RoleImportance));
        }

        public ICommand SaveRole
            => new RelayCommand((_) => SaveUpdatedRoleImpl());
        private async void SaveUpdatedRoleImpl()
        {
            if (IsInputStringCorrect(roleName) && IsRoleImportanceSelected())
            {
                if (IsRoleSelected())
                {
                    newRole = new Role(selectedRole.Id, playName, roleName, roleImportance);
                    dbSaver.UpdateRole(newRole);
                }
                else
                {
                    newRole = new Role(-1, playName, roleName, roleImportance);
                    int newRoleId = dbSaver.AddNewRole(newRole);
                    newRole = new Role(newRoleId, playName, roleName, roleImportance);
                }
                ShowRoleMessage("Данные успешно сохранены!", "Green");
                InitializeAsync();
            }
            else
            {
                ShowRoleMessage("Не все данные заполнены!", "Red");
            }
            await Task.Delay(5000);
            HideRoleMessage();
        }
        private bool IsRoleImportanceSelected()
        {
            return roleImportance != null;
        }

        public ICommand CompletAction
            => new RelayCommand((_) => CompletActionImpl());
        private void CompletActionImpl()
        {
            if (IsInputStringCorrect(playName) && IsInputTimeOnlyCorrect(playDuration.ToShortTimeString()))
            {
                updatedPlay = new Play(play.Id, playName, playDuration);
                if(updatedPlay.Id == -1)
                {
                    int newPlayID = dbSaver.AddNewPlay(updatedPlay);
                    play = new Play(newPlayID, playName, playDuration);
                }
                else
                {
                    dbSaver.UpdatePlay(updatedPlay);
                }
                rolesVisibility = true;
                OnPropertyChanged(nameof(IsRolesVisible)); 
                ShowMessage("Данные успешно сохранены!", "Green");

            }
            else ShowMessage("Не все поля заполнены корректно!", "Red");
            OnPropertyChanged();
        }

        public ICommand ToPlays
        {
            get
            {
                return new RelayCommand(
                    (_) => ToPlaysImpl()
                    );
            }
        }
        private bool IsInputStringCorrect(string input)
        {
            return Regex.IsMatch(input, @"^[а-яА-Я0-9 ]+$") && input != null;
        }
        private bool IsInputTimeOnlyCorrect(string inputTimeOnly)
        {
            return TimeOnly.TryParse(inputTimeOnly, out _);
        }

        private void ToPlaysImpl()
        {
            mwVm.CurrentViewModel = new RepertoireViewModel(mwVm);
        }
    }
}
