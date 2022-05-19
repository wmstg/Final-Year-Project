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
    /// Interaction logic for ManageSettingsWindow.xaml
    /// </summary>
    public partial class ManageSettingsWindow : Window
    {
        public ManageSettingsWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            SetApplicationSettings();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void SubmitFormBtn_Click(object sender, RoutedEventArgs e)
        {
            int errorCnt = 0;

            if (TextBoxAppointmentDuration.Text.Length > 0 && TextBoxPracticeName.Text.Length > 0 &&
                TextBoxInactiveTimeout.Text.Length > 0)
            {
                int appointmentDuration, inactiveTimeout;
                if (int.TryParse(TextBoxAppointmentDuration.Text, out appointmentDuration) && int.TryParse(TextBoxInactiveTimeout.Text, out inactiveTimeout))
                {
                    Properties.Settings.Default.inactiveTimeout = inactiveTimeout;
                    Properties.Settings.Default.appointmentDuration = appointmentDuration;
                }
                else
                {
                    errorCnt++;
                }
                Properties.Settings.Default.practiceName = TextBoxPracticeName.Text;
                Properties.Settings.Default.Save();
                
            }
            else
            {
                errorCnt++;
            }
            
            if (errorCnt > 0)
            {
                MessageBox.Show("Not all settings have been updated. Please check the correct values are provided, all fields are required.");
            }
            else
            {
                MessageBox.Show("Settings have been updated.");
            }
        }

        private void SetApplicationSettings()
        {
            TextBoxAppointmentDuration.Text = Properties.Settings.Default.appointmentDuration.ToString();
            TextBoxInactiveTimeout.Text = Properties.Settings.Default.inactiveTimeout.ToString();
            TextBoxPracticeName.Text = Properties.Settings.Default.practiceName;
        }
    }
}
