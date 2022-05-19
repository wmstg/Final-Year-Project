using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class UserAccessControl
    {
        public static void StaffHomeScreen(string requiredRole)
        {
            switch (requiredRole)
            {
                case "Administrator":
                    AdministratorMainWindow administratorMainWindow = new AdministratorMainWindow();
                    administratorMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    administratorMainWindow.Show();
                    break;

                case "Secretary":
                    SecretaryMainWindow secretaryMainWindow = new SecretaryMainWindow();
                    secretaryMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    secretaryMainWindow.Show();
                    break;

                case "Doctor":
                    DoctorsMainWindow doctorsMainWindow = new DoctorsMainWindow();
                    doctorsMainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    doctorsMainWindow.Show();
                    break;
            }
        }

        public static void MainViewControl(System.Windows.Window window)
        {
            Staff staff = new Staff();
            staff.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
            staff.GetUserData();
            UserAccessControl.StaffHomeScreen(staff.RoleType);

            window.Close();
        }
    }
}
