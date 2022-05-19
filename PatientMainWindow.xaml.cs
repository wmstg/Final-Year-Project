using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for PatientMainWindow.xaml
    /// </summary>
    public partial class PatientMainWindow : Window
    {
        public PatientMainWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.PATIENT);
            //CurrentTimeTxt.Content = ((DateTime)DateTime.Now).ToString("f", DateTimeFormatInfo.InvariantInfo);
            NextAppointment();

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void ViewPrescriptions_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionHistoryWindow prescriptionHistoryWindow = new PrescriptionHistoryWindow();
            prescriptionHistoryWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            prescriptionHistoryWindow.Show();
            this.Close();
        }

        private void BookAppointment_Click(object sender, RoutedEventArgs e)
        {
            BookAppointmentWindow bookAppointmentWindow = new BookAppointmentWindow();
            bookAppointmentWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bookAppointmentWindow.Show();
            this.Close();
        }

        private void ViewMedical_Click(object sender, RoutedEventArgs e)
        {
            MedicalHistoryWindow medicalHistoryWindow = new MedicalHistoryWindow();
            medicalHistoryWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            medicalHistoryWindow.Show();
            this.Close();
        }

        private void NextAppointment()
        {
            string appointmentDate = "";

            Patient patient = new Patient();
            patient.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
            patient.GetUserData();

            String queryString = "SELECT TOP 1 AppointmentDate FROM [dbo].[" + DatabaseConstants.PATIENT_APPOINTMENTS_TABLE + "] " +
                "WHERE PatientId=@id AND AppointmentDate >= @date " +
                "ORDER BY AppointmentDate Desc ";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@id", patient.PatientId));
                sqlCommand.Parameters.Add(new SqlParameter("@date", DateTime.Now));
                connection.Open();

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                Object[] roleRights = new Object[sqlDataReader.FieldCount];

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        appointmentDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("AppointmentDate")).ToString();
                    }
                }
                else
                {
                    appointmentDate = "You have no appointments";
                }
                connection.Close();
            }

            TextBlockNextAppointment.Text = String.Format("Your next appointment is: {0}", appointmentDate);
        }
    }
}
