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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectMedi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*if (Properties.Settings.Default.currentUserId.Length == 0 || Properties.Settings.Default.currentUserId == "0")
            {*/
                LoginWindow login = new LoginWindow();
                login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                login.Show();
                Close();
            //}
        }
    }
}
