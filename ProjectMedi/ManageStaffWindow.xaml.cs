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
    /// Interaction logic for ManageStaffWindow.xaml
    /// </summary>
    public partial class ManageStaffWindow : Window
    {
        public ManageStaffWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            SearchStaffList();
        }

        private void ButtonNewStaff_Click(object sender, RoutedEventArgs e)
        {
            RegisterStaffWindow registerStaffWindow= new RegisterStaffWindow();
            registerStaffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            registerStaffWindow.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            AdministratorMainWindow administratorMainWindow = new AdministratorMainWindow();
            administratorMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            administratorMainWindow.Show();
            this.Close();
        }

        private void SearchStaffList()
        {
            string queryString = "SELECT " + DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.USER_ID + ", " +
                DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.STAFF_ID + "," +
                DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.ROLE_TYPE + ", " + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.FIRSTNAME + "," +
                DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.LASTNAME + "," + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.EMAIL_ADDRESS +
                " FROM [dbo]." + DatabaseConstants.STAFF_TABLE +
                " LEFT JOIN [dbo]." + DatabaseConstants.USERS_TABLE + " ON " + DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.USER_ID + "=" + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.USER_ID;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var listViewItem = new Staff
                        {
                            UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal(DatabaseConstants.STAFF_ID)),
                            LastName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.LASTNAME)),
                            FirstName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.FIRSTNAME)),
                            RoleType = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.ROLE_TYPE)),
                            EmailAddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.EMAIL_ADDRESS))
                        };
                        //PatientsList.ItemsSource = listViewItem;
                        StaffMemberList.Items.Add(listViewItem);
                    }
                }
            }
        }

        private void ButtonDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete the staff member", "Delete Staff Member", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                // Delete the staff user
            }
        }

        private void ButtonEditStaff_Click(object sender, RoutedEventArgs e)
        {
            if (StaffMemberList.SelectedIndex != -1)
            {
                EditStaffWindow editStaffWindow = new EditStaffWindow(((Staff)StaffMemberList.SelectedItems[0]).UserId);
                editStaffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                editStaffWindow.Show();
                this.Close();
            }
        }
    }
}
