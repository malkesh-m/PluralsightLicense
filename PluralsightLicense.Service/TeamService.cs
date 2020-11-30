using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using PluralsightLicense.Data;

namespace PluralsightLicense.Service
{
    public interface ITeamService
    {

        bool CreateTeam(Team Team);
        bool AssignDeveloperstoTeam(TeamDeveloper teamDeveloper);

        List<Team> GetTeams();

        bool DeleteTeam(int teamId);
        bool IsAvailable(int teamId);
    }
    public class TeamService : ITeamService
    {
        private Logger log;
        private string connectionString = Properties.Resources.ConnectionString;
        public TeamService()
        {
            //this.log = log;
        }

        /// <summary>
        /// Returns false if it can't create the item
        /// </summary>
        /// <returns>bool</returns>
        public bool CreateTeam(Team Team)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Insert INTO tbl_Team (TeamName) VALUES('" + Team.TeamName + "')", sqlConnection);
            sqlConnection.Open();
            var v = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            return true;
        }

        /// <summary>
        /// Returns all if there is no team id given
        /// </summary>
        /// <returns>bool</returns>
        public List<Team> GetTeams()
        {

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM tbl_Team", sqlConnection);
            sqlConnection.Open();
            DataTable dataTable = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            da.Fill(dataTable);
            sqlConnection.Close();
            var result = new List<Team>();
            result = ConvertDataTableToList<Team>(dataTable);
            return result;
        }




        /// <summary>
        /// Returns all if there is no team id given
        /// </summary>
        /// <returns>bool</returns>
        public List<TeamDevelopersVM> GetTeamDevelopers()
        {

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("SELECT Id,t.TeamName,d.DeveloperName FROM tbl_TeamDevelopers Td LEFT JOIN tbl_Developer d ON td.DeveloperId=d.DeveloperId LEFT JOIN tbl_Team t on Td.TeamId=t.TeamId", sqlConnection);
            sqlConnection.Open();
            DataTable dataTable = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            da.Fill(dataTable);
            sqlConnection.Close();
            var result = new List<TeamDevelopersVM>();
            result = ConvertDataTableToList<TeamDevelopersVM>(dataTable);
            return result;
        }

        /// <summary>
        /// Returns false if it can't delete the item
        /// </summary>
        /// <returns>bool</returns>
        public bool DeleteTeam(int teamId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM tbl_TeamDevlopers WHERE TeamId=" + teamId, sqlConnection);
            SqlCommand sqlCommand1 = new SqlCommand("DELETE FROM tbl_Team WHERE TeamId=" + teamId, sqlConnection);
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlCommand1.ExecuteNonQuery();
            sqlConnection.Close();
            return true;
        }

        /// <summary>
        /// Returns false if it can't delete the item
        /// </summary>
        /// <returns>bool</returns>
        public bool IsAvailable(int teamId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Select TeamID FROM tbl_Team WHERE TeamId=" + teamId, sqlConnection);
            sqlConnection.Open();
            if (Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0)
            {
                sqlConnection.Close();
                return true;
            }
            else
            {
                sqlConnection.Close();
                return false;
            }
        }


        public bool AssignDeveloperstoTeam(TeamDeveloper teamDeveloper)
        {
            foreach (var devId in teamDeveloper.DeveloperIds.Split(","))
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand("Insert INTO tbl_TeamDevelopers (TeamId,DeveloperId) VALUES(" + teamDeveloper.TeamId + "," + devId + ")", sqlConnection);
                sqlConnection.Open();
                var v = sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            return true;
        }

        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}