using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectMedi
{
    public sealed class Utility
    {
        public enum UI_END
        {
            PATIENT,
            STAFF
        }

        public static String PasswordGenerator()
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$%&#{}[]";
            char[] generatedPassword = new char[10];
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                generatedPassword[i] = characters[random.Next(characters.Length)];
            }

            return new String(generatedPassword);
        }

        public static int SaveUserAddressData()
        {
            return 1;
        }

        /// <summary>
        /// Displays the firstname of the logged in user
        /// </summary>
        /// <param name="uI_END"></param>
        /// <returns></returns>
        public static String UIUserNameDisplay(UI_END uI_END)
        {
            if (uI_END == UI_END.PATIENT)
            {
                Patient patient = new Patient();
                patient.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
                patient.GetUserData();

                return "Welcome " + patient.FirstName;
            }
            else
            {
                Staff staff = new Staff();
                staff.UserId = Int32.Parse(Properties.Settings.Default.currentUserId);
                staff.GetUserData();

                return "Welcome " + staff.FirstName;
            }
        }

        public static void SetWindowTitle(Window window)
        {
            window.Title = window.Title + " :: " + Properties.Settings.Default.practiceName;
        }
    }
}
