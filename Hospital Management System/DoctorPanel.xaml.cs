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
    /// Interaction logic for DoctorPanel.xaml
    /// </summary>
    public partial class DoctorPanel : Window
    {
        public DoctorPanel()
        {
            InitializeComponent();
        }

        private void LoadAppointments()
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_appoinment = new SqlCommand("SELECT PatientNameSurname,AppointmentID,AppointmentPatient ,AppointmentDoctor,AppointmentDay ,AppointmentHour FROM TableAppointment INNER JOIN TablePatient ON PatientTC=AppointmentPatient WHERE AppointmentDay=@pday ORDER BY AppointmentHour ASC", MyConnection.connection);
            command_get_appoinment.Parameters.AddWithValue("@pday", DateTime.Today.Date);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_get_appoinment);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            datagrid.ItemsSource = dataTable.DefaultView;

            datagrid.Columns[1].Visibility = Visibility.Hidden;
            datagrid.Columns[2].Visibility = Visibility.Hidden;
            datagrid.Columns[3].Visibility = Visibility.Hidden;
            datagrid.Columns[4].Visibility = Visibility.Hidden;

            // Ekrandaki datagride tıklandığında AppointmentID değerini seçin.

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAppointments();
        }

        // Metot açın doktorun id'si girilip sonuç olarak doktorun bölümünü alsın.
        private int GetClinicByID(int id)
        {
            MyConnection.CheckConnection();
            SqlCommand command_GetClinicByID = new SqlCommand("SELECT DoctorClinicID FROM TableDoctor WHERE DoctorID=@pid",MyConnection.connection);
            command_GetClinicByID.Parameters.AddWithValue("@pid", id);
            SqlDataReader dr = command_GetClinicByID.ExecuteReader();
            int selected_clinic_id = 0;
            while (dr.Read())
            {
                selected_clinic_id= Convert.ToInt32(dr[0]);
            }
            return selected_clinic_id;
        }


        int selected_appointment_id = 0;
        int selected_doctor_id = 0;
        int selected_doctor_clinic_id = 0;
        string selected_appointment_patient = "";
        string selected_appointment_patient_tc = "";
        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_appointment_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 :Convert.ToInt32((datagrid.SelectedItem as DataRowView)["AppointmentID"]);
            selected_appointment_patient = ((datagrid.SelectedItem as DataRowView) == null) ? "" :(datagrid.SelectedItem as DataRowView)["PatientNameSurname"].ToString();
            selected_appointment_patient_tc = ((datagrid.SelectedItem as DataRowView) == null) ? "" :(datagrid.SelectedItem as DataRowView)["AppointmentPatient"].ToString();
            selected_doctor_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["AppointmentDoctor"]);
            selected_doctor_clinic_id = GetClinicByID(selected_doctor_id);
            tboxSelectedPatient.Text=selected_appointment_patient;
        }

        private void btnDeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_delete_appointment = new SqlCommand("DELETE FROM TableAppointment WHERE AppointmentID =@pid",MyConnection.connection);
            command_delete_appointment.Parameters.AddWithValue("@pid",selected_appointment_id);
            command_delete_appointment.ExecuteNonQuery();
            LoadAppointments();
        }
    }
}
