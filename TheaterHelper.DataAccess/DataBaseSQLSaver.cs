using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheaterHelper.BusinessLogic;

namespace TheaterHelper.DataAccess
{
    public class DataBaseSQLSaver
    {
        private string connectionString = "Server=DENIS-DESKTOP;Database=TheaterHelper.Datebase;Trusted_Connection=True;TrustServerCertificate=true;";

        public int AddNewActor(Actor actor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string spProcedure = "AddActorWithTitle";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Surname", actor.Surname);
                sqlCommand.Parameters.AddWithValue("@Name", actor.Name);
                sqlCommand.Parameters.AddWithValue("@Patronymic", actor.Patronymic);
                sqlCommand.Parameters.AddWithValue("@Salary", actor.Salary);
                sqlCommand.Parameters.AddWithValue("@TitleName", actor.Title);
                SqlParameter newActorID = new SqlParameter
                {
                    ParameterName = "@NewActor_ID",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(newActorID);

                sqlCommand.ExecuteNonQuery();

                return (int)sqlCommand.Parameters["@NewActor_ID"].Value;
            }
        }
        public void UpdateActor(Actor actor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string spProcedure = "UpdateActorWithTitle";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Actor_ID", actor.Id);
                sqlCommand.Parameters.AddWithValue("@Surname", actor.Surname);
                sqlCommand.Parameters.AddWithValue("@Name", actor.Name);
                sqlCommand.Parameters.AddWithValue("@Patronymic", actor.Patronymic);
                sqlCommand.Parameters.AddWithValue("@Salary", actor.Salary);
                sqlCommand.Parameters.AddWithValue("@TitleName", actor.Title);
                sqlCommand.ExecuteNonQuery();
            }
        }
        
        public int AddNewPlay(Play play)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string spProcedure = "AddNewPlay";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PlayName", play.Name);
                sqlCommand.Parameters.AddWithValue("@Duration", play.Duration);
                sqlCommand.Parameters.AddWithValue("@PlayStatusName", "Архивная");

                SqlParameter newPlayID = new SqlParameter
                {
                    ParameterName = "@NewPlay_ID",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(newPlayID);

                sqlCommand.ExecuteNonQuery();

                return (int)sqlCommand.Parameters["@NewPlay_ID"].Value;
            }
        }
        public void UpdatePlay(Play play)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string spProcedure = "UpdatePlay";

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Play_ID", play.Id);
                sqlCommand.Parameters.AddWithValue("@PlayName", play.Name);
                sqlCommand.Parameters.AddWithValue("@Duration", play.Duration);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public int AddNewRole(Role role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string spProcedure = "AddNewRole";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@CharacterName", role.Name);
                sqlCommand.Parameters.AddWithValue("@ImportanceName", role.RoleImportance);
                sqlCommand.Parameters.AddWithValue("@PlayTitle", role.PlayTitle);
                SqlParameter newRoleID = new SqlParameter
                {
                    ParameterName = "@NewRole_ID",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(newRoleID);

                sqlCommand.ExecuteNonQuery();

                return (int)sqlCommand.Parameters["@NewRole_ID"].Value;
            }
        }
        public void UpdateRole(Role role) 
        {
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                string spProcedure = "UpdateRole";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Role_ID", role.Id);
                sqlCommand.Parameters.AddWithValue("@RoleName", role.Name);
                sqlCommand.Parameters.AddWithValue("@ImportanceName", role.RoleImportance);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void SaveTroupe(Troupe troupe)
        {
            foreach (var executors in troupe.TroupeComposition)
            {
                if(executors.Id != -1)
                {
                    UpdateExecutors(executors);
                }
                else
                {
                    AddNewExecutors(troupe.Id, executors);
                }
            }
        }
        private void UpdateExecutors(Executors executors)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string spProcedure = "UpdateExecutors";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Executors_ID", executors.Id);
                sqlCommand.Parameters.AddWithValue("@Role_ID", executors.Role.Id);
                sqlCommand.Parameters.AddWithValue("@MainActor_ID", executors.MainActor.Id);
                sqlCommand.Parameters.AddWithValue("@Doubler_ID", executors.Doubler.Id);
                sqlCommand.ExecuteNonQuery();
            }
        }
        private void AddNewExecutors(int troupeId, Executors executors)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string spProcedure = "AddNewExecutors";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Troupe_ID", troupeId);
                sqlCommand.Parameters.AddWithValue("@Role_ID", executors.Role.Id);
                sqlCommand.Parameters.AddWithValue("@MainActor_ID", executors.MainActor.Id);
                sqlCommand.Parameters.AddWithValue("@Doubler_ID", executors.Doubler.Id);
                sqlCommand.ExecuteNonQuery();
            }
        }
        public int AddNewTroupe(Troupe troupe)
        {
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                string spProcedure = "AddNewTroupe";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                SqlParameter outputIdParam = new SqlParameter("@NewTroupeID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(outputIdParam);
                sqlCommand.ExecuteNonQuery();

                int newTroupeId = (int)outputIdParam.Value;

                
                foreach(var executors in troupe.TroupeComposition)
                {
                    AddNewExecutors(newTroupeId, executors);
                }
                return newTroupeId;
            }
        }
        

        public void UpdateSessionSubmissions(ScheduledSubmission scheduledSubmission)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string spProcedure = "UpdateSessionSubmissions";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Session_ID", scheduledSubmission.Session.Id);
                sqlCommand.Parameters.AddWithValue("@SessionDate", scheduledSubmission.Session.SessionDate);
                sqlCommand.Parameters.AddWithValue("@NumberOfSoldTickets", scheduledSubmission.Session.NumberSoldTickets);
                sqlCommand.Parameters.AddWithValue("@Submission_ID", scheduledSubmission.Submission.Id);
                sqlCommand.Parameters.AddWithValue("@Play_ID", scheduledSubmission.Submission.Play.Id);
                sqlCommand.Parameters.AddWithValue("@Troupe_ID", scheduledSubmission.Submission.Troupe.Id);
                sqlCommand.ExecuteNonQuery();
            }
        }
        public int AddNewSessionSubmissions(ScheduledSubmission scheduledSubmission)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string spProcedure = "AddSessionSubmissions";

                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(spProcedure, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@SessionDate", scheduledSubmission.Session.SessionDate);
                sqlCommand.Parameters.AddWithValue("@NumberOfSoldTickets", scheduledSubmission.Session.NumberSoldTickets);
                sqlCommand.Parameters.AddWithValue("@Play_ID", scheduledSubmission.Submission.Play.Id);
                sqlCommand.Parameters.AddWithValue("@Troupe_ID", scheduledSubmission.Submission.Troupe.Id);
                
                SqlParameter newSessionSubmissionsID = new SqlParameter
                {
                    ParameterName = "@NewSessionSubmissions_ID",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(newSessionSubmissionsID);

                sqlCommand.ExecuteNonQuery();

                return (int)sqlCommand.Parameters["@NewSessionSubmissions_ID"].Value;
            }
        }
    }
}
