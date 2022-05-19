using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace ProjectMedi
{
    /// <summary>
    /// Interaction logic for ViewPatientProfile.xaml
    /// </summary>
    public partial class ViewPatientProfileWindow : Window
    {
        private Patient patient;

        public ViewPatientProfileWindow()
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
        }

        public ViewPatientProfileWindow(int patientId)
        {
            InitializeComponent();
            Utility.SetWindowTitle(this);
            SetUpEnvironment(patientId);
        }

        private void ButtonSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients first name");
            }
            else if (LastName.Text.Length == 0)
            {
                MessageBox.Show("Please enter the patients surname");
            }
            /*else if ((bool)GenderRadioButton.IsChecked)
            {
                MessageBox.Show("Please select the patients gender");
            }*/
            else if (ContactNumber.Text.Length == 0)
            {
                MessageBox.Show("Please enter a contact number");
            }
            else if (EmailAddress.Text.Length == 0)
            {
                MessageBox.Show("Please enter a valid email address");
            }
            else
            {
                UpdatePatientData();
            }
        }

        private void SetUpEnvironment(int patientId)
        {
            patient = new Patient();
            patient.PatientId = patientId;
            patient.GetUIDFromPatientId();
            patient.GetUserData();
            patient.GetAddressData();

            LabelPatientName.Content = "Viewing Profile: " + patient.LastName + ", " + patient.FirstName;
            FirstName.Text = patient.FirstName;
            LastName.Text = patient.LastName;

            if (patient.Gender == User.GENDER_MALE) { GenderMale.IsChecked = true; }
            else if (patient.Gender == User.GENDER_FEMALE) { GenderFemale.IsChecked = true; }
            else if (patient.Gender == User.GENDER_OTHER) { GenderOther.IsChecked = true; }

            Nationality.Text = patient.Nationality;
            ContactNumber.Text = patient.ContactNumber;
            EmailAddress.Text = patient.EmailAddress;

            TextBoxAddressLine1.Text = patient.Addresses[0].AddressLine1;
            TextBoxAddressLine2.Text = patient.Addresses[0].AddressLine2;
            TextBoxCity.Text = patient.Addresses[0].City;
            TextBoxPostcode.Text = patient.Addresses[0].PostCode;

            //DataGridAddresses.ItemsSource = patient.Addresses;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GenderRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void UpdatePatientData()
        {
            patient.FirstName = FirstName.Text;
            patient.LastName = LastName.Text;
            patient.ContactNumber = ContactNumber.Text;
            patient.EmailAddress = EmailAddress.Text;

            patient.UpdateData();

            if (TextBoxAddressLine1.Text != patient.Addresses[0].AddressLine1 || TextBoxPostcode.Text != patient.Addresses[0].PostCode || TextBoxCity.Text != patient.Addresses[0].City) {
                User.Address.UpdateAddress(patient.UserId, new string[] { TextBoxAddressLine1.Text , TextBoxAddressLine2.Text, TextBoxPostcode.Text, TextBoxCity.Text  });
            }

                //TODO ~ Finish Update (Maybe update method in Patient and Staff class) ~ Or Leave it
                //if (res > 0)
                //{
            MessageBox.Show("The patients data has been updated.");
                //}
            //}
        }
    }
}
