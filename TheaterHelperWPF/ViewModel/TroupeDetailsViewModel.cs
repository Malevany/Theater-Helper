using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class TroupeDetailsViewModel : ViewModelBase
    {
        private MainWindowViewModel mwVm;
        private IDataAccess dataBaseSQL = new DataBaseSQLAccess();
        private DataBaseSQLSaver dataBaseSaver = new DataBaseSQLSaver();
        private ViewModelBase currentTroupeDetailsViewModel;

        private Troupe troupe;
        private List<Executors> tempTroupeComposition = new List<Executors>();

        private Repertoire repertoire;
        private ObservableCollection<PlayShortViewModel> playsShortVM;
        private bool isPlaysListVisible = false;
        private ObservableCollection<string> playsNames;
        private PlayShortViewModel play;

        private List<Role> playRoles;
        private ObservableCollection<RoleShortViewModel> playRolesShortVM;
        private RoleShortViewModel selectedRole;

        private List<Actor> actors;
        private ObservableCollection<ActorShortViewModel> actorsShortVM;
        private ActorShortViewModel selectedMainActor;
        private ActorShortViewModel selectedDoubler;

        private string headText;

        private bool isPlayNameVisible = false;
        private string playName;

        private bool saveExecutorsMessageVisibility = false;

        private string saveMessage;
        private bool saveMessageVisibility = false;
        private string saveMessageColor;

        private string buttonContent;

        public TroupeDetailsViewModel(MainWindowViewModel mwVm, Troupe troupe, Play play)
        {
            this.mwVm = mwVm;
            this.troupe = troupe;

            this.play = new PlayShortViewModel(play);
            InitializePlaysAsync();
            InitializeActorsAsync();
            if (troupe.Id == -1)
            {
                headText = "Создание труппы";
                isPlaysListVisible = true;
                buttonContent = "Создать";
            }
            else
            {
                tempTroupeComposition = troupe.TroupeComposition;
                headText = $"Информация о труппе под номером {troupe.Id}";
                isPlayNameVisible = true;
                playName = play.Name;
                buttonContent = "Сохранить";
                InitializeRolesAsync();
            }

        }

        private async Task InitializePlaysAsync()
        {

            var repertoireTask = dataBaseSQL.GetRepertoire();
            repertoire = await repertoireTask;
            IEnumerable<PlayShortViewModel> playsVMs =
                from play in repertoire.Plays
                select new PlayShortViewModel(play);
            playsShortVM = new ObservableCollection<PlayShortViewModel>(playsVMs);
            playsNames = new ObservableCollection<string>(playsShortVM.Select(p => p.Name));
            OnPropertyChanged(nameof(Plays));
            OnPropertyChanged();
        }
        private async Task InitializeActorsAsync()
        {
            var actorsTask = dataBaseSQL.GetActors();
            actors = await actorsTask;
            IEnumerable<ActorShortViewModel> actorsVMs =
                from actor in actors
                select new ActorShortViewModel(actor);
            actorsShortVM = new ObservableCollection<ActorShortViewModel>(actorsVMs);
            OnPropertyChanged(nameof(Actors));
            OnPropertyChanged();
        }
        private async Task InitializeRolesAsync()
        {
            var playRolesTask = dataBaseSQL.GetRolesForPlay(play.Name);
            playRoles = await playRolesTask;
            IEnumerable<RoleShortViewModel> playRolesVMs =
                from playRole in playRoles
                select new RoleShortViewModel(playRole);
            playRolesShortVM = new ObservableCollection<RoleShortViewModel>(playRolesVMs);
            OnPropertyChanged(nameof(Roles));
            OnPropertyChanged();
        }

        public string HeadText => headText;
        
        public ViewModelBase CurrentTroupeDetailsViewModel
        {
            get => currentTroupeDetailsViewModel;
            set
            {
                currentTroupeDetailsViewModel = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsPlayNameVisible => isPlayNameVisible;
        public string PlayName => playName;

        public bool IsExecutorsMessageVisible => saveExecutorsMessageVisibility;

        public string SaveMessage => saveMessage;
        public bool IsSaveMessageVisible => saveMessageVisibility;
        public string SaveMessageColor => saveMessageColor;
        
        public string ButtonContent => buttonContent;

        public bool IsPlaysListVisible => isPlaysListVisible;
        public ObservableCollection<string> Plays
        {
            get => playsNames;
        }
        public string SelectedPlay
        {
            get => play.Name;
            set
            {
                if (play.Name == value) return;
                play = playsShortVM.First(p => p.Name == value);
                tempTroupeComposition = new List<Executors>();
                OnPropertyChanged(nameof(SelectedPlay));
                OnPropertyChanged();
                InitializeRolesAsync();
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
                if (selectedRole == value) return;
                selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
                if (troupe.Id != -1)
                {
                    if (IsExecutorsFilled())
                    {
                        selectedMainActor = actorsShortVM.First(a => a.Id == troupe.TroupeComposition.Find(e => e.Role.Id == selectedRole.Id).MainActor.Id);
                        OnPropertyChanged(nameof(SelectedMainActor));
                        selectedDoubler = actorsShortVM.First(a => a.Id == troupe.TroupeComposition.Find(e => e.Role.Id == selectedRole.Id).Doubler.Id);
                        OnPropertyChanged(nameof(SelectedDoubler));
                    }
                    else
                    {
                        selectedMainActor = null;
                        selectedDoubler = null;
                        OnPropertyChanged(nameof(SelectedMainActor));
                        OnPropertyChanged(nameof(SelectedDoubler));
                    }
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActorShortViewModel> Actors
        {
            get => actorsShortVM;
        }
        public ActorShortViewModel SelectedMainActor
        {
            get => selectedMainActor;
            set
            {
                if (selectedMainActor == value) return;
                selectedMainActor = value;
                OnPropertyChanged(nameof(SelectedMainActor));
                OnPropertyChanged();
            }
        }
        public ActorShortViewModel SelectedDoubler
        {
            get => selectedDoubler;

            set
            {
                if (selectedDoubler == value) return;
                selectedDoubler = value;
                OnPropertyChanged(nameof(SelectedDoubler));
                OnPropertyChanged();
            }
        }

        public ICommand SaveExecutors
        {
            get
            {
                return new RelayCommand(
                    (_) => SaveExecutorsImpl(),
                    (_) => IsEveryComponentSelected()
                    );
            }
        }

        private bool IsEveryComponentSelected()
        {
            return IsRoleSelected() & IsMainActorSelected() & IsDoublerSelected();
        }
        private bool IsRoleSelected()
        {
            return selectedRole != null;
        }
        private bool IsMainActorSelected()
        {
            return selectedMainActor != null;
        }
        private bool IsDoublerSelected()
        {
            return selectedDoubler != null;
        }
        
        private async void SaveExecutorsImpl()
        {
            var role = playRoles.First(r => r.Id == selectedRole.Id);
            var mainActor = actors.First(a => a.Id == selectedMainActor.Id);
            var doubler = actors.First(a => a.Id == selectedDoubler.Id);
            if (IsExecutorsFilled())
            {
                var newExecutors = tempTroupeComposition.First(e => e.Role.Id == role.Id);
                newExecutors = new Executors(newExecutors.Id, troupe.Id, role, mainActor, doubler);
            }
            else
            {
                var newExecutors = new Executors(-1, troupe.Id, role, mainActor, doubler);
                tempTroupeComposition.Add(newExecutors);
            }
            saveExecutorsMessageVisibility = true;
            OnPropertyChanged(nameof(IsExecutorsMessageVisible));
            await Task.Delay(3000);
            saveExecutorsMessageVisibility = false;
            OnPropertyChanged(nameof(IsExecutorsMessageVisible));
        }
        private bool IsExecutorsFilled()
        {
            return tempTroupeComposition.FirstOrDefault(e => e.Role.Id == selectedRole.Id) != null;
        }

        public ICommand CompleteAction
        {
            get
            {
                return new RelayCommand(
                    (_) => CompleteActionImpl()
                    );
            }
        }
        private async Task CompleteActionImpl()
        {
            if (IsTroupeComplete())
            {
                troupe = new Troupe(troupe.Id, tempTroupeComposition);
                if (troupe.Id == -1)
                {
                    int newTroupeID = dataBaseSaver.AddNewTroupe(troupe);
                    troupe = new Troupe(newTroupeID, tempTroupeComposition);
                }
                else
                {
                    dataBaseSaver.SaveTroupe(troupe);
                }
                ShowSaveMessage("Данные успешно сохранены!", "Green");
            }
            else
            {
                ShowSaveMessage("Не все роли распределены!", "Red");
            }
            await Task.Delay(5000);
            HideSaveMessage();
        }
        private bool IsTroupeComplete()
        {
            return tempTroupeComposition.Count == playRoles.Count;
        }
        private void ShowSaveMessage(string message, string messageColor)
        {
            saveMessage = message;
            saveMessageColor = messageColor;
            saveMessageVisibility = true;
            OnPropertyChanged(nameof(SaveMessage));
            OnPropertyChanged(nameof(SaveMessageColor));
            OnPropertyChanged(nameof(IsSaveMessageVisible));
        }
        private void HideSaveMessage()
        {
            saveMessage = string.Empty;
            saveMessageColor = string.Empty;
            saveMessageVisibility = false;
            OnPropertyChanged(nameof(SaveMessage));
            OnPropertyChanged(nameof(SaveMessageColor));
            OnPropertyChanged(nameof(IsSaveMessageVisible));
        }

        public ICommand ToRepertoire
        {
            get
            {
                return new RelayCommand(
                    (_) => ToRepertoireImpl()
                    );
            }
        }
        private void ToRepertoireImpl()
        {
            mwVm.CurrentViewModel = new RepertoireViewModel(mwVm);
        }

        public ICommand OpenInstruction
        {
            get 
            {
                return new RelayCommand(
                    (_) => OpenInstructionImpl()
                    );
            }
        }
        private void OpenInstructionImpl()
        {
            currentTroupeDetailsViewModel = new TroupeDetailsInstructionViewModel(this);
            OnPropertyChanged(nameof(CurrentTroupeDetailsViewModel));
        }
        
    }
}
