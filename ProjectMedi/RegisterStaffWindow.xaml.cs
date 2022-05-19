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
    /// Interaction logic for RegisterStaffWindow.xaml
    /// </summary>
    public partial class RegisterStaffWindow : Window
    {
        public RegisterStaffWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private void SubmitFormBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members first name");
            }
            else if (LastName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members surname");
            }
            else if (LoginName.Text.Length == 0)
            {
                MessageBox.Show("Please provide a login name for this staff member");
            }
            else if (LoginNameExist())
            {
                MessageBox.Show(String.Format("The username {0} already exists", LoginName.Text));
            }
            else if (AddressLine1.Text.Length == 0)
            {
                MessageBox.Show("Please enter the first line of address");
            }
            else if (City.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members city");
            }
            else if (Postcode.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members post code");
            }
            else
            {
                SaveFormData();
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void StaffTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBoxStaffType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                sqlCommand.Parameters.Add(new SqlParameter("@LoginId", LoginName.Text));
                sqlCommand.Parameters.Add(new SqlParameter("@EmailAddress", EmailAddress.Text));
                connection.Open();

                int userId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                connection.Close();

                CreateStaffData(userId);
                User.SaveAddress(userId, new string[] { AddressLine1.Text, AddressLine2.Text, Postcode.Text, City.Text });

                if (userId > 0)
                {
                    MessageBox.Show("A new staff member has been added.\nPlease note their login information\nLogin Id: " + LoginName.Text + "\nPassword: " + password, "User Registered");
                    AdministratorMainWindow administratorMainWindow = new AdministratorMainWindow();
                    administratorMainWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    administratorMainWindow.Show();
                    this.Close();
                }
            }
        }

        private int CreateStaffData(int userId)
        {
            String queryString = "INSERT INTO [dbo].[" + DatabaseConstants.STAFF_TABLE + "] (" + DatabaseConstants.USER_ID + ", " + DatabaseConstants.START_DATE + "," +
                DatabaseConstants.END_DATE + ", " + DatabaseConstants.ROLE_TYPE + ") OUTPUT INSERTED.StaffId VALUES (@UserId, @StartDate, @EndDate, @RoleType); ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@UserId", userId));
                sqlCommand.Parameters.Add(new SqlParameter("@StartDate", DateTime.Now));
                sqlCommand.Parameters.Add(new SqlParameter("@EndDate", DateTime.Now)); // No longer this - TODO Clean up
                sqlCommand.Parameters.Add(new SqlParameter("@RoleType", ComboBoxStaffType.Text));
                connection.Open();

                int staffId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                connection.Close();
                return staffId;
            }
        }

        private bool LoginNameExist()
        {
            string loginName = LoginName.Text;

            String queryString = "SELECT Count(*) FROM [dbo].[" + DatabaseConstants.USERS_TABLE + "] WHERE " + DatabaseConstants.LOGIN_ID + " = @loginId";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@loginId", loginName));
                connection.Open();
                int result = (int)sqlCommand.ExecuteScalar();
                connection.Close();

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void LabelBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManageStaffWindow manageStaffWindow = new ManageStaffWindow();
            manageStaffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageStaffWindow.Show();
            this.Close();
        }
    }
}
