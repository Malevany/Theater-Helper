using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.BusinessLogic.PayrollStrategies;
using TheaterHelper.BusinessLogic.PremiumDecorator;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class ActorsViewModel : ViewModelBase
    {
        private MainWindowViewModel mwVm;
        private IDataAccess dataBaseSQL = new DataBaseSQLAccess();
        private Theater theater = new Theater();

        private List<Actor> actors;
        private ObservableCollection<ActorShortViewModel> actorsShortVM;
        private ActorShortViewModel selectedActor;
        private ViewModelBase currentActorsViewModel;

        private DateTime selectedPayrollDate = new DateTime(2023, 1, 1, 00, 00, 00);
        private DateTime selectedPremiumDate = new DateTime(2023, 1, 1, 00, 00, 00);

        private ObservableCollection<ActorPayrollViewModel> actorsPayroll;
        private Dictionary<string, IPayrollStrategy> payrollStrategies = new Dictionary<string, IPayrollStrategy>();
        private ObservableCollection<string> payrollStrategiesNames = new ObservableCollection<string>();
        private IPayrollStrategy selectedPayrollStrategy;

        private ObservableCollection<ActorPremiumViewModel> actorsPremium;
        private Dictionary<string, int> premiumCriteria = new Dictionary<string, int>();
        private ObservableCollection<string> premiumCriteriaNames = new ObservableCollection<string>();
        private bool firstPremiumCriteria;
        private bool secondPremiumCriteria;
        private bool thirdPremiumCriteria;
        private int[] selectedPremiumCriteria = new int[3];


        public ActorsViewModel(MainWindowViewModel mwVm)
        {
            this.mwVm = mwVm;
            InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            var actorsTask = dataBaseSQL.GetActors();
            actors = await actorsTask;
            IEnumerable<ActorShortViewModel> actorsVMs =
                from actor in actors
                select new ActorShortViewModel(actor);
            actorsShortVM = new ObservableCollection<ActorShortViewModel>(actorsVMs);
            OnPropertyChanged(nameof(Actors));

            var basePS = new BasePayrollStrategy();
            var extendedPS = new ExtendedPayrollStrategy();
            payrollStrategies.Add("На основе участия в постановках за месяц", basePS);
            payrollStrategies.Add("На основе участия в постановках за месяц с учетом звания артиста", extendedPS);
            OnPropertyChanged(nameof(PayrollStrategies));

            premiumCriteria.Add("", 0);
            premiumCriteria.Add("Среднее значение успеха постановок с участием артиста", 1);
            premiumCriteria.Add("Количество постановок с участием этого артиста", 2);
            premiumCriteria.Add("Звание артиста", 3);
            OnPropertyChanged(nameof(PremiumCriteria));
        }

        public ObservableCollection<ActorShortViewModel> Actors
        {
            get => actorsShortVM;
        }
        public ActorShortViewModel SelectedActor
        {
            get => selectedActor;
            set
            {
                if (selectedActor == value) return;
                selectedActor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        public bool IsVisible => IsActorSelected();
        public ViewModelBase CurrentActorsViewModel
        {
            get => currentActorsViewModel;
            set
            {
                currentActorsViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddActor
        {
            get
            {
                return new RelayCommand(
                    (_) => AddActorImpl()
                    );
            }
        }
        private void AddActorImpl()
        {
            int actorId = -1;
            Actor newActor = new Actor(actorId, "", "", "", 0, "");
            mwVm.CurrentViewModel = new ActorViewModel(newActor, mwVm);
        }
        public ICommand ShowActor
        {
            get
            {
                return new RelayCommand(
                    (_) => ShowActorImpl(),
                    (_) => IsActorSelected()
                    );
            }
        }
        private void ShowActorImpl()
        {
            Actor actor = actors.First(x => x.Id == selectedActor.Id);
            mwVm.CurrentViewModel = new ActorViewModel(actor, mwVm);
        }

        private bool IsActorSelected()
        {
            return selectedActor != null;
        }


        public ObservableCollection<ActorPayrollViewModel> ActorsPayroll
        {
            get => actorsPayroll;
        }
        public DateTime SelectedPayrollDate
        {
            get => selectedPayrollDate;
            set
            {
                if (selectedPayrollDate == value) return;
                selectedPayrollDate = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> PayrollStrategies
        {
            get => new ObservableCollection<string>(payrollStrategies.Keys);
        }
        public string SelectedPayrollStrategy
        {
            set
            {
                if (selectedPayrollStrategy == payrollStrategies[value]) return;
                selectedPayrollStrategy = payrollStrategies[value];
                OnPropertyChanged(nameof(SelectedPayrollStrategy));
            }
        }

        public ICommand CalculateActorsPayroll
        {
            get
            {
                return new RelayCommand(
                    async (_) => await CalculateActorsPayrollImpl(),
                    (_) => IsPayrollStrategySelected()
                    );
            }
        }
        private async Task CalculateActorsPayrollImpl()
        {
            theater.GetSelectedPayrollStrategy(selectedPayrollStrategy);
            var timetable = await dataBaseSQL.GetTimetable(selectedPayrollDate);
            theater.CalculatePayrollActors($"{selectedPayrollDate:Y}", actors, timetable);
            IEnumerable<ActorPayrollViewModel> actorsPayrollVMs =
                from actorPayroll in theater.ActorsPayroll
                select new ActorPayrollViewModel(actorPayroll.Key, actorPayroll.Value);
            actorsPayroll = new ObservableCollection<ActorPayrollViewModel>(actorsPayrollVMs);
            OnPropertyChanged(nameof(ActorsPayroll));
        }
        private bool IsPayrollStrategySelected()
        {
            return selectedPayrollStrategy != null;
        }

        public ObservableCollection<ActorPremiumViewModel> ActorsPremium
        {
            get => actorsPremium;
        }
        public ObservableCollection<string> PremiumCriteria
        {
            get => new ObservableCollection<string>(premiumCriteria.Keys);
        }
        public DateTime SelectedPremiumDate
        {
            get => selectedPremiumDate;
            set
            {
                if (selectedPremiumDate == value) return;
                selectedPremiumDate = value;
                OnPropertyChanged();
            }
        }
        public string FirstSelectedPremiumCriteria
        {
            set
            {
                if (selectedPremiumCriteria[0] == premiumCriteria[value]) return;
                selectedPremiumCriteria[0] = premiumCriteria[value];
                OnPropertyChanged(nameof(FirstSelectedPremiumCriteria));
            }
        }
        public bool FirstCriteriaChecked
        {
            get => firstPremiumCriteria;
            set
            {
                if (firstPremiumCriteria == value) return;
                firstPremiumCriteria = value;
                OnPropertyChanged(nameof(FirstCriteriaChecked));
            }
        }
        public string SecondSelectedPremiumCriteria
        {
            set
            {
                if (selectedPremiumCriteria[1] == premiumCriteria[value]) return;
                selectedPremiumCriteria[1] = premiumCriteria[value];
                OnPropertyChanged(nameof(SecondSelectedPremiumCriteria));
            }
        }
        public bool SecondCriteriaChecked
        {
            get => secondPremiumCriteria;
            set
            {
                if (secondPremiumCriteria == value) return;
                secondPremiumCriteria = value;
                OnPropertyChanged(nameof(SecondCriteriaChecked));
            }
        }
        public string ThirdSelectedPremiumCriteria
        {
            set
            {
                if (selectedPremiumCriteria[2] == premiumCriteria[value]) return;
                selectedPremiumCriteria[2] = premiumCriteria[value];
                OnPropertyChanged(nameof(ThirdSelectedPremiumCriteria));
            }
        }
        public bool ThirdCriteriaChecked
        {
            get => thirdPremiumCriteria;
            set
            {
                if (thirdPremiumCriteria == value) return;
                thirdPremiumCriteria = value;
                OnPropertyChanged(nameof(ThirdCriteriaChecked));
            }
        }

        public ICommand CalculateActorsPremium
        {
            get
            {
                return new RelayCommand(
                    (_) => CalculateActorsPremiumImpl(),
                    (_) => IsAnyPremiumCriteriaSelected()
                    );
            }
        }
        private async void CalculateActorsPremiumImpl()
        {
            var premiumCalculator = new PremiumCalculator();
            if (firstPremiumCriteria) premiumCalculator = new SuccessRateDecorator(premiumCalculator);
            if (secondPremiumCriteria) premiumCalculator = new AppearancesDecorator(premiumCalculator);
            if (thirdPremiumCriteria) premiumCalculator = new TitleDecorator(premiumCalculator);
            theater.GetSelectedPremiumCalculator(premiumCalculator);
            var timetable = await dataBaseSQL.GetTimetable(selectedPremiumDate);
            theater.CalculatePremiumActors($"{selectedPremiumDate:Y}", actors, timetable);
            IEnumerable<ActorPremiumViewModel> actorsPremiumVMs =
                from actorPremium in theater.ActorsPremium
                select new ActorPremiumViewModel(actorPremium.Key, actorPremium.Value);
            actorsPremium = new ObservableCollection<ActorPremiumViewModel>(actorsPremiumVMs);
            OnPropertyChanged(nameof(ActorsPremium));
        }
        private bool IsAnyPremiumCriteriaSelected()
        {
            return firstPremiumCriteria || secondPremiumCriteria || thirdPremiumCriteria;
        }
    }
}