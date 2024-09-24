using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class SubmissionViewModel : ViewModelBase
    {
        private MainWindowViewModel mwVm;
        private IDataAccess dataBaseSQL = new DataBaseSQLAccess();
        private DataBaseSQLSaver dataBaseSQLSaver = new DataBaseSQLSaver();
        private ScheduledSubmission scheduledSubmission;

        private DateTime selectedDate = DateTime.UtcNow.Date;
        private int numberOfSoldTickets = 0;

        private Repertoire repertoire;
        private List<Play> plays;
        private ObservableCollection<PlayShortViewModel> playsShortVM;
        private PlayShortViewModel selectedPlay;

        private List<Troupe> troupes;
        private ObservableCollection<TroupeShortViewModel> troupesShortVM;
        private TroupeShortViewModel selectedTroupe;

        private string headText;
        private string message;
        private bool messageVisibility;
        private string messageColor;
        private string buttonContent;

        public SubmissionViewModel(MainWindowViewModel mwVm, ScheduledSubmission scheduledSubmission)
        {
            this.mwVm = mwVm;
            this.scheduledSubmission = scheduledSubmission;
            
            InitializePlaysAsync();
            if (scheduledSubmission.Id == -1)
            {
                headText = "Добавление выступления";
                buttonContent = "Добавить";
            }
            else
            {
                selectedDate = scheduledSubmission.Session.SessionDate;
                numberOfSoldTickets = scheduledSubmission.Session.NumberSoldTickets;
                headText = $"Информация о выступлении от {selectedDate:D}";
                buttonContent = "Сохранить";

            }
        }
        private async Task InitializePlaysAsync()
        {
            var repertoireTask = dataBaseSQL.GetRepertoire();
            repertoire = await repertoireTask;
            plays = repertoire.Plays;
            IEnumerable<PlayShortViewModel> playsVMs =
                from play in plays
                select new PlayShortViewModel(play);
            playsShortVM = new ObservableCollection<PlayShortViewModel>(playsVMs);
            OnPropertyChanged(nameof(Plays));
            OnPropertyChanged();
            if (scheduledSubmission.Id != -1)
            {
                selectedPlay = playsShortVM.First(p => p.Id == scheduledSubmission.Submission.Play.Id);
                OnPropertyChanged(nameof(SelectedPlay));
                OnPropertyChanged();
                InitializeTroupesAsync();
            }
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
            if (scheduledSubmission.Id != -1)
            {
                selectedTroupe = troupesShortVM.First(t => t.Id == scheduledSubmission.Submission.Troupe.Id);
                OnPropertyChanged(nameof(SelectedTroupe));
                OnPropertyChanged();
            }
        }
        public string HeadText => headText;

        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                if (selectedDate == value) return;
                selectedDate = value.Date + selectedDate.TimeOfDay;
                OnPropertyChanged(nameof(SelectedDate));
                OnPropertyChanged();
            }
        }
        public string SelectedTime
        {
            get => selectedDate.ToShortTimeString();
            set
            {
                if (selectedDate.ToShortTimeString() == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputTimeOnlyCorrect(value))
                {
                    selectedDate = selectedDate.Date + TimeSpan.Parse(value);
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Время начала'", "Red");
                OnPropertyChanged();
            }
        }
        public string NumberOfSoldTickets
        {
            get => numberOfSoldTickets.ToString();
            set
            {
                if (numberOfSoldTickets.ToString() == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputNumberOfSoldTicketsCorrect(value))
                {
                    numberOfSoldTickets = Convert.ToInt32(value);
                    HideMessage();
                }
                else ShowMessage("Некоректно заполнено поле 'Количество проданных билетов'", "Red");

            }
        }

        public string Message => message;
        public bool IsMessageVisible => messageVisibility;
        public string MessageColor => messageColor;

        public string ButtonContent => buttonContent;
        

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
                InitializeTroupesAsync();
                OnPropertyChanged();
            }
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
                OnPropertyChanged(nameof(SelectedTroupe));
                OnPropertyChanged();
            }
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

        public ICommand CompletAction
        {
            get
            {
                return new RelayCommand(
                    (_) => CompleteActionImpl()
                    );
            }
        }
        private async void CompleteActionImpl()
        {
            if(IsScheduledSubmissionFilled())
            {
                if (scheduledSubmission.Id == -1)
                {
                    var updatedSession = new Session(-1, selectedDate, numberOfSoldTickets);
                    var updatedSubmission = new Submission(-1, plays.Find(p => p.Id == selectedPlay.Id), troupes.Find(t => t.Id == selectedTroupe.Id));
                    var newScheduledSubmission = new ScheduledSubmission(-1, updatedSession, updatedSubmission);
                    int newScheduledSubmissionId = dataBaseSQLSaver.AddNewSessionSubmissions(newScheduledSubmission);
                    scheduledSubmission = new ScheduledSubmission(newScheduledSubmissionId, updatedSession, updatedSubmission);
                }
                else
                {
                    var updatedSession = new Session(scheduledSubmission.Session.Id, selectedDate, numberOfSoldTickets);
                    var updatedSubmission = new Submission(scheduledSubmission.Submission.Id, plays.Find(p => p.Id == selectedPlay.Id), troupes.Find(t => t.Id == selectedTroupe.Id));
                    var newScheduledSubmission = new ScheduledSubmission(scheduledSubmission.Id, updatedSession, updatedSubmission);
                    dataBaseSQLSaver.UpdateSessionSubmissions(newScheduledSubmission);
                }
                ShowMessage("Данные сохранены успешно", "Green");
                await Task.Delay(5000);
                HideMessage();
            }
            else
            {
                ShowMessage("Не все поля заполнены!", "Red");
                await Task.Delay(5000);
                HideMessage();
            }
        }
        private bool IsScheduledSubmissionFilled()
        {
            return IsInputNumberOfSoldTicketsCorrect(numberOfSoldTickets.ToString()) & IsInputTimeOnlyCorrect(selectedDate.TimeOfDay.ToString()) & IsPlaySelected() & IsTroupeSelected();
        }
        private bool IsPlaySelected()
        {
            return selectedPlay != null;
        }
        private bool IsTroupeSelected()
        {
            return selectedTroupe != null;
        }
        private bool IsInputTimeOnlyCorrect(string inputTimeOnly)
        {
            return TimeOnly.TryParse(inputTimeOnly, out _);
        }
        private bool IsInputNumberOfSoldTicketsCorrect(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]+$") && input != "";
        }

        public ICommand ToTimetable
        {
            get
            {
                return new RelayCommand(
                    (_) => ToTimetableImpl()
                    );
            }
        }
        private void ToTimetableImpl()
        {
            mwVm.CurrentViewModel = new TimetableViewModel(mwVm);
        }
    }
}
