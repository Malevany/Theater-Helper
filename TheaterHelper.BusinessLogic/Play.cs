using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Play //Спектакль
    {
        private int id;
        private string name;
        private TimeOnly duration;

        public int Id { get { return id; } }
        public string Name {  get { return name; } }
        public TimeOnly Duration { get { return duration; } }

        public Play(int id, string name, TimeOnly duration)
        {
            this.id = id;
            this.name = name;
            this.duration = duration;
        }
    }
}
