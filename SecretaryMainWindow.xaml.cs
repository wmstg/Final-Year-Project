using System;
using System.Collections.Generic;
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
    /// Interaction logic for SecretaryMainWindow.xaml
    /// </summary>
    public partial class SecretaryMainWindow : Window
    {
        public SecretaryMainWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private void SearchPatients_Click(object sender, RoutedEventArgs e)
        {
            SearchPatientsWindow searchPatientsWindow = new SearchPatientsWindow();
            searchPatientsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            searchPatientsWindow.Show();
            Close();
        }

        private void RegisterPatients_Click(object sender, RoutedEventArgs e)
        {
            RegisterPatientWindow registerPatientWindow = new RegisterPatientWindow();
            registerPatientWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            registerPatientWindow.Show();
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void ManageAppointments_Click(object sender, RoutedEventArgs e)
        {
            ManageAppointmentsWindow manageAppointmentsWindow = new ManageAppointmentsWindow();
            manageAppointmentsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageAppointmentsWindow.Show();
            Close();
        }
    }
}
