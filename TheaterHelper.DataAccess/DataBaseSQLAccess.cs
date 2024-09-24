using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelper.DataAccess
{
    public class DataBaseSQLAccess : IDataAccess
    {
        private string connectionString = "Server=DENIS-DESKTOP;Database=TheaterHelper.Datebase;Trusted_Connection=True;TrustServerCertificate=true;";
        public async Task<List<Actor>> GetActors()
        {
            List<Actor> actors = new List<Actor>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string command = "Select * from ActorsWithTitles";

                SqlCommand sqlCommand = new SqlCommand(command, connection);

                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync()) 
                        {
                            int id = reader.GetInt32(0);
                            string surname = reader.GetString(1);
                            string name = reader.GetString(2);
                            string patronymic = reader.GetString(3);
                            double salary = Convert.ToDouble(reader.GetDecimal(4));
                            string title = reader.GetString(5);

                            actors.Add(new Actor(id, surname, name, patronymic, salary, title));
                        }
                    }
                }
            }
            return actors;
        }        
        public async Task<Repertoire> GetRepertoire()
        {
            List<Play> plays = new List<Play>();
            Dictionary<int, string> playsRelevance = new Dictionary<int, string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                
                string expCommand = "GetRepertoire";

                SqlCommand sqlCommand = new SqlCommand(expCommand, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                using(SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int playid = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            TimeOnly duration = TimeOnly.FromTimeSpan(reader.GetTimeSpan(2));
                            string relevance = reader.GetString(3);

                            plays.Add(new Play(playid, name, duration));
                            playsRelevance.Add(playid, relevance);
                        }
                    }
                }
            }
            return new Repertoire(plays, playsRelevance);
            
        }
        public async Task<List<Role>> GetRoles()
        {
            List<Role> roles = new List<Role>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string command = "SELECT * FROM RolesWithPlayTitleAndImportance";

                SqlCommand sqlCommand = new SqlCommand(command, connection);

                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string playTitle = reader.GetString(1);
                            string name = reader.GetString(2);
                            string roleImportance = reader.GetString(3);

                            roles.Add(new Role(id, playTitle, name, roleImportance));
                        }
                    }
                }
            }
            return roles;
        }
        public async Task<List<Troupe>> GetTroupes()
        {
            List<Troupe> troupes = new List<Troupe>();
            List<Executors> troupesComposition = new List<Executors>();
            List<Actor> actors = await GetActors();
            List<Role> roles = await GetRoles();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string command = "SELECT * FROM TroupeComposition";
                SqlCommand sqlCommand = new SqlCommand(command, connection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Executors exec = new Executors(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                roles.First(r => r.Id == reader.GetInt32(2)),
                                actors.First(a => a.Id == reader.GetInt32(3)),
                                actors.First(a => a.Id == reader.GetInt32(4))
                                );
                            troupesComposition.Add(exec);
                        }
                    }
                }

                command = "SELECT * FROM Troupes";
                sqlCommand = new SqlCommand(command, connection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            troupes.Add(new Troupe(
                                reader.GetInt32(0),
                                troupesComposition.Where(e => e.TroupeId == reader.GetInt32(0)).ToList()));
                        }
                    }
                }

            }
            return troupes;
        }
        public async Task<List<Submission>> GetSubmissions()
        {
            List<Submission> submissions = new List<Submission>();
            var repertoire = await GetRepertoire();
            var troupes = await GetTroupes();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string command = "SELECT * FROM Submissions";

                SqlCommand sqlCommand = new SqlCommand(command, connection);
                using(SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            
                            int id = reader.GetInt32(0);
                            int playId = reader.GetInt32(1);
                            int troupeId = reader.GetInt32(2);

                            var submission = new Submission(
                                id,
                                repertoire.Plays.Find(r => r.Id == playId),
                                troupes.Find(t => t.Id == troupeId));

                            submissions.Add(submission);
                        }
                    }
                }
            }
            return submissions;
        }
        public async Task<List<Session>> GetSessions()
        {
            List<Session> sessions = new List<Session>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string command = "SELECT * FROM Sessions";

                SqlCommand sqlCommand = new SqlCommand(command, connection);
                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            DateTime sessionDate = reader.GetDateTime(1);
                            int numberOfSoldTickets = reader.GetInt32(2);

                            var session = new Session(id, sessionDate, numberOfSoldTickets);
                            session.UpdateState();
                            sessions.Add(session);
                        }
                    }
                }
            }
            return sessions;
        }
        public async Task<Timetable> GetTimetable(DateTimeOffset date)
        {
            var sessions = await GetSessions();
            var submissions = await GetSubmissions();
            var schedule = new List<ScheduledSubmission>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                

                string command = "GetDatesByMonthAndYear";

                SqlCommand sqlCommand = new SqlCommand(command, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Date", date);

                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while(await reader.ReadAsync())
                        {
                            int scheduleId = reader.GetInt32(0);
                            int sessionId = reader.GetInt32(1);
                            int submissionId = reader.GetInt32(2);

                            var scheduledSubmission = new ScheduledSubmission(
                                scheduleId,
                                sessions.Find(s => s.Id == sessionId),
                                submissions.Find(s => s.Id == submissionId));

                            schedule.Add(scheduledSubmission);
                        }
                    }
                }
                return new Timetable(schedule);
            }

        }
        public async Task<List<Role>> GetRolesForPlay(string playName)
        {
            List<Role> roles = new List<Role>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string spProcedure = "GetRolesForPlay";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PlayName", playName);

                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string playTitle = reader.GetString(1);
                            string name = reader.GetString(2);
                            string roleImportance = reader.GetString(3);

                            roles.Add(new Role(id, playTitle, name, roleImportance));
                        }
                    }
                }
            }
            return roles;
        }
        public async Task<List<Troupe>> GetTroupesForPlay(string playName)
        {
            List<Troupe> troupes = new List<Troupe>();
            List<Executors> troupesComposition = new List<Executors>();
            List<Actor> actors = await GetActors();
            List<Role> roles = await GetRoles();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string spProcedure = "GetTroupesForPlay";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PlayName", playName);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Executors exec = new Executors(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                roles.First(r => r.Id == reader.GetInt32(2)),
                                actors.First(a => a.Id == reader.GetInt32(3)),
                                actors.First(a => a.Id == reader.GetInt32(4))
                                );
                            troupesComposition.Add(exec);
                        }
                    }
                }

                string command = "SELECT * FROM Troupes";
                
                sqlCommand = new SqlCommand(command, connection);

                using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int troupeId = reader.GetInt32(0);
                            // Фильтрация трупп по troupesComposition
                            if (troupesComposition.Any(e => e.TroupeId == troupeId))
                            {
                                troupes.Add(new Troupe(
                                    troupeId,
                                    troupesComposition.Where(e => e.TroupeId == troupeId).ToList()));
                            }
                        }
                    }
                }

            }
            return troupes;
        }
    }
}

