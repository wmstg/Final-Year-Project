using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjectMedi
{
    class Authentication
    {
        /// <summary>
        /// Login process - Returns a boolean value which determines if the login credentials are valid
        /// </summary>
        /// <param name="loginId">The login id of the authenticating user</param>
        /// <param name="password">The user supplied password</param>
        /// <returns></returns>
        public bool Login(String loginId, String password)
        {
            String queryString = "SELECT UserId, Password, PasswordSalt FROM [dbo].[" + DatabaseConstants.USERS_TABLE + "] " +
                "WHERE " + DatabaseConstants.LOGIN_ID + " = @id";
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@id";
            param.Value = loginId;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();
                
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(param);
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    if (VerifyPassword(password, sqlDataReader["Password"].ToString(), sqlDataReader["PasswordSalt"].ToString()))
                    {
                        Properties.Settings.Default.currentUserId = sqlDataReader["UserId"].ToString();
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Incorrect username or password");
                    }
                }
                connection.Close();
            }
            return false;
        }

        public static String HashPassword(String password, String salt)
        {
            var pass = new System.Security.Cryptography.Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 1000);
            return Convert.ToBase64String(pass.GetBytes(64));
        }

        public static String GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public static bool VerifyPassword(String password, String hashedPassword, String salt)
        {
            if (HashPassword(password, salt) == hashedPassword)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the session has expired
        /// </summary>
        /// <returns></returns>
        public static bool SessionTimeout()
        {
            int sessionLoginTime = Properties.Settings.Default.sessionLoginTime;
            if (DateTime.Now.AddMinutes(20) < DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public static void LogOut(System.Windows.Window window)
        {
            Properties.Settings.Default.currentUserId = "0";
            LoginWindow login = new LoginWindow();
            login.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            login.Show();
            window.Close();
        }
    }
}
