using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class Repertoire
    {
        private List<Play> plays;
        private Dictionary<int, string> playsRelevance;
        public List<Play> Plays { get { return plays; } }
        public Dictionary<int, string> PlaysRelevance { get { return playsRelevance; } }
        public Repertoire(List<Play> plays, Dictionary<int, string> playsRelevance)
        {
            this.plays = plays;
            this.playsRelevance = playsRelevance;
        }
    }
}
