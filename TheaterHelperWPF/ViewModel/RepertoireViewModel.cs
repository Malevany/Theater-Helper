using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class RepertoireViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mwVm;
        private readonly IDataAccess dataBaseSQL = new DataBaseSQLAccess();

        private Repertoire repertoire;
        private ObservableCollection<PlayShortViewModel> playsShortVM;
        private PlayShortViewModel selectedPlay;

        private List<Troupe> troupes;
        private ObservableCollection<TroupeShortViewModel> troupesShortVM;
        private TroupeShortViewModel selectedTroupe;

        public RepertoireViewModel(MainWindowViewModel mwVm)
        {
            this.mwVm = mwVm;
            InitializeAsync();
        }
        public async Task InitializeAsync()
        {
            var repertoireTask = dataBaseSQL.GetRepertoire();
            repertoire = await repertoireTask;
            IEnumerable<PlayShortViewModel> playsVMs =
                from play in repertoire.Plays
                select new PlayShortViewModel(play);
            playsShortVM = new ObservableCollection<PlayShortViewModel>(playsVMs);
            OnPropertyChanged(nameof(Plays));
        }
        private async Task InitializeTroupesAsync()
        {
            var troupesTask = dataBaseSQL.GetTroupesForPlay(selectedPlay.Name);
            troupes = await troupesTask;
            IEnumerable<TroupeShortViewModel> troupesVMs =
                from troupe in troupes
                select new TroupeShortViewModel(troupe);
            troupesShortVM = new ObservableCollection<TroupeShortViewModel>(troupesVMs);
            OnPropertyChanged(nameof(TroupesForPlay));
            OnPropertyChanged();
        }
        public ObservableCollection<PlayShortViewModel> Plays
        {
            get => playsShortVM;
        }

        public PlayShortViewModel SelectedPlay
        {
            get => selectedPlay;
            set
            {
                if (selectedPlay == value) return;
                selectedPlay = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPlayButtonsVisible));
                InitializeTroupesAsync();
            }
        }
        public bool IsPlayButtonsVisible => IsPlaySelected();

        public ICommand AddPlay
        {
            get
            {
                return new RelayCommand(
                    (_) => AddPlayImpl()
                    );
            }
        }
        private void AddPlayImpl()
        {
            Play play = new Play(-1, "", new TimeOnly());
            mwVm.CurrentViewModel = new PlayViewModel(play, mwVm);
        }

        public ICommand ShowPlay
        {
            get
            {
                return new RelayCommand(
                    (_) => ShowPlayImpl(),
                    (_) => IsPlaySelected()
                    );
            }
        }
        private void ShowPlayImpl()
        {
            Play play = repertoire.Plays.First(p => p.Id == selectedPlay.Id);
            mwVm.CurrentViewModel = new PlayViewModel(play, mwVm);
        }
        private bool IsPlaySelected()
        {
            return selectedPlay != null;
        }

        public ObservableCollection<TroupeShortViewModel> TroupesForPlay
        {
            get => troupesShortVM;
        }
        public TroupeShortViewModel SelectedTroupe
        {
            get => selectedTroupe;
            set
            {
                if (selectedTroupe == value) return;
                selectedTroupe = value;
                OnPropertyChanged(nameof(IsTroupeButtonsVisible));
                OnPropertyChanged();
            }
        }
        public bool IsTroupeButtonsVisible => IsTroupeSelected();

        public ICommand ShowTroupe
        {
            get
            {
                return new RelayCommand(
                    (_) => ShowTroupeImpl(),
                    (_) => IsTroupeSelected()
                    );
            }
        }
        private void ShowTroupeImpl()
        {
            Troupe troupe = troupes.First(t => t.Id == selectedTroupe.Id);
            Play play = repertoire.Plays.First(p => p.Id == selectedPlay.Id);
            mwVm.CurrentViewModel = new TroupeDetailsViewModel(mwVm, troupe, play);
        }
        private bool IsTroupeSelected()
        {
            return selectedTroupe != null;
        }
        public ICommand AddTroupe
        {
            get
            {
                return new RelayCommand(
                    (_) => AddTroupeImpl()
                    );
            }
        }
        private void AddTroupeImpl()
        {
            Troupe troupe = new Troupe(-1, null);
            Play play = new Play(-1, "", new TimeOnly(0));
            mwVm.CurrentViewModel = new TroupeDetailsViewModel(mwVm, troupe, play);
        }
    }
}
