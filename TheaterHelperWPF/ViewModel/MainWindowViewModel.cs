using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase currentViewModel;
        
        public MainWindowViewModel() 
        {
            CurrentViewModel = new HomeScreenViewModel();
            OnPropertyChanged();
        }

        public ViewModelBase CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowHomeScreen
        {
            get
            {
                return new RelayCommand(
                    o => CurrentViewModel = new HomeScreenViewModel());
            }
        }

        public ICommand ShowActors
        {
            get
            {
                return new RelayCommand(
                     o => ShowActorsImplAsync());
            }
        }
        private void ShowActorsImplAsync()
        {
            var actorsViewModel = new ActorsViewModel(this);
            CurrentViewModel = actorsViewModel;            
        }

        public ICommand ShowRepertoire
        {
            get
            {
                return new RelayCommand(
                    o => ShowRepertoireAsync());
            }
        }
        private void ShowRepertoireAsync()
        {
            var repertoireViewModel = new RepertoireViewModel(this);
            CurrentViewModel = repertoireViewModel;
        }

        public ICommand ShowTimetable
            => new RelayCommand((_) => ShowTimetableImpl());
        private void ShowTimetableImpl()
        {
            var timetableViewModel = new TimetableViewModel(this);
            CurrentViewModel = timetableViewModel;
        }
    }
}
