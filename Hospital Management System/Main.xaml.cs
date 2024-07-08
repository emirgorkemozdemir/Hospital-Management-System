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

namespace Hospital_Management_System
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnClinic_Click(object sender, RoutedEventArgs e)
        {
            ClinicOperations new_window = new ClinicOperations();
            new_window.Show();
            this.Hide();
        }

        private void btnDoctor_Click(object sender, RoutedEventArgs e)
        {
            DoctorOperations new_window = new DoctorOperations();
            new_window.Show();
            this.Hide();
        }
    }
}
