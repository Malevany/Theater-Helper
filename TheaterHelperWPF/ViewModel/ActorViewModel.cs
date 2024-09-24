using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TheaterHelper.BusinessLogic;
using TheaterHelper.DataAccess;

namespace TheaterHelperWPF.ViewModel
{
    public class ActorViewModel : ViewModelBase
    {
        private Actor actor;
        private MainWindowViewModel mwVm;
        private Actor updatedActor;
        private DataBaseSQLSaver dbSaver = new DataBaseSQLSaver();

        private ObservableCollection<string> titlesList = new ObservableCollection<string>()
        {
            //"",
            "Старший артист",
            "Заслуженный Артист РФ",
            "Заслуженный деятель искусств РФ",
            "Народный артист РФ"
        };
        private string surname;
        private string name;
        private string patronymic;
        private double salary;
        private string title;

        private string message;
        private bool messageVisibility;
        private string messageColor;

        private string buttonContent;

        public ActorViewModel(Actor actor, MainWindowViewModel mwVm)
        {
            this.actor = actor;
            this.mwVm = mwVm;
            if (actor.Id == -1)
            {
                buttonContent = "Добавить актера";
            }
            else buttonContent = "Сохранить изменения";
            surname = actor.Surname;
            name = actor.Name;
            patronymic = actor.Patronymic;
            salary = actor.Salary;
            title = actor.Title;
        }

        public string Surname
        {
            get => surname;
            set
            {
                if (surname == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    surname = value;
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Фамилия'", "Red");
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (name == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    name = value;
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Имя'", "Red");
                OnPropertyChanged();
            }
        }
        public string Patronymic
        {
            get => patronymic;
            set
            {
                if (patronymic == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    patronymic = value;
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Отчество'", "Red");
                OnPropertyChanged();
            }
        }
        public string Salary
        {
            get => salary.ToString();
            set
            {
                if (salary.ToString() == value)
                {
                    HideMessage();
                    return;
                }
                if (IsSalaryCorrect(value))
                {
                    salary = Convert.ToDouble(value);
                    HideMessage();
                }
                else ShowMessage("Некорректно заполнено поле 'Оклад за спектакль'", "Red");
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get
            {
                if (titlesList.First(t => t == title) == null)
                    return "";
                else
                    return titlesList.First(t => t == title);
            }
            set
            {
                if (title == value)
                {
                    HideMessage();
                    return;
                }
                if (IsInputStringCorrect(value))
                {
                    title = value;
                    HideMessage();
                }
                else ShowMessage("Неккоректно заполнено поле 'Звание'", "Red");
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> TitlesList
        {
            get => titlesList;
        }
        public string ButtonContent
        {
            get => buttonContent;
        }

        public string Message
        {
            get => message;
        }
        public bool IsMessageVisible
        {
            get => messageVisibility;
        }
        public string MessageColor
        {
            get => messageColor;
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
        private bool IsInputStringCorrect(string input)
        {
            return Regex.IsMatch(input, @"^[а-яА-Я ]+$") && input != "";
        }
        private bool IsAllStringsCorrect(List<string> input)
        {
            bool result = false;
            foreach (var item in input)
            {
                if (IsInputStringCorrect(item))
                {
                    result = true;
                }
                else return false;
            }
            return result;
        }
        private bool IsSalaryCorrect(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]+$") && input != "";
        }

        public ICommand CompletAction
            => new RelayCommand((_) => CompletActionImpl());
        private async void CompletActionImpl()
        {
            if (IsAllStringsCorrect(new List<string>() { surname, name, patronymic, title }) && IsSalaryCorrect(salary.ToString()))
            {
                updatedActor = new Actor(actor.Id, surname, name, patronymic, salary, title);
                if (updatedActor.Id == -1)
                {
                    int newActorID = dbSaver.AddNewActor(updatedActor);
                    actor = new Actor(newActorID, surname, name, patronymic, salary, title);
                }
                else dbSaver.UpdateActor(updatedActor);
                ShowMessage("Данные успешно сохранены!", "Green");

            }
            else ShowMessage("Не все поля заполнены корректно!", "Red");
            await Task.Delay(5000);
            HideMessage();
            OnPropertyChanged();
        }

        public ICommand ToActors
            => new RelayCommand((_) => ToActorsImpl());
        private void ToActorsImpl()
        {
            mwVm.CurrentViewModel = new ActorsViewModel(mwVm);
        }


    }
}
