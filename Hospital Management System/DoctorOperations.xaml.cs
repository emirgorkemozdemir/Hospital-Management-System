using Hospital_Management_System.Classes;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Hospital_Management_System
{
    /// <summary>
    /// Interaction logic for DoctorOperations.xaml
    /// </summary>
    public partial class DoctorOperations : Window
    {
        public DoctorOperations()
        {
            InitializeComponent();
        }

        private void GetDoctors()
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_doctors = new SqlCommand("SELECT DoctorID,DoctorNameSurname,ClinicName FROM TableDoctor INNER JOIN TableClinic ON ClinicID=DoctorClinicID", MyConnection.connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_get_doctors);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            datagrid.ItemsSource = dataTable.DefaultView;
        }

        // GetClinics isimli bir metot açın. Bu metodun sonucunda veritabanından
        // clinicleri getirin. Bunları comboboxa yazdırın.
        private void GetClinics()
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_clinics = new SqlCommand("SELECT * FROM TableClinic", MyConnection.connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_get_clinics);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            cmbAddClinic.ItemsSource = dataTable.DefaultView;

            // Display member gözüken değer için
            cmbAddClinic.DisplayMemberPath = "ClinicName";

            // Selected value path arkada çalışan değer için
            cmbAddClinic.SelectedValuePath = "ClinicID";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetDoctors();
            GetClinics();
        }

        private void btnAddDoctor_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
