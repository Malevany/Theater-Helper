using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheaterHelper.BusinessLogic
{
    public class ActorStatistic
    {
        private Actor actor;
        private double actorSuccessRate;
        private int actorAppearances;

        public Actor Actor { get { return actor; } }
        public double ActorSuccessRate { get { return actorSuccessRate; } }
        public int ActorAppearances { get { return actorAppearances; } }

        public ActorStatistic(Actor actor, double actorSuccessRate, int actorAppearances)
        {
            this.actor = actor;
            this.actorSuccessRate = actorSuccessRate;
            this.actorAppearances = actorAppearances;
        }
    }
}
