using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectMedi
{
    /// <summary>
    /// Interaction logic for RegisterPatientWindow.xaml
    /// </summary>
    public partial class RegisterPatientWindow : Window
    {
        RadioButton GenderRadioButton = null;

        public RegisterPatientWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void SubmitFormBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients first name");
            }
            else if (LastName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients surname");
            }
            else if (DateOfBirth.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Please select a valid birth date");
            }
            /*else if ((bool)GenderRadioButton.IsChecked)
            {
                MessageBox.Show("Please select the patients gender");
            }*/
            else if (AddressLine1.Text.Length == 0)
            {
                MessageBox.Show("Please enter the first line of address");
            }
            else if (City.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients city");
            }
            else if (Postcode.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients post code");
            }
            else
            {
                SaveFormData();
            }
        }

        private void GenderRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)((RadioButton)sender).IsChecked)
            {
                GenderRadioButton = (RadioButton)sender;
            }
        }

        private void SaveFormData()
        {
            // Create password
            string passwordSalt = Authentication.GenerateSalt();
            string password = Utility.PasswordGenerator();
            string passwordHash = Authentication.HashPassword(password, passwordSalt);

            // Insert the data
            String queryString = "INSERT INTO [dbo].[" + DatabaseConstants.USERS_TABLE + "] (" + DatabaseConstants.FIRSTNAME + ", " + DatabaseConstants.LASTNAME + "," +
                DatabaseConstants.LOGIN_ID + ", " + DatabaseConstants.PASSWORD + ", " + DatabaseConstants.PASSWORD_SALT + ", " + DatabaseConstants.EMAIL_ADDRESS +
                ") OUTPUT INSERTED.UserId VALUES (@FirstName, @LastName, @LoginId, @Password, @PasswordSalt, @EmailAddress); ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@FirstName", FirstName.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@LastName", LastName.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@Password", passwordHash));
                sqlCommand.Parameters.Add(new SqlParameter("@PasswordSalt", passwordSalt));
                sqlCommand.Parameters.Add(new SqlParameter("@LoginId", ""));
                sqlCommand.Parameters.Add(new SqlParameter("@EmailAddress", EmailAddress.Text));
                connection.Open();
                
                int userId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                connection.Close();
                
                int patientLoginId = CreatePatientData(userId);
                User.SaveAddress(userId, new string[] { AddressLine1.Text, AddressLine2.Text, Postcode.Text, City.Text});
                int res = SetPatientLoginId(userId, patientLoginId);

                if (res > 0)
                {
                    MessageBox.Show("A New patient has been added.\nLogin Id: " + patientLoginId +"\nPassword: " + password, "User Registered");
                    // The code below redirected staff members to the administrative view ~ Do not remove until the replaced code is fully tested

                    /*AdministratorMainWindow administratorMainWindow = new AdministratorMainWindow();
                    administratorMainWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    administratorMainWindow.Show();
                    this.Close();*/
                    UserAccessControl.MainViewControl(this);
                }
            }
        }

        private int CreatePatientData(int userId)
        {
            String queryString = "INSERT INTO [dbo].[" + DatabaseConstants.PATIENTS_TABLE + "] (" + DatabaseConstants.USER_ID + ", " + DatabaseConstants.GENDER + "," +
                DatabaseConstants.CONTACT_NUMBER + ", " + DatabaseConstants.DATE_OF_BIRTH + ", " + DatabaseConstants.NATIONALITY + ", " + DatabaseConstants.DATE_JOINED_SURGERY +
                ") OUTPUT INSERTED.PatientId VALUES (@UserId, @Gender, @ContactNumber, @DateOfBirth, @Nationality, @DateJoinedSurgery); ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                sqlCommand.Parameters.Add(new SqlParameter("@Gender", LastName.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@ContactNumber", ContactNumber.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@DateOfBirth", DateOfBirth.SelectedDate));
                sqlCommand.Parameters.Add(new SqlParameter("@Nationality", Nationality.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@DateJoinedSurgery", ""));
                connection.Open();

                int patientId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                connection.Close();
                return patientId;
            }
        }

        private int SetPatientLoginId(int userId, int patientId)
        {
            string queryString = "UPDATE [dbo]." + DatabaseConstants.USERS_TABLE + " SET LoginId=@LoginId WHERE UserId=@UserId";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                sqlCommand.Parameters.Add(new SqlParameter("@LoginId", patientId));
                connection.Open();

                int sqlResponse = sqlCommand.ExecuteNonQuery();
                connection.Close();

                return sqlResponse;
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void SaveAddressData()
        {

        }
    }
}
