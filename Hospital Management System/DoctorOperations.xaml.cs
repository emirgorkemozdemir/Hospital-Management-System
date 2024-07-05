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

            cmbAddClinic.SelectedIndex = 0;


            cmbFilter.ItemsSource = dataTable.DefaultView;

            // Display member gözüken değer için
            cmbFilter.DisplayMemberPath = "ClinicName";

            // Selected value path arkada çalışan değer için
            cmbFilter.SelectedValuePath = "ClinicID";

            cmbFilter.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetDoctors();
            GetClinics();
        }

        private void btnAddDoctor_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_add = new SqlCommand("INSERT INTO TableDoctor (DoctorNameSurname,DoctorClinicID) VALUES (@pname,@pid)",MyConnection.connection);
            command_add.Parameters.AddWithValue("@pname", tboxAddDoctorName.Text);
            command_add.Parameters.AddWithValue("@pid", cmbAddClinic.SelectedValue);
            command_add.ExecuteNonQuery();
            IncreaseDoctorCount();
            GetDoctors();
            GetClinics();
            tboxAddDoctorName.Text = "";
        }

        private string selected_doctor_name = "";
        private string selected_doctor_clinic = "";
        private int selected_doctor_id = 0;
        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_doctor_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["DoctorID"]);
            selected_doctor_name = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["DoctorNameSurname"].ToString();
            selected_doctor_clinic = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["ClinicName"].ToString();
            tboxDeleteDoctorName.Text = selected_doctor_name;
            tboxUpdateDoctor.Text = selected_doctor_name;
        }

        private void btnDeleteDoctor_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_delete = new SqlCommand("DELETE FROM TableDoctor WHERE DoctorID=@pid",MyConnection.connection);
            command_delete.Parameters.AddWithValue("@pid", selected_doctor_id);
            command_delete.ExecuteNonQuery();
            DecreaseDoctorCount();
            tboxDeleteDoctorName.Text = "";
            GetDoctors();
        }

        // Bir tane metot açınız, açılan metotta doktor ekleme işleminden sonra
        // doktorun eklendiği kliniğin doktor sayısını 1 arttırınız.
        private void IncreaseDoctorCount()
        {
            MyConnection.CheckConnection();
            SqlCommand command_increase = new SqlCommand("UPDATE TableClinic SET ClinicDoctorCount=ClinicDoctorCount+1 WHERE ClinicID=@pid",MyConnection.connection);
            command_increase.Parameters.AddWithValue("@pid",Convert.ToInt32(cmbAddClinic.SelectedValue));
            command_increase.ExecuteNonQuery();
        }


        // selected_doctor_clinic değerine göre TableClinic tablosunda
        // doktor sayısını azaltın.
        private void DecreaseDoctorCount()
        {
            MyConnection.CheckConnection();
            SqlCommand command_increase = new SqlCommand("UPDATE TableClinic SET ClinicDoctorCount=ClinicDoctorCount-1 WHERE ClinicName=@pname", MyConnection.connection);
            command_increase.Parameters.AddWithValue("@pname", selected_doctor_clinic);
            command_increase.ExecuteNonQuery();
        }

        private void btnUpdateDoctor_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_update = new SqlCommand("UPDATE TableDoctor SET DoctorNameSurname=@pname WHERE DoctorID=@pid",MyConnection.connection);
            command_update.Parameters.AddWithValue("@pname", tboxUpdateDoctor.Text);
            command_update.Parameters.AddWithValue("@pid", selected_doctor_id);
            command_update.ExecuteNonQuery();
            GetDoctors();
            tboxUpdateDoctor.Text = "";
        }

        private void FilterByClinicID()
        {
            MyConnection.CheckConnection();
            SqlCommand command_filter = new SqlCommand("SELECT DoctorID,DoctorNameSurname,ClinicName FROM TableDoctor INNER JOIN TableClinic ON ClinicID=DoctorClinicID WHERE DoctorClinicID=@pid",MyConnection.connection);
            command_filter.Parameters.AddWithValue("@pid",Convert.ToInt32(cmbFilter.SelectedValue));
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_filter);
            DataTable data_table = new DataTable();
            dataAdapter.Fill(data_table);
            datagrid.ItemsSource = data_table.DefaultView;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Buranın mantığı şöyle işleyecek

            // Eğer textbox boşsa ama comboboxtan seçim yapıldıysa
            // sadece comboboxtan seçilen kliniğe göre sıralanır.

            // Eğer combobx boşsa ama textboxa yazı girildiyse
            // sadece yazıya göre arama yapılır.

            // İkisi de doluysa ikisine göre arama yapılır

            // İkisi de boşsa tüm doktorlar ekrana yansıtılır.

            if (tboxFilter.Text=="" && cmbFilter.SelectedValue!=null)
            {
                // Burada kliniğe göre arama yapılacak
                // Bir metot yazın, bu metotta cmbfilter'ın seçili değerine göre
                // select yapın.
                FilterByClinicID();
            }
            else if (tboxFilter.Text!="" && cmbFilter.SelectedValue == null)
            {
                // Burada doktor ismine göre arama yapılacak
                MessageBox.Show("Sadece doktor ismi yazıldı");
            }
            else if (tboxFilter.Text != "" && cmbFilter.SelectedValue!= null)
            {
                // Burada hem doktor ismine göre hem de kliniğe göre arama yapılacak
                MessageBox.Show("Hem klinik seçildi hem doktor ismi yazıldı");
            }
            else
            {
                // Burada direkt tüm doktorları listeleyeceğiz
                MessageBox.Show("İkisi de boş");
                GetDoctors();
            }
        }
    }
}
