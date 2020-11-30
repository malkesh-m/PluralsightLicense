using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using PluralsightLicense.Data;

namespace PluralsightLicense.Service
{
    public interface IDeveloperService
    {

        bool CreateDeveloper(Developer developer);

        List<Developer> GetDevelopers(int teamId);

        bool DeleteDeveloper(int developerId);
    }
    public class DeveloperService : IDeveloperService
    {
        private Logger log;
        private string connectionString = Properties.Resources.ConnectionString;
        public DeveloperService()
        {
            //this.log = log;
        }

        /// <summary>
        /// Returns false if it can't create the item
        /// </summary>
        /// <returns>bool</returns>
        public bool CreateDeveloper(Developer developer)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Insert INTo tbl_Developer (DeveloperName,IsPluralsightLicenseAssigned) VALUES('" + developer.DeveloperName + "','" + developer.IsPluralsightLicenseAssigned + "')", sqlConnection);
            sqlConnection.Open();
            var v = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            return true;
        }

        /// <summary>
        /// Returns all if there is no team id given
        /// </summary>
        /// <returns>bool</returns>
        public List<Developer> GetDevelopers(int teamId)
        {

            SqlConnection sqlConnection = new SqlConnection(connectionString);
           if (teamId == 0)
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM tbl_Developer", sqlConnection);
                sqlConnection.Open();
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                da.Fill(dataTable);
                sqlConnection.Close();
                var result = new List<Developer>();
                result = ConvertDataTableToList<Developer>(dataTable);
                return result;
            }
            else
            {
                //Pendig 
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM tbl_Developer", sqlConnection);
                sqlConnection.Open();
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                da.Fill(dataTable);
                sqlConnection.Close();
                var result = new List<Developer>();
                result = ConvertDataTableToList<Developer>(dataTable);
                return result;
            }
        }

        /// <summary>
        /// Returns false if it can't delete the item
        /// </summary>
        /// <returns>bool</returns>
        public bool DeleteDeveloper(int developerId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("DELETE FROM tbl_TeamDevlopers WHERE DeveloperId=" + developerId, sqlConnection);
            SqlCommand sqlCommand1 = new SqlCommand("DELETE FROM tbl_Developer WHERE DeveloperId=" + developerId, sqlConnection);
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
        public bool IsAvailable(int developerId)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand("Select DeveloperId FROM tbl_Developer WHERE DeveloperId=" + developerId, sqlConnection);
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