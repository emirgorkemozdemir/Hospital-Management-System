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
    /// Interaction logic for ClinicOperations.xaml
    /// </summary>
    public partial class ClinicOperations : Window
    {
        public ClinicOperations()
        {
            InitializeComponent();
        }

        private void GetClinics()
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_clinics = new SqlCommand("SELECT * FROM TableClinic", MyConnection.connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_get_clinics);
            DataTable d_table = new DataTable();
            dataAdapter.Fill(d_table);
            datagrid.ItemsSource = d_table.DefaultView;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetClinics();
        }

        private void btnAddClinic_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_add_clinic = new SqlCommand("INSERT INTO TableClinic (ClinicName) VALUES (@pname)",MyConnection.connection);
            command_add_clinic.Parameters.AddWithValue("@pname", tboxClinicName.Text);
            command_add_clinic.ExecuteNonQuery();
            tboxClinicName.Text = "";
            GetClinics();
        }

        private string selected_clinic_name = "";
        private int selected_clinic_id = 0;
        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_clinic_id = Convert.ToInt32((datagrid.SelectedItem as DataRowView)["ClinicID"]);
            selected_clinic_name = (datagrid.SelectedItem as DataRowView)["ClinicName"].ToString();
            tboxDeleteClinicName.Text = selected_clinic_name;
        }

        private void btnDeleteClinic_Click(object sender, RoutedEventArgs e)
        {
            // Buraya delete kısmını yazacağız.
        }
    }
}
