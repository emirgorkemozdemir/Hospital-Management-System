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
            SqlCommand command_select = new SqlCommand("SELECT * FROM TablePatient ORDER BY PatientLastActive DESC", MyConnection.connection);
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

            //cboxHour.Items.Add("7:00");
            //cboxHour.Items.Add("7:10");
            //cboxHour.Items.Add("7:20");
            //cboxHour.Items.Add("7:30");
            //cboxHour.Items.Add("7:40");
            //cboxHour.Items.Add("7:50");
            //cboxHour.Items.Add("8:00");
            //cboxHour.Items.Add("8:10");
            //cboxHour.Items.Add("8:20");
            //cboxHour.Items.Add("8:30");
            //cboxHour.Items.Add("8:40");
            //cboxHour.Items.Add("8:50");
            //cboxHour.Items.Add("9:00");
            //cboxHour.Items.Add("9:10");
            //cboxHour.Items.Add("9:20");
            //cboxHour.Items.Add("9:30");
            //cboxHour.Items.Add("9:40");
            //cboxHour.Items.Add("9:50");
            //cboxHour.Items.Add("10:00");
            //cboxHour.Items.Add("10:10");
            //cboxHour.Items.Add("10:20");
            //cboxHour.Items.Add("10:30");
            //cboxHour.Items.Add("10:40");
            //cboxHour.Items.Add("10:50");
            //cboxHour.Items.Add("11:00");
            //cboxHour.Items.Add("11:10");
            //cboxHour.Items.Add("11:20");
            //cboxHour.Items.Add("11:30");
            //cboxHour.Items.Add("11:40");
            //cboxHour.Items.Add("11:50");
            //cboxHour.Items.Add("12:00");
            //cboxHour.Items.Add("12:10");
            //cboxHour.Items.Add("12:20");
            //cboxHour.Items.Add("12:30");
            //cboxHour.Items.Add("12:40");
            //cboxHour.Items.Add("12:50");
            //cboxHour.Items.Add("1:00 PM");
            //cboxHour.Items.Add("1:10 PM");
            //cboxHour.Items.Add("1:20 PM");
            //cboxHour.Items.Add("1:30 PM");
            //cboxHour.Items.Add("1:40 PM");
            //cboxHour.Items.Add("1:50 PM");
            //cboxHour.Items.Add("2:00 PM");
            //cboxHour.Items.Add("2:10 PM");
            //cboxHour.Items.Add("2:20 PM");
            //cboxHour.Items.Add("2:30 PM");
            //cboxHour.Items.Add("2:40 PM");
            //cboxHour.Items.Add("2:50 PM");
            //cboxHour.Items.Add("3:00 PM");
            //cboxHour.Items.Add("3:10 PM");
            //cboxHour.Items.Add("3:20 PM");
            //cboxHour.Items.Add("3:30 PM");
            //cboxHour.Items.Add("3:40 PM");
            //cboxHour.Items.Add("3:50 PM");
            //cboxHour.Items.Add("4:00 PM");
            //cboxHour.Items.Add("4:10 PM");
            //cboxHour.Items.Add("4:20 PM");
            //cboxHour.Items.Add("4:30 PM");
            //cboxHour.Items.Add("4:40 PM");
            //cboxHour.Items.Add("4:50 PM");
            //cboxHour.Items.Add("5:00 PM");
            //cboxHour.SelectedIndex = 0;
        }

        private List<string> SelectLockedDates()
        {
           List<string> LockedHours = new List<string>();

            MyConnection.CheckConnection();
            SqlCommand command_get_hours = new SqlCommand("SELECT AppointmentHour FROM TableAppointment WHERE AppointmentDoctor=@pdid AND AppointmentDay=@pday", MyConnection.connection);
            command_get_hours.Parameters.AddWithValue("@pdid", cboxDoctors.SelectedValue);
            command_get_hours.Parameters.AddWithValue("@pday", datepickerappointment.SelectedDate.Value.Date);
            SqlDataReader dataReader = command_get_hours.ExecuteReader();
            while (dataReader.Read())
            {
                LockedHours.Add(dataReader[0].ToString());
            }
            dataReader.Close();
            return LockedHours;
        }

        private void SaatleriEkle()
        {
            if (cboxDoctors.SelectedValue == null || datepickerappointment.SelectedDate == null)
            {
                MessageBox.Show("You have to select doctor and date");
            }
            else
            {
                var lockedhours = SelectLockedDates();

                DateTime saat = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 7, 10, 0);
                DateTime aksamSaat = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 17, 0, 0);


                while (saat <= aksamSaat)
                {
                    //MessageBox.Show(saat.Hour.ToString() + ":" + saat.Minute.ToString());
                    if (lockedhours.Contains(saat.Hour.ToString() +":"+ saat.Minute.ToString()))
                    {
                        saat = saat.AddMinutes(10);
                    }
                    else
                    {
                        cboxHour.Items.Add(saat.ToString("hh:mm tt"));
                        saat = saat.AddMinutes(10);
                    }                 
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetPatients();
            LoadComboBoxes();
            GetClinics();
            // Metot yazın , bu metot ile bugünün tarihi öncesindeki tüm tarihleri kilitleyin.
            datepickerappointment.DisplayDateStart = DateTime.Today;
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_add = new SqlCommand("INSERT INTO TablePatient (PatientTC,PatientNameSurname,PatientBirthDate,PatientGender,PatientPhone,PatientBloodType) VALUES (@ptc,@pname,@pbdate,@pgender,@pphone,@pblood)", MyConnection.connection);
            command_add.Parameters.AddWithValue("@ptc", tboxTC.Text);
            command_add.Parameters.AddWithValue("@pname", tboxNameSurname.Text);
            command_add.Parameters.AddWithValue("@pbdate", datepicker.SelectedDate);
            command_add.Parameters.AddWithValue("@pgender", cmbGender.SelectedValue);
            command_add.Parameters.AddWithValue("@pblood", cmbBlood.SelectedValue);
            command_add.Parameters.AddWithValue("@pphone", tboxPhone.Text);
            command_add.ExecuteNonQuery();
            GetPatients();
            LoadComboBoxes();
            tboxPhone.Text = "";
            tboxNameSurname.Text = "";
            tboxTC.Text = "";
        }

        private void GetClinics()
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_clinics = new SqlCommand("SELECT * FROM TableClinic", MyConnection.connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_get_clinics);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            cboxClinics.ItemsSource = dataTable.DefaultView;

            // Display member gözüken değer için
            cboxClinics.DisplayMemberPath = "ClinicName";

            // cboxClinics value path arkada çalışan değer için
            cboxClinics.SelectedValuePath = "ClinicID";
        }

        private void FilterByTC(string tc)
        {
            MyConnection.CheckConnection();
            SqlCommand command_filter = new SqlCommand("SELECT * FROM TablePatient WHERE PatientTC=@ptc", MyConnection.connection);
            command_filter.Parameters.AddWithValue("@ptc", tc);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_filter);
            DataTable d_Table = new DataTable();
            dataAdapter.Fill(d_Table);
            datagrid.ItemsSource = d_Table.DefaultView;
            tboxFilter.Text = "";
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Bir metot yazın, bu metotta girilen tc kimlik numarasına göre
            // hastayı ekrana getirin.
            FilterByTC(tboxFilter.Text);
        }

        string selected_patient_tc = "";
        string selected_patient_name = "";
        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_patient_tc = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["PatientTC"].ToString();
            selected_patient_name = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["PatientNameSurname"].ToString();
            tboxDelete.Text = selected_patient_name;
            tboxAppointmentPatient.Text = selected_patient_tc;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_delete = new SqlCommand("DELETE FROM TablePatient WHERE PatientTC=@ptc", MyConnection.connection);
            command_delete.Parameters.AddWithValue("@ptc", selected_patient_tc);
            command_delete.ExecuteNonQuery();
            GetPatients();
            tboxDelete.Text = "";
        }

        private void GetDoctorByClinicID()
        {
            MyConnection.CheckConnection();
            SqlCommand command_filter = new SqlCommand("SELECT DoctorID,DoctorNameSurname FROM TableDoctor WHERE DoctorClinicID=@pid", MyConnection.connection);
            command_filter.Parameters.AddWithValue("@pid", Convert.ToInt32(cboxClinics.SelectedValue));
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_filter);
            DataTable data_table = new DataTable();
            dataAdapter.Fill(data_table);
            cboxDoctors.ItemsSource = data_table.DefaultView;
            cboxDoctors.SelectedValuePath = "DoctorID";
            cboxDoctors.DisplayMemberPath = "DoctorNameSurname";
        }

        private void cboxClinics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Üstteki combobox'ın seçimi değiştiğinde, clinic id'ye göre o clinicte
            // çalışan doktorları alttaki comboboxa listeleyin.
            GetDoctorByClinicID();
        }

        private void btnAddAppointment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cboxHour_DropDownOpened(object sender, EventArgs e)
        {
            SaatleriEkle();
        }
    }
}
