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
    /// Interaction logic for PatientOperations.xaml
    /// </summary>
    public partial class PatientOperations : Window
    {
        public PatientOperations()
        {
            InitializeComponent();
        }

        private void GetPatients()
        {
            MyConnection.CheckConnection();
            SqlCommand command_select = new SqlCommand("SELECT * FROM TablePatient ORDER BY PatientLastActive DESC",MyConnection.connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_select);
            DataTable d_Table = new DataTable();
            dataAdapter.Fill(d_Table);
            datagrid.ItemsSource = d_Table.DefaultView;
        }

        private void LoadComboBoxes()
        {
            cmbBlood.Items.Add("A RH+");
            cmbBlood.Items.Add("A RH-");
            cmbBlood.Items.Add("B RH+");
            cmbBlood.Items.Add("B RH-");
            cmbBlood.Items.Add("AB RH+");
            cmbBlood.Items.Add("AB RH-");
            cmbBlood.Items.Add("O RH+");
            cmbBlood.Items.Add("O RH-");

            cmbBlood.SelectedIndex = 0;

            cmbGender.Items.Add("E");
            cmbGender.Items.Add("K");

            cmbGender.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetPatients();
            LoadComboBoxes();
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_add = new SqlCommand("INSERT INTO TablePatient (PatientTC,PatientNameSurname,PatientBirthDate,PatientGender,PatientPhone,PatientBloodType) VALUES (@ptc,@pname,@pbdate,@pgender,@pphone,@pblood)",MyConnection.connection);
            command_add.Parameters.AddWithValue("@ptc",tboxTC.Text);
            command_add.Parameters.AddWithValue("@pname",tboxNameSurname.Text);
            command_add.Parameters.AddWithValue("@pbdate",datepicker.SelectedDate);
            command_add.Parameters.AddWithValue("@pgender",cmbGender.SelectedValue);
            command_add.Parameters.AddWithValue("@pblood",cmbBlood.SelectedValue);
            command_add.Parameters.AddWithValue("@pphone",tboxPhone.Text);
            command_add.ExecuteNonQuery();
            GetPatients();
            LoadComboBoxes();
            tboxPhone.Text = "";
            tboxNameSurname.Text = "";
            tboxTC.Text = "";
        }
    }
}
