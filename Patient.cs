using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class Patient: User
    {
        public int PatientId { get; set; }
        public String Gender;
        public String ContactNumber;
        public String DateOfBirth;
        public String Nationality;
        public String DateJoinedSurgery;

        public String NameDetail
        {
            get
            {
                return String.Format("{0}, {1} - {2}", LastName, FirstName, PatientId);
            }
        }

        public bool IsPatient()
        {
            String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.PATIENTS_TABLE + "] " +
                           "WHERE " + DatabaseConstants.USER_ID + " = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = UserId;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(param);
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                return sqlDataReader.HasRows;
            }
        }

        public void GetUIDFromPatientId()
        {
            String queryString = "SELECT UserId FROM [dbo].[" + DatabaseConstants.PATIENTS_TABLE + "] " +
               "WHERE " + DatabaseConstants.PATIENT_ID + " = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = this.PatientId;

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

        public override void GetUserData()
        {
            String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.PATIENTS_TABLE + "] " +
               "WHERE " + DatabaseConstants.USER_ID + " = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = UserId;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(param);
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    UserType = USER_TYPE.PATIENT;
                    while (sqlDataReader.Read())
                    {
                        //UserId = Convert.ToInt32(sqlDataReader.GetValue(sqlDataReader.GetOrdinal(DatabaseConstants.USER_ID)));
                        PatientId = Convert.ToInt32(sqlDataReader.GetValue(sqlDataReader.GetOrdinal(DatabaseConstants.PATIENT_ID)));
                        Gender = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.GENDER));
                        ContactNumber = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.CONTACT_NUMBER));
                        DateOfBirth = sqlDataReader.GetValue(sqlDataReader.GetOrdinal(DatabaseConstants.DATE_OF_BIRTH)).ToString();
                        Nationality = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.NATIONALITY));

                        switch (sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.NATIONALITY))){
                            case "Other":
                                Gender = "Other";
                                break;
                            case "Female":
                                Gender = "Female";
                                break;
                            case "Male":
                            default:
                                Gender = "Male";
                                break;
                        }
                    }
                }
                else
                {
                    UserType = USER_TYPE.STAFF;
                }
                connection.Close();
                
                base.GetUserData();
            }
        }

        public override void UpdateData()
        {
            base.UpdateData();

            string queryString = "UPDATE " + DatabaseConstants.PATIENTS_TABLE + " SET " + DatabaseConstants.GENDER + " = @Gender, " + DatabaseConstants.CONTACT_NUMBER + " = @ContactNumber" +
                " WHERE " + DatabaseConstants.USER_ID + "= @UserId";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.AddWithValue("@Gender", Gender);
                sqlCommand.Parameters.AddWithValue("@ContactNumber", ContactNumber);
                sqlCommand.Parameters.AddWithValue("@UserId", UserId);
                connection.Open();

                int v = sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
