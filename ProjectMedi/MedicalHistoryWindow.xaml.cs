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
    /// Interaction logic for MedicalHistory.xaml
    /// </summary>
    public partial class MedicalHistoryWindow : Window
    {
        public MedicalHistoryWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            Patient patient = new Patient();
            patient.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
            patient.GetUserData();

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.PATIENT);

            GetMedicalHistory(patient.PatientId);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            PatientMainWindow patientMainWindow = new PatientMainWindow();
            patientMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            patientMainWindow.Show();
            this.Close();
        }

        private void GetMedicalHistory(int patientId)
        {
            List<String> medicalHistory = new List<String>();

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
                        medicalHistory.Add(String.Format("{0}\n{1}\n{2}", sqlDataReader.GetString(1), sqlDataReader.GetString(0), sqlDataReader.GetString(2)).ToString());
                    }
                }
                else
                {
                    medicalHistory.Add(String.Format("There are no records to display"));
                }
                connection.Close();
            }
            DataGridMedicalHistory.DataContext = medicalHistory;
        }

    }
}
