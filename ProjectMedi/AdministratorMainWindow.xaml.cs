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
    /// Interaction logic for AdministratorMainWindow.xaml
    /// </summary>
    public partial class AdministratorMainWindow : Window
    {
        public AdministratorMainWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);

            Staff staff = new Staff();
            staff.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
            staff.GetUserData();

            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void RegisterPatients_Click(object sender, RoutedEventArgs e)
        {
            RegisterPatientWindow registerPatientWindow = new RegisterPatientWindow();
            registerPatientWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            registerPatientWindow.Show();
            Close();
        }

        private void SearchPatients_Click(object sender, RoutedEventArgs e)
        {
            SearchPatientsWindow searchPatientsWindow = new SearchPatientsWindow();
            searchPatientsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            searchPatientsWindow.Show();
            Close();
        }

        private void ManageStaff_Click(object sender, RoutedEventArgs e)
        {
            ManageStaffWindow manageStaffWindow = new ManageStaffWindow();
            manageStaffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageStaffWindow.Show();
            this.Close();
        }

        private void ManageSettings_Click(object sender, RoutedEventArgs e)
        {
            ManageSettingsWindow manageSettingsWindow = new ManageSettingsWindow();
            manageSettingsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageSettingsWindow.Show();
            this.Close();
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
