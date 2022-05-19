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
    /// Interaction logic for PrescriptionHistoryWindow.xaml
    /// </summary>
    public partial class PrescriptionHistoryWindow : Window
    {
        public PrescriptionHistoryWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            Patient patient = new Patient();
            patient.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
            patient.GetUserData();

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.PATIENT);

            GetPrescriptionHistory(patient.PatientId);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            PatientMainWindow patientMainWindow = new PatientMainWindow();
            patientMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            patientMainWindow.Show();
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void GetPrescriptionHistory(int patientId)
        {
            List<Prescriptions> prescriptionHistory = new List<Prescriptions>();

            if (patientId == 0)
            {
                return;
            }

            String queryString = "SELECT * FROM " + DatabaseConstants.PRESCRIPTIONS_TABLE + " " +
                "WHERE " + DatabaseConstants.PATIENT_ID + "=@param1 " +
                "ORDER BY PrescriptionId DESC";
            Console.WriteLine(queryString);
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
                    MessageBox.Show("There is no data to display");
                }
                connection.Close();
            }

            DataGridPrescriptionHistory.ItemsSource = prescriptionHistory;
        }
    }
}
