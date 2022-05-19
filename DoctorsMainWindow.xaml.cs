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
    /// Interaction logic for DoctorsMainWindow.xaml
    /// </summary>
    public partial class DoctorsMainWindow : Window
    {
        List<Appointments> appointments = new List<Appointments>();
        Staff staff = Staff.GetSessionStaff();

        public DoctorsMainWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);

            AppointmentView();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void SearchPatients_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetAppointments()
        {
            String queryString = "SELECT * FROM [dbo].[" + DatabaseConstants.PATIENT_APPOINTMENTS_TABLE + "] " +
                "WHERE ConsultantId = @id AND AppointmentDate > @date";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@id", staff.StaffId));
                sqlCommand.Parameters.Add(new SqlParameter("@date", DateTime.Now));
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                Object[] roleRights = new Object[sqlDataReader.FieldCount];

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
            //DataGridUpcomingAppointments.DataContext = appointments;
            DataGridUpcomingAppointments.ItemsSource = appointments;
        }

        private void ButtonStartConsultation_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUpcomingAppointments.SelectedIndex != -1)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Start a consultation with the selected patient?", "Start Consultation", MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ConsultationWindow consultationWindow = new ConsultationWindow(appointments[DataGridUpcomingAppointments.SelectedIndex].PatientId);
                    consultationWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    consultationWindow.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("It is not possible to start a consultation without a selected appointment.");
            }
        }
    }
}
