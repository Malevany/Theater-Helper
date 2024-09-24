using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;
using TheaterHelper.BusinessLogic.PayrollStrategies;
using TheaterHelper.BusinessLogic.PremiumDecorator;
using TheaterHelper.BusinessLogic.SessionState;

namespace TheaterHelper.BusinessLogic
{
    public class Theater
    {
        private Dictionary<Actor, double> actorsPayroll = new Dictionary<Actor, double>();
        private Dictionary<Actor, double> actorsPremium = new Dictionary<Actor, double>();
        private IPayrollStrategy selectedPayrollStrategy;
        private PremiumCalculator selectedPremiumCalculator = new PremiumCalculator();

        public Dictionary<Actor, double> ActorsPayroll { get { return actorsPayroll; } }
        public Dictionary<Actor, double> ActorsPremium { get { return actorsPremium; } }

        public Theater()
        {
            
        }
        public void GetSelectedPayrollStrategy(IPayrollStrategy selectedPayrollStrategy)
        {
            this.selectedPayrollStrategy = selectedPayrollStrategy;
        }
        public void GetSelectedPremiumCalculator(PremiumCalculator premiumCalculator)
        {
            selectedPremiumCalculator = premiumCalculator;
        }

        

        public void CalculatePayrollActors(string selectedDate, List<Actor> actors, Timetable timetable)
        {
            Dictionary<Actor, int> actorAppearances = new Dictionary<Actor, int>();
            foreach (var actor in actors)
            {
                actorAppearances.Add(actor, CountNumberOfActorAppearances(actor.Id, timetable));
            }
            actorsPayroll = selectedPayrollStrategy.CalculatePayrollActors(actorAppearances);
        }
        private int CountNumberOfActorAppearances(int actorId, Timetable timetable)
        {
            int countActorAppearances = 0;
            var pastSessions = timetable.Schedule.Where(t => t.Session.GetStatus() == "Проведена");
            foreach (var session in pastSessions)
            {
                var executors = session.Submission.Troupe.TroupeComposition;

                foreach (var executor in executors)
                {
                    if (executor.MainActor.Id == actorId || executor.Doubler.Id == actorId)
                    {
                        countActorAppearances++;
                        break;
                    }
                }
            }
            return countActorAppearances;
        }
        public void CalculatePremiumActors(string selectedDate, List<Actor> actors, Timetable timetable)
        {
            List<ActorStatistic> actorsStatistic = new List<ActorStatistic>();
            foreach (var actor in actors)
            {
                actorsStatistic.Add(new ActorStatistic(actor, CalculateAverageSuccessRateOfSubmissions(actor.Id, timetable), CountNumberOfActorAppearances(actor.Id, timetable)));
            }
            actorsPremium = selectedPremiumCalculator.CalculatePremium(actorsStatistic);
        }
        private double CalculateAverageSuccessRateOfSubmissions(int actorId, Timetable timetable)
        {
            double averageSuccessRateOfSubmissions = 0;
            int countActorAppearances = 0;
            var pastSessions = timetable.Schedule.Where(t => t.Session.GetStatus() == "Проведена");
            foreach (var session in pastSessions)
            {
                var executors = session.Submission.Troupe.TroupeComposition;
                foreach (var executor in executors)
                {
                    if (executor.MainActor.Id == actorId || executor.Doubler.Id == actorId)
                    {
                        countActorAppearances++;
                        if (session.Session.NumberSoldTickets <= 50)
                        {
                            averageSuccessRateOfSubmissions += 0.5;
                        }
                        else if (session.Session.NumberSoldTickets > 50 && session.Session.NumberSoldTickets <= 100)
                        {
                            averageSuccessRateOfSubmissions += 1;
                        }
                        else
                        {
                            averageSuccessRateOfSubmissions += 1.5;
                        }
                        break;
                    }
                }
            }
            if (countActorAppearances == 0)
            {
                return 0;
            }
            else
            {
                return averageSuccessRateOfSubmissions / countActorAppearances;
            }
        }
    }
}
//public double CalculateAmountOfActorsPayroll(string selectedDate, List<Actor> actors, Timetable timetable)
//{
//    double amountOfActorsPayroll = 0;
//    CalculatePayrollActors(selectedDate, actors, timetable);
//    foreach (var actorPayroll in actorsPayroll.Values)
//    {
//        amountOfActorsPayroll += actorPayroll;
//    }
//    return amountOfActorsPayroll;
//}
//public double CalculateAmountOfActorsPremium(string selectedDate, List<Actor> actors, Timetable timetable)
//{
//    double amountOfActorsPremium = 0;
//    CalculatePremiumActors(selectedDate, actors, timetable);
//    foreach (var actorPremium in actorsPremium.Values)
//    {
//        amountOfActorsPremium += actorPremium;
//    }
//    return amountOfActorsPremium;
//}
//#region [Add, edit, remove actor]
//public void AddNewActor(Actor actor)
//{
//    actors.Add(actor);
//}
//public void EditActor(Actor editedActor)
//{
//    for (int i = 0; i < actors.Count; i++)
//    {
//        if (actors[i].Id == editedActor.Id)
//        {
//            actors[i] = editedActor;
//        }
//    }
//}
//public void RemoveActor(Actor actor)
//{
//    actors.Remove(actor);
//}
//#endregion

//public void AddNewRole(Role role)
//{
//    roles.Add(role);
//}

//public void EditExecutors(Executors updatedExecutors)
//{
//    foreach (Troupe troupe in troupes)
//    {
//        if (troupe.Id == updatedExecutors.TroupeId)
//        {
//            troupe.TroupeComposition[updatedExecutors.Id] = updatedExecutors;
//        }
//    }
//}
//public void AddNewPlay(Play play)
//{
//    repertoire.Plays.Add(play);
//    repertoire.PlaysRelevance[play.Id] = "Архивный";
//}
//public void EditPlay(Dictionary<Play, string> newPlayInformation)
//{
//    foreach (var play in newPlayInformation.Keys)
//    {
//        repertoire.Plays[play.Id] = play;
//        repertoire.PlaysRelevance[play.Id] = newPlayInformation[play];
//    }
//}
//public void RemovePlay(Play play)
//{
//    repertoire.PlaysRelevance.Remove(play.Id);
//    repertoire.Plays.Remove(play);
//}