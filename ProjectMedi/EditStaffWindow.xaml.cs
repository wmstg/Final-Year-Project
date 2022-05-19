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
    /// Interaction logic for EditStaffWindow.xaml
    /// </summary>
    public partial class EditStaffWindow : Window
    {
        private Staff staff;

        public EditStaffWindow()
        {
            InitializeComponent();
        }

        public EditStaffWindow(int staffId)
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            welcomeTxt.Content = Utility.UIUserNameDisplay(Utility.UI_END.STAFF);
            LabelCurrentDateTime.Content = ((DateTime)DateTime.Now).ToString("f", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            SetUpEnvironment(staffId);
        }

        private void LabelBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ManageStaffWindow manageStaffWindow = new ManageStaffWindow();
            manageStaffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            manageStaffWindow.Show();
            this.Close();
        }

        private void SubmitFormBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members first name");
            }
            else if (LastName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the staff members surname");
            }
            else if (AddressLine1.Text.Length == 0)
            {
                MessageBox.Show("Please enter the first line of address");
            }
            else if (City.Text.Length == 0)
            {
                MessageBox.Show("Please enter the city");
            }
            else if (Postcode.Text.Length == 0)
            {
                MessageBox.Show("Please enter the post code");
            }
            else
            {
                UpdateStaffData();
            }
        }

        private void UpdateStaffData()
        {
            staff.FirstName = FirstName.Text;
            staff.LastName = LastName.Text;
            //staff.ContactNumber = ContactNumber.Text;
            staff.EmailAddress = EmailAddress.Text;

            staff.UpdateData();

            if (AddressLine1.Text != staff.Addresses[0].AddressLine1 || Postcode.Text != staff.Addresses[0].PostCode || City.Text != staff.Addresses[0].City)
            {
                User.Address.UpdateAddress(staff.UserId, new string[] { AddressLine1.Text, AddressLine2.Text, Postcode.Text, City.Text });
            }

            //TODO ~ Finish Update (Maybe update method in Patient and Staff class) ~ Or Leave it
            //if (res > 0)
            //{
            MessageBox.Show("The staff members data has been updated.");
            //}
            //}
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserAccessControl.MainViewControl(this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Authentication.LogOut(this);
        }

        private void ComboBoxStaffType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SetUpEnvironment(int staffId)
        {
            staff = new Staff();
            staff.StaffId = staffId;
            staff.GetUIDFromStaffId();
            staff.GetUserData();
            staff.GetAddressData();

            FirstName.Text = staff.FirstName;
            LastName.Text = staff.LastName;
            LoginName.Text = staff.LoginId;
            

            //ContactNumber.Text = staff.Number;
            EmailAddress.Text = staff.EmailAddress;

            ComboBoxStaffType.SelectedIndex = ComboBoxStaffType.Items.IndexOf(staff.RoleType);

            AddressLine1.Text = staff.Addresses[0].AddressLine1;
            AddressLine2.Text = staff.Addresses[0].AddressLine2;
            City.Text = staff.Addresses[0].City;
            Postcode.Text = staff.Addresses[0].PostCode;

            //DataGridAddresses.ItemsSource = patient.Addresses;
        }


    }
}
