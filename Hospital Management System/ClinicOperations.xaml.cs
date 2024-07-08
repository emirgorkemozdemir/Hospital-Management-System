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
            datagrid.Columns[0].Visibility = Visibility.Hidden;
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
            selected_clinic_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["ClinicID"]);
            selected_clinic_name = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["ClinicName"].ToString();
            tboxDeleteClinicName.Text = selected_clinic_name;
            tboxUpdateClinicName.Text = selected_clinic_name;
        }

        private void btnDeleteClinic_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_delete_clinic = new SqlCommand("DELETE FROM TableClinic WHERE ClinicID=@pid",MyConnection.connection);
            command_delete_clinic.Parameters.AddWithValue("@pid", selected_clinic_id);
            command_delete_clinic.ExecuteNonQuery();
            GetClinics();
        }

        private void btnUpdateClinic_Click(object sender, RoutedEventArgs e)
        {
            // Update kısmını yazınız
            MyConnection.CheckConnection();
            SqlCommand command_update_clinic = new SqlCommand("UPDATE TableClinic SET ClinicName=@pname WHERE ClinicID=@pid",MyConnection.connection);
            command_update_clinic.Parameters.AddWithValue("@pid",selected_clinic_id);
            command_update_clinic.Parameters.AddWithValue("@pname", tboxUpdateClinicName.Text);
            command_update_clinic.ExecuteNonQuery();
            GetClinics();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Main new_window = new Main();
            new_window.Show();
            this.Hide();
        }
    }
}
 