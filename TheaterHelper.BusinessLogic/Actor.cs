using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Actor
    {
        private int id;
        private string surname;
        private string name;
        private string patronymic; //Отчество
        private double salary;
        private string title;

        public int Id { get { return id; } }
        public string Surname { get { return surname; } }
        public string Name { get { return name; } }
        public string Patronymic { get { return patronymic; } }
        public double Salary { get {  return salary; } }
        public string Title { get { return title; } }
        public Actor(int id, string surname, string name, string patronymic, double salary, string title)
        {
            this.id = id;
            this.surname = surname;
            this.name = name;
            this.patronymic = patronymic;
            this.salary = salary;
            this.title = title;
        }
    }
}
