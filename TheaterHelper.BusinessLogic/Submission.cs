using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Submission //Представление
    {
        private int id;
        private Play play;
        private Troupe troupe;

        public int Id { get { return id; } }
        public Play Play { get { return play; } }
        public Troupe Troupe { get { return troupe; } }

        public Submission(int id, Play play, Troupe troupe)
        {
            this.id = id;
            this.play = play;
            this.troupe = troupe;
        }
    }
}
