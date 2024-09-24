using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;
using TheaterHelperWPF.View;

namespace TheaterHelperWPF.ViewModel
{
    public class TimetableViewModel : ViewModelBase
    {
        private MainWindowViewModel mwVm;
        private IDataAccess dataBaseSQL = new DataBaseSQLAccess();

        private DateTime selectedMonth = new DateTime(2023, 1, 1, 00, 00, 00);
        private Timetable timetable;

        private ObservableCollection<SubmissionShortViewModel> schedulesShortVM;
        private SubmissionShortViewModel selectedSchedule;

        public TimetableViewModel(MainWindowViewModel mwVm)
        {
            this.mwVm = mwVm;
            InitializeAsync();
        }
        private async Task InitializeAsync()
        {
            var timetableTask = dataBaseSQL.GetTimetable(selectedMonth);
            timetable = await timetableTask;
            IEnumerable<SubmissionShortViewModel> scheduleVMs =
                from schedule in timetable.Schedule
                select new SubmissionShortViewModel(schedule);
            schedulesShortVM = new ObservableCollection<SubmissionShortViewModel>(scheduleVMs);
            OnPropertyChanged(nameof(ScheduleForMonth));
            OnPropertyChanged(nameof(SelectedMonthName));
        }

        public ObservableCollection<SubmissionShortViewModel> ScheduleForMonth
        {
            get => schedulesShortVM;
        }
        public SubmissionShortViewModel SelectedSchedule
        {
            get => selectedSchedule;
            set
            {
                if (value == selectedSchedule) return;
                selectedSchedule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        public DateTime SelectedMonth
        {
            get => selectedMonth;
            set
            {
                if (selectedMonth == value) return;
                selectedMonth = value;
                OnPropertyChanged();
                InitializeAsync();
            }
        }
        public string SelectedMonthName
        {
            get => $"{SelectedMonth:Y}";
        }
        public bool IsVisible => IsScheduleSelected();
        public ICommand AddSchedule
        {
            get
            {
                return new RelayCommand(
                    (_) => AddScheduleImpl()
                    );
            }
        }
        public ICommand ShowSchedule
        {
            get
            {
                return new RelayCommand(
                    (_) => ShowScheduleImpl(),
                    (_) => IsScheduleSelected()
                    );
            }
        }

        private bool IsScheduleSelected()
        {
            return selectedSchedule != null;
        }
        private void ShowScheduleImpl()
        {
            var scheduleSubmission = timetable.Schedule.Find(s => s.Id == selectedSchedule.ID);
            mwVm.CurrentViewModel = new SubmissionViewModel(mwVm, scheduleSubmission);
        }
        private void AddScheduleImpl()
        {
            var scheduleSubmission = new ScheduledSubmission(-1, null, null);
            mwVm.CurrentViewModel = new SubmissionViewModel(mwVm, scheduleSubmission);
        }
    }
}
