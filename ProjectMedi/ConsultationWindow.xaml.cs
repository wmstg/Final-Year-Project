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
    /// Interaction logic for ConsultationWindow.xaml
    /// </summary>
    public partial class ConsultationWindow : Window
    {
        Patient patient;

        public ConsultationWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
        }

        public ConsultationWindow(int userId)
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            SetUpEnvironment(userId);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void GenderRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SetUpEnvironment(int patientId)
        {
            patient = new Patient();
            patient.PatientId = patientId;
            patient.GetUIDFromPatientId();
            patient.GetUserData();
            patient.GetAddressData();

            LabelPatientName.Content = patient.LastName + ", " + patient.FirstName;
            FirstName.Text = patient.FirstName;
            LastName.Text = patient.LastName;

            if (patient.Gender == User.GENDER_MALE) { GenderMale.IsChecked = true; }
            else if (patient.Gender == User.GENDER_FEMALE) { GenderFemale.IsChecked = true; }
            else if (patient.Gender == User.GENDER_OTHER) { GenderOther.IsChecked = true; }

            Nationality.Text = patient.Nationality;
            ContactNumber.Text = patient.ContactNumber;
            EmailAddress.Text = patient.EmailAddress;

            try
            {
                TextBoxAddressLine1.Text = patient.Addresses[0].AddressLine1;
                TextBoxAddressLine2.Text = patient.Addresses[0].AddressLine2;
                TextBoxCity.Text = patient.Addresses[0].City;
                TextBoxPostcode.Text = patient.Addresses[0].PostCode;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(String.Format("Address information for patient {0} ({1}, {2}) is missing.", patient.PatientId, patient.FirstName, patient.LastName));
            }

            DataGridAddresses.ItemsSource = patient.Addresses;

            GetPrescriptionHistory(patient.PatientId);
            GetConsultationHistory(patient.PatientId);
            
        }

        private void ButtonNewPrescription_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPrescriptionTitle.Text.Length > 0 && TextBoxPrescriptionDose.Text.Length > 0 &&
                TextBoxPrescriptionDuration.Text.Length > 0 && TextBoxPrescriptionNotes.Text.Length > 0)
            {
                String queryString = "INSERT INTO " + DatabaseConstants.PRESCRIPTIONS_TABLE + " (PatientId, BeginDate, EndDate, Quantity, Dosage, Title, Notes) " +
                    "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7)";
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                    SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@param1", patient.PatientId));
                    sqlCommand.Parameters.Add(new SqlParameter("@param2", DatePickerBegin.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param3", DatePickerEnd.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param4", TextBoxPrescriptionQuantity.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param5", TextBoxPrescriptionDose.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param6", TextBoxPrescriptionTitle.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param7", TextBoxPrescriptionNotes.Text));

                    try
                    {
                        connection.Open();
                        if (sqlCommand.ExecuteNonQuery()>0)
                        {
                            // Clears and reloads the source data
                            DataGridPrescriptionHistory.ItemsSource = null;
                            GetPrescriptionHistory(patient.PatientId);
                            MessageBox.Show("A new prescription has been added.", "Success");
                        }
                    }
                    catch(SqlException err)
                    {
                        MessageBox.Show(err.Message.ToString(), "SQL Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please fill in all fields for the new prescription form.");
            }
        }

        private void GetPrescriptionHistory(int patientId)
        {
            List<Prescriptions> prescriptionHistory = new List<Prescriptions>();

            if (patientId == 0)
            {
                MessageBox.Show("Testing");
                return;
            }

            String queryString = "SELECT * FROM " + DatabaseConstants.PRESCRIPTIONS_TABLE + " " +
                "WHERE " + DatabaseConstants.PATIENT_ID + "=@param1 " +
                "ORDER BY PrescriptionId DESC";
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@param1", patientId));

                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        prescriptionHistory.Add(new Prescriptions()
                        {
                            PrescriptionId = sqlDataReader.GetInt32(0),
                            PatientId = sqlDataReader.GetInt32(1),
                            BeginDate = sqlDataReader.GetString(2),
                            EndDate = sqlDataReader.GetString(3),
                            Quantity = sqlDataReader.GetInt32(4),
                            Dosage = sqlDataReader.GetString(5),
                            Title = sqlDataReader.GetString(6),
                            Notes = sqlDataReader.GetString(7)
                        });
                    }
                }
                else
                {
                    MessageBox.Show("There is no prescription data to display");
                }
                connection.Close();
            }

            DataGridPrescriptionHistory.ItemsSource = prescriptionHistory;
        }

        private void ButtonSubmitConsultation_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxConsultation.Text.Length > 0 )
            {
                String queryString = "INSERT INTO " + DatabaseConstants.CONSULTATIONS_TABLE + " (" + DatabaseConstants.PATIENT_ID + ", " + DatabaseConstants.CONTENT + ", " + DatabaseConstants.CONSULTATION_DATE + ", " +
                    DatabaseConstants.CONSULTANT_ID + ", " + DatabaseConstants.CONSULTANT_NAME + ", " + DatabaseConstants.CONSULTANT_PRACTICE + ") VALUES (@param1, @param2, @param3, @param4, @param5, @param6)";

                using (SqlConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                    SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@param1", patient.PatientId));
                    sqlCommand.Parameters.Add(new SqlParameter("@param2", TextBoxConsultation.Text));
                    sqlCommand.Parameters.Add(new SqlParameter("@param3", DateTime.Now.ToShortDateString()));
                    sqlCommand.Parameters.Add(new SqlParameter("@param4", Staff.GetSessionStaff().StaffId));
                    sqlCommand.Parameters.Add(new SqlParameter("@param5", "Dr " + Staff.GetSessionStaff().LastName));
                    sqlCommand.Parameters.Add(new SqlParameter("@param6", "Practice Name"));

                    try
                    {
                        connection.Open();
                        if (sqlCommand.ExecuteNonQuery() > 0)
                        {
                            // Clears and reloads the source data
                            DataGridConsultationHistory.ItemsSource = null;
                            GetConsultationHistory(patient.PatientId);
                            MessageBox.Show("The consultation has been saved.", "Success");
                        }
                    }
                    catch (SqlException err)
                    {
                        MessageBox.Show(err.Message.ToString(), "SQL Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please fill in the consultation form.");
            }
        }

        /// <summary>
        /// Gets the patients consultation history
        /// </summary>
        /// <param name="patientId"></param>
        private void GetConsultationHistory(int patientId)
        {
            List<String> consultationHistory = new List<String>();

            if (patientId == 0)
            {
                return;
            }

            String queryString = "SELECT " + DatabaseConstants.CONTENT + ", " + DatabaseConstants.CONSULTATION_DATE + ", " + DatabaseConstants.CONSULTANT_NAME + ", " + DatabaseConstants.CONSULTANT_PRACTICE + " " +
                "FROM " + DatabaseConstants.CONSULTATIONS_TABLE + " " +
                "WHERE " + DatabaseConstants.PATIENT_ID + "=@param1 " +
                "ORDER BY Id DESC";
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlServer"].ToString();

                SqlCommand sqlCommand = new SqlCommand(queryString, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@param1", patientId));

                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        consultationHistory.Add(String.Format("{0}\n{1}\n{2}", sqlDataReader.GetString(1), sqlDataReader.GetString(0), sqlDataReader.GetString(2)).ToString());
                    }
                }
                else
                {
                    consultationHistory.Add(String.Format("There are no records to display"));
                }
                connection.Close();
            }
            DataGridConsultationHistory.DataContext = consultationHistory;
            DataGridConsultationHistory.ItemsSource = consultationHistory;
        }

    }
}
