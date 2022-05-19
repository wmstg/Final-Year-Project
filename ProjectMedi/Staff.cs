using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class Staff: User
    {
        public int RoleId;
        public int StaffId;
        public String RoleType { get; set; }

        public enum ROLE_TYPE
        {
            ADMINISTRATOR,
            SECRETARY,
            DOCTOR
        }

        public override void GetUserData()
        {
            base.GetUserData();

            String queryString = "SELECT * FROM [dbo].[Staff] " +
               "WHERE UserId = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = UserId;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(param);
                connection.Open();
                Console.WriteLine("FN: " + this.FirstName);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        StaffId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal(DatabaseConstants.STAFF_ID));
                        RoleType = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.ROLE_TYPE));
                    } 
                }
                connection.Close();
            }
        }

        public void GetUIDFromStaffId()
        {
            String queryString = "SELECT UserId FROM [dbo].[" + DatabaseConstants.STAFF_TABLE + "] " +
               "WHERE " + DatabaseConstants.STAFF_ID + " = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = this.StaffId;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(param);
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        UserId = Convert.ToInt32(sqlDataReader.GetValue(sqlDataReader.GetOrdinal(DatabaseConstants.USER_ID)));
                    }
                }
            }
        }

        public static Staff GetSessionStaff()
        {
            int userId = Int32.Parse(Properties.Settings.Default.currentUserId);

            Staff loggedInStaff = new Staff();
            loggedInStaff.UserId = userId;
            loggedInStaff.GetUserData();

            return loggedInStaff;
        }

        public override void UpdateData()
        {
            base.UpdateData();

            string queryString = "UPDATE " + DatabaseConstants.STAFF_TABLE + " SET " + DatabaseConstants.ROLE_TYPE + " = @RoleType" +
                " WHERE " + DatabaseConstants.USER_ID + "= @UserId";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.AddWithValue("@RoleType", RoleType);
                sqlCommand.Parameters.AddWithValue("@UserId", UserId);
                connection.Open();

                int v = sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
