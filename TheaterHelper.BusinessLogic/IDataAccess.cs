using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelper.BusinessLogic
{
    public interface IDataAccess
    {
        Task<List<Actor>> GetActors();
        Task<Repertoire> GetRepertoire();
        Task<List<Role>> GetRoles();
        Task<List<Troupe>> GetTroupes();
        Task<List<Submission>> GetSubmissions();
        Task<List<Session>> GetSessions();
        Task<Timetable> GetTimetable(DateTimeOffset date);
        Task<List<Role>> GetRolesForPlay(string playName);
        Task<List<Troupe>> GetTroupesForPlay(string playName);
    }
}
