using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class User
    {
        public int UserId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String LoginId;
        public String EmailAddress { get; set; }
        public USER_TYPE UserType;

        public List<Address> Addresses;

        public enum USER_TYPE
        {
            STAFF,
            PATIENT
        }

        public static string GENDER_MALE { get { return "Male"; } }
        public static string GENDER_FEMALE { get { return "Female"; } }
        public static string GENDER_OTHER { get { return "Other"; } }

        public virtual void GetUserData()
        {
            String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.USERS_TABLE + "] " +
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

                while (sqlDataReader.Read())
                {
                    FirstName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.FIRSTNAME));
                    LastName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.LASTNAME));
                    EmailAddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.EMAIL_ADDRESS));
                }
                connection.Close();
            }
        }

        public virtual void GetAddressData()
        {
            Addresses = new Address(UserId).GetAddresses();
        }

        public static void SaveAddress(int userId, string[] data)
        {
            User.Address.ClearPrimaryAddress(userId);

            String queryString = "INSERT INTO [dbo].[" + DatabaseConstants.USER_ADDRESS_TABLE + "] (" + DatabaseConstants.USER_ID + ", " + DatabaseConstants.ADDRESS_LINE_1 + "," +
                DatabaseConstants.ADDRESS_LINE_2 + ", " + DatabaseConstants.POST_CODE + ", " + DatabaseConstants.CITY + ", " + DatabaseConstants.IS_PRIMARY_ADDRESS +
                ") VALUES (@UserId, @AddressLine1, @AddressLine2, @PostCode, @City, @IsPrimaryAddress); ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                sqlCommand.Parameters.Add(new SqlParameter("@AddressLine1", data[0]));
                sqlCommand.Parameters.Add(new SqlParameter("@AddressLine2", data[1])); // No longer this - TODO Clean up
                sqlCommand.Parameters.Add(new SqlParameter("@PostCode", data[2]));
                sqlCommand.Parameters.Add(new SqlParameter("@City", data[3]));
                sqlCommand.Parameters.Add(new SqlParameter("@IsPrimaryAddress", 1));
                connection.Open();

                int aff = sqlCommand.ExecuteNonQuery();
                connection.Close();
                return;
            }
        }

        public virtual void UpdateData()
        {
            string queryString = "UPDATE [dbo].[" + DatabaseConstants.USERS_TABLE + "] SET " + DatabaseConstants.FIRSTNAME + " = @Firstname, " + DatabaseConstants.LASTNAME + " = @LastName," +
                DatabaseConstants.EMAIL_ADDRESS + " = @EmailAddress" +
                " WHERE " + DatabaseConstants.USER_ID + " = @UserId";;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@FirstName", FirstName));
                sqlCommand.Parameters.Add(new SqlParameter("@LastName", LastName));
                sqlCommand.Parameters.Add(new SqlParameter("@EmailAddress", EmailAddress));
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", UserId));
                connection.Open();

                int v = sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        public class Address
        {
            private int UserId;
            public String AddressLine1;
            public String AddressLine2;
            public String PostCode;
            public String City;
            public bool IsPrimaryAddress;
            public String AddressDetails { get
                {
                    return String.Format(AddressLine1 + "\n" +
                                AddressLine2 + (AddressLine2 != null ? "\n" : "") +
                                City + "\n" +
                                PostCode);
                }
            }

            public List<Address> addresses = new List<Address>();
            
            public Address(int userId)
            {
                this.UserId = userId;
            }

            public Address()
            {
            }

            public static void UpdateAddress(int userId, string[] data)
            {
                User.SaveAddress(userId, data);
            }

            public static void ClearPrimaryAddress(int userId)
            {
                string queryString = "UPDATE [dbo].[" + DatabaseConstants.USER_ADDRESS_TABLE + "] SET " + DatabaseConstants.IS_PRIMARY_ADDRESS + " = @IsPrimaryAddress " +
               " WHERE " + DatabaseConstants.USER_ID + " = @UserId"; ;

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                    SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                    sqlCommand.Parameters.AddWithValue("@IsPrimaryAddress", 0);
                    sqlCommand.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    int v = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }

            public List<Address> GetAddresses()
            {

                String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.USER_ADDRESS_TABLE + "] " +
               "WHERE " + DatabaseConstants.USER_ID + " = @id " +
               "ORDER BY " + DatabaseConstants.IS_PRIMARY_ADDRESS + " DESC";
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

                    while (sqlDataReader.Read())
                    {
                        addresses.Add(new Address
                        {
                            AddressLine1 = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.ADDRESS_LINE_1)),
                            AddressLine2 = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.ADDRESS_LINE_2)),
                            PostCode = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.POST_CODE)),
                            City = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.CITY)),
                            IsPrimaryAddress = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal(DatabaseConstants.IS_PRIMARY_ADDRESS))
                        });
                        //addresses.Add(address);
                    }
                    connection.Close();
                
                }
            return addresses;
            }
        }
    }
}
