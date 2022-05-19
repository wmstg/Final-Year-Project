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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        String salt = Authentication.GenerateSalt();
        public LoginWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            ApplicationTitle.Content = Properties.Settings.Default.practiceName;
        }

        /**
         * Removes the default text from user id textbox
         *
         */
        private void RemoveDefault(object sender, RoutedEventArgs e)
        {
            if (userIdInput.Text == "User Id")
            {
                userIdInput.Text = "";
            }
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, RoutedEventArgs e)
        {
            String loginId = userIdInput.Text;
            String password = passwordInput.Password;

            if (loginId.Length > 0 && password.Length > 0)
            {
                // TODO
                Authentication auth = new Authentication();
                if (auth.Login(loginId, password))
                {
                    // Direct to correct screen
                    /*if user is a patient direct to patient dashboard.
                     * Otherwise determine staff opposed to administrator dash
                     */
                    Patient patient = new Patient();
                    patient.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);

                    if (patient.IsPatient())
                    {
                        PatientMainWindow patientMainWindow = new PatientMainWindow();
                        patientMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        patientMainWindow.Show();
                    }
                    else // Staff member
                    {
                        // Determine the staff user type
                        Staff staff = new Staff();
                        staff.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
                        staff.GetUserData();

                        if (staff.RoleType == "Administrator")
                        {
                            AdministratorMainWindow administratorMainWindow = new AdministratorMainWindow();
                            administratorMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            administratorMainWindow.Show();
                        }
                        else if (staff.RoleType == "Secretary")
                        {
                            SecretaryMainWindow secretaryMainWindow = new SecretaryMainWindow();
                            secretaryMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            secretaryMainWindow.Show();
                        }
                        else
                        {
                            DoctorsMainWindow doctorsMainWindow = new DoctorsMainWindow();
                            doctorsMainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            doctorsMainWindow.Show();
                        }
                        
                    }
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Username or password is invalid");

            }
        }
    }
}
