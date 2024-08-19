using Hospital_Management_System.Classes;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
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
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.IO;
using System.Net.Http;

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
            SqlCommand command_GetClinicByID = new SqlCommand("SELECT DoctorClinicID FROM TableDoctor WHERE DoctorID=@pid", MyConnection.connection);
            command_GetClinicByID.Parameters.AddWithValue("@pid", id);
            SqlDataReader dr = command_GetClinicByID.ExecuteReader();
            int selected_clinic_id = 0;
            while (dr.Read())
            {
                selected_clinic_id = Convert.ToInt32(dr[0]);
            }
            dr.Close();
            return selected_clinic_id;
        }


        int selected_appointment_id = 0;
        int selected_doctor_id = 0;
        int selected_doctor_clinic_id = 0;
        string selected_appointment_patient = "";
        string selected_appointment_patient_tc = "";
        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_appointment_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["AppointmentID"]);
            selected_appointment_patient = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["PatientNameSurname"].ToString();
            selected_appointment_patient_tc = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["AppointmentPatient"].ToString();
            selected_doctor_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["AppointmentDoctor"]);
            selected_doctor_clinic_id = GetClinicByID(selected_doctor_id);
            tboxSelectedPatient.Text = selected_appointment_patient;
        }

        private void DeleteAppointment()
        {
            MyConnection.CheckConnection();
            SqlCommand command_delete_appointment = new SqlCommand("DELETE FROM TableAppointment WHERE AppointmentID =@pid", MyConnection.connection);
            command_delete_appointment.Parameters.AddWithValue("@pid", selected_appointment_id);
            command_delete_appointment.ExecuteNonQuery();
            LoadAppointments();
        }

        private void btnDeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            DeleteAppointment();
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_finish_appointment = new SqlCommand("INSERT INTO TableAppointmentResult (AppointmentResultPatient,AppointmentResultDoctor,AppointmentResultClinic,AppointmentResultText,AppointmentResultPrescription,AppointmentID) VALUES (@ppatient,@pdoctor,@pclinic,@ptext,@ppres,@pappointment)", MyConnection.connection);
            command_finish_appointment.Parameters.AddWithValue("@ppatient", selected_appointment_patient_tc);
            command_finish_appointment.Parameters.AddWithValue("@pdoctor", selected_doctor_id);
            command_finish_appointment.Parameters.AddWithValue("@pclinic", selected_doctor_clinic_id);
            command_finish_appointment.Parameters.AddWithValue("@ptext", tboxText.Text);
            command_finish_appointment.Parameters.AddWithValue("@ppres", tboxPrescription.Text);
            command_finish_appointment.Parameters.AddWithValue("@pappointment", selected_appointment_id);
            command_finish_appointment.ExecuteNonQuery();

            DeleteAppointment();
        }

        static string ExtractSubstring(string url, string startMarker, string endMarker)
        {
            int startIndex = url.IndexOf(startMarker);
            if (startIndex == -1)
            {
                return null;  // Başlangıç işareti bulunamadı
            }

            startIndex += startMarker.Length;  // Başlangıç işareti sonrasından başla
            int endIndex = url.IndexOf(endMarker, startIndex);
            if (endIndex == -1)
            {
                return null;  // Bitiş işareti bulunamadı
            }

            return url.Substring(startIndex, endIndex - startIndex);
        }

        private async Task DownloadFilesAsync(List<string> links)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (var link in links)
                {
                    try
                    {
                        var response = await client.GetAsync(link);
                        response.EnsureSuccessStatusCode();

                        var fileName = "image_" + Guid.NewGuid() + ".jpeg";
                        var filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to download {link}: {ex.Message}");
                    }
                }
            }
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_get_files = new SqlCommand("SELECT FileLink FROM TableFile WHERE FileAppointment=@papp", MyConnection.connection);
            command_get_files.Parameters.AddWithValue("@papp", selected_appointment_id);
            SqlDataReader dataReader = command_get_files.ExecuteReader();
            List<string> links = new List<string>();
            while (dataReader.Read())
            {
                string download_link = "https://drive.google.com/uc?export=download&id=" + ExtractSubstring(dataReader[0].ToString(), "/d/", "/v");
                links.Add(download_link);
            }

            dataReader.Close();

           await DownloadFilesAsync(links);
            MessageBox.Show("All Files Downloaded To Desktop!");
            
        }
    }
}
