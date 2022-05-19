using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Interaction logic for SearchPatientsWindow.xaml
    /// </summary>
    public partial class SearchPatientsWindow : Window
    {
        private String queryString = null;

        public SearchPatientsWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            SetUpPatientListView();
            SearchPatientsList();
        }

        private void SearchPatientsList()
        {
            PatientsList.Items.Clear();

            if (queryString == null || SearchPatient.Text.Length == 0) {

                queryString = "SELECT " + DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.PATIENT_ID + "," +
                DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.USER_ID + ", " + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.FIRSTNAME + "," +
                DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.LASTNAME + "," + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.EMAIL_ADDRESS +
                " FROM [dbo]." + DatabaseConstants.PATIENTS_TABLE +
                " LEFT JOIN [dbo]." + DatabaseConstants.USERS_TABLE + " ON " + DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.USER_ID + "=" + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.USER_ID;
            }
            
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
                        var listViewItem = new User
                        {
                            UserId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal(DatabaseConstants.PATIENT_ID)),
                            LastName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.LASTNAME)),
                            FirstName = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.FIRSTNAME)),
                            EmailAddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.EMAIL_ADDRESS))
                        };
                        PatientsList.Items.Add(listViewItem);
                    }
                }
            }
        }

        private void SetUpPatientListView()
        {
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void PatientsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;

            if (listView != null && listView.SelectedItem != null)
            {
                User user = (User)listView.SelectedItems[0];

                /*ConsultationWindow consultationWindow = new ConsultationWindow(user.UserId);
                consultationWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                consultationWindow.Show();*/
                ViewPatientProfileWindow viewPatientProfileWindow = new ViewPatientProfileWindow(user.UserId);
                viewPatientProfileWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                viewPatientProfileWindow.ShowDialog();
                //this.Close();
            }
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        /// <summary>
        /// Looks up a patients details, querying against the database to find matches against the patients firstname, lastname or patient id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchPatient_KeyUp(object sender, KeyEventArgs e)
        {
            String searchQuery = SearchPatient.Text;

            queryString = "SELECT " + DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.PATIENT_ID + "," +
                DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.USER_ID + ", " + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.FIRSTNAME + "," +
                DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.LASTNAME + "," + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.EMAIL_ADDRESS +
                " FROM [dbo]." + DatabaseConstants.PATIENTS_TABLE +
                " LEFT JOIN [dbo]." + DatabaseConstants.USERS_TABLE + " ON " + DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.USER_ID + "=" + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.USER_ID + 
                " WHERE (" + DatabaseConstants.USERS_TABLE + "."  + DatabaseConstants.FIRSTNAME + " LIKE '" + searchQuery + "%' OR " + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.LASTNAME +
                " LIKE '" + searchQuery + "%') OR (" + DatabaseConstants.PATIENTS_TABLE + "." + DatabaseConstants.PATIENT_ID + " LIKE '" + searchQuery + "%')";

            SearchPatientsList();
        }
    }
}
