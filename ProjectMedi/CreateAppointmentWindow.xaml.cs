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
    /// Interaction logic for CreateAppointmentWindow.xaml
    /// </summary>
    public partial class CreateAppointmentWindow : Window
    {
        List<Patient> patient = new List<Patient>();

        public CreateAppointmentWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            GetAllPatients();

            SetAppointmentTime();
            SetConsultants();

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);


            PatientComboBox.DisplayMemberPath = "NameDetail";
            PatientComboBox.SelectedValuePath = "PatientId";

            PatientComboBox.ItemsSource = patient;
        }

        private void GetAllPatients()
        {
            String queryString = "SELECT UserId FROM [dbo].[" + DatabaseConstants.PATIENTS_TABLE + "] ";

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
                        Patient rPatient = new Patient();
                        rPatient.UserId = Convert.ToInt32(sqlDataReader.GetValue(sqlDataReader.GetOrdinal(DatabaseConstants.USER_ID)));
                        rPatient.GetUserData();

                        patient.Add(rPatient);
                    }
                }
            }
        }

        private void SetAppointmentTime()
        {
            DateTime dateTimeStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00);
            DateTime dateTimeEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00);

            do
            {
                TimeComboBox.Items.Add(dateTimeStart.ToString("HH:mm"));
            }
            while ((dateTimeStart = dateTimeStart.AddMinutes(15)) <= dateTimeEnd);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void SetConsultants()
        {
            DoctorComboBox.DisplayMemberPath = "Name";
            DoctorComboBox.SelectedValuePath = "StaffId";

            string queryString = "SELECT " + DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.STAFF_ID + "," +
                DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.USER_ID + ", " + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.FIRSTNAME + "," +
                DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.LASTNAME + "," + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.EMAIL_ADDRESS +
                " FROM [dbo]." + DatabaseConstants.STAFF_TABLE +
                " LEFT JOIN [dbo]." + DatabaseConstants.USERS_TABLE + " ON " + DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.USER_ID + "=" + DatabaseConstants.USERS_TABLE + "." + DatabaseConstants.USER_ID +
                " WHERE " + DatabaseConstants.STAFF_TABLE + "." + DatabaseConstants.ROLE_TYPE + "='Doctor'";

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

                        DoctorComboBox.Items.Add(new Doctor()
                        {
                            Name = "Dr " + sqlDataReader.GetString(sqlDataReader.GetOrdinal(DatabaseConstants.LASTNAME)),
                            StaffId = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal(DatabaseConstants.STAFF_ID))
                        });
                    }
                }
            }
        }

        private void ButtonBookAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (!ScheduleCalender.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a valid date.");
            }
            else if (PatientComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("A patient needs to be selected.");
            }
            else if (DoctorComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("A doctor needs to be selected.");
            }
            else if (TimeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a valid time.");
            }
            else
            {
                DateTime appointmentTime = DateTime.ParseExact(ScheduleCalender.SelectedDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null).Add(TimeSpan.Parse(TimeComboBox.Text));

                String queryString = "INSERT INTO " + DatabaseConstants.PATIENT_APPOINTMENTS_TABLE + " (" + DatabaseConstants.PATIENT_ID + ", " + DatabaseConstants.APPOINTMENT_DATE + ", " + DatabaseConstants.DURATION + ", " +
                    DatabaseConstants.CONSULTANT_ID + ", " + DatabaseConstants.ATTENDED + ") VALUES (@param1, @param2, @param3, @param4, @param5)";

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                    SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@param1", PatientComboBox.SelectedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("@param2", appointmentTime));
                    sqlCommand.Parameters.Add(new SqlParameter("@param3", "15"));
                    sqlCommand.Parameters.Add(new SqlParameter("@param4", DoctorComboBox.SelectedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("@param5", false));

                    try
                    {
                        connection.Open();
                        if (sqlCommand.ExecuteNonQuery() > 0)
                        {
                            // Clears and reloads the source data
                            //DataGridConsultationHistory.ItemsSource = null;
                            //GetConsultationHistory(patient.PatientId);
                            MessageBox.Show("The appointment has been booked.", "Success");
                        }
                    }
                    catch (SqlException err)
                    {
                        MessageBox.Show(err.Message.ToString(), "SQL Error");
                    }
                }
            }


        }
    }
}
