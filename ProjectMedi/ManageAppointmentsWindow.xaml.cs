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
    /// Interaction logic for ManageAppointmentsWindow.xaml
    /// </summary>
    public partial class ManageAppointmentsWindow : Window
    {
        List<Appointments> appointments = new List<Appointments>();

        public ManageAppointmentsWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            AppointmentView();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void GetAppointments()
        {
            String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.PATIENT_APPOINTMENTS_TABLE + "] " +
                "WHERE AppointmentDate > @date ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@date", DateTime.Now));
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        appointments.Add(new Appointments()
                        {
                            AppointmentId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("AppointmentId")),
                            AppointmentDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("AppointmentDate")).ToString(),
                            PatientId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PatientId")),
                            Duration = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Duration")),
                            ConsultantId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("ConsultantId")),
                            Attended = sqlDataReader.GetBoolean(sqlDataReader.GetOrdinal("Attended"))
                        });
                    }
                }
                connection.Close();
            }
        }

        private void AppointmentView()
        {
            GetAppointments();
            //AppointmentsGridView.DataContext = appointments;
            AppointmentsList.ItemsSource = appointments;
        }

        private void ButtonViewPatient_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsList.SelectedIndex != -1)
            {
                ViewPatientProfileWindow viewPatientProfileWindow = new ViewPatientProfileWindow(((Appointments)AppointmentsList.SelectedItems[0]).PatientId);
                viewPatientProfileWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                viewPatientProfileWindow.ShowDialog();
            }
        }

        private void ButtonNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            CreateAppointmentWindow createAppointmentWindow = new CreateAppointmentWindow();
            createAppointmentWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            createAppointmentWindow.Show();
            this.Close();
        }

        private void ButtonDeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Deletion", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string queryString = "DELETE FROM " + DatabaseConstants.PATIENT_APPOINTMENTS_TABLE + " " +
                    "WHERE AppointmentId=@appointmentId";

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                    SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@appointmentId", ((Appointments)AppointmentsList.SelectedItems[0]).AppointmentId));
                    connection.Open();

                    int v = sqlCommand.ExecuteNonQuery();
                }

                // Reset the appointments list
                appointments.Clear();
                AppointmentsList.ItemsSource = null;
                AppointmentView();
            }
        }
    }
}
