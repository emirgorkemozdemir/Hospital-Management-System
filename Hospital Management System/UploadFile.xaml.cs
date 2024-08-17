using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Hospital_Management_System.Classes;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Hospital_Management_System
{
    /// <summary>
    /// Interaction logic for UploadFile.xaml
    /// </summary>
    public partial class UploadFile : Window
    {
        static string[] Scopes = { DriveService.Scope.DriveFile };
        static string ApplicationName = "Drive API .NET Quickstart";
        string credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
       

        private void GetAppointmentsByTC()
        {
            MyConnection.CheckConnection();
            SqlCommand command_GetAppointmentsByTC = new SqlCommand("SELECT * FROM TableAppointment WHERE AppointmentPatient = @ptc",MyConnection.connection);
            command_GetAppointmentsByTC.Parameters.AddWithValue("@ptc",tboxTC.Text);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_GetAppointmentsByTC);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            datagrid.ItemsSource = dataTable.DefaultView;
        }

        private void GetAppointmentsByHour()
        {
            MyConnection.CheckConnection();
            DateTime todays_date = DateTime.Now.Date;
            SqlCommand command_GetAppointmentsByTC = new SqlCommand("SELECT TOP 10 * FROM TableAppointment WHERE AppointmentDay=@pday ORDER BY AppointmentHour DESC", MyConnection.connection);
            command_GetAppointmentsByTC.Parameters.AddWithValue("@pday", todays_date);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command_GetAppointmentsByTC);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            datagrid.ItemsSource = dataTable.DefaultView;
        }

        int selected_appointment_id = 0;
        string patient_tc = "";

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_appointment_id = ((datagrid.SelectedItem as DataRowView) == null) ? 0 : Convert.ToInt32((datagrid.SelectedItem as DataRowView)["AppointmentID"]);
            patient_tc = ((datagrid.SelectedItem as DataRowView) == null) ? "" : (datagrid.SelectedItem as DataRowView)["AppointmentPatient"].ToString();
        }

        private void upload()
        {
          
            // OAuth 2.0 kimlik doğrulaması
            UserCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Google Drive API servisi oluşturma
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Dosya metadata ve yükleme işlemi
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = patient_tc+ DateTime.Now.Date.ToString("yyyy-MM-dd") + selected_appointment_id.ToString(),
                MimeType = "image/jpeg"
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(selectedFilePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;

            // Dosya paylaşım ayarlarını herkese açık olarak güncelle
            var permission = new Permission()
            {
                Type = "anyone",
                Role = "reader"
            };

            var permissionRequest = service.Permissions.Create(permission, file.Id);
            permissionRequest.Fields = "id";
            permissionRequest.Execute();

            // Paylaşım linkini oluşturma
            var fileRequest = service.Files.Get(file.Id);
            fileRequest.Fields = "webViewLink";
            var fileDetails = fileRequest.Execute();

            string shareableLink = fileDetails.WebViewLink;
            tbox.Text = shareableLink;

            MyConnection.CheckConnection();
            SqlCommand command_add_file_to_db = new SqlCommand("INSERT INTO TableFile (FileLink,FileAppointment) VALUES (@plink,@papp)",MyConnection.connection);
            command_add_file_to_db.Parameters.AddWithValue("@plink", shareableLink);
            command_add_file_to_db.Parameters.AddWithValue("@papp", selected_appointment_id);
            command_add_file_to_db.ExecuteNonQuery();

            MessageBox.Show("File uploaded successfully");
        }
        public UploadFile()
        {
            InitializeComponent();
            GetAppointmentsByHour();
        }

        string selectedFilePath = "";

        private void SelectImageFile()
        {
            // OpenFileDialog nesnesi oluştur
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                // Resim dosyalarını filtrele
                Filter = "Image Files (*.jpeg; *.jpg; *.png)|*.jpeg;*.jpg;*.png|All Files (*.*)|*.*",
                Title = "Select an Image File"
            };

            // Dosya seçim penceresini göster
            bool? result = openFileDialog.ShowDialog();

            if (result == true) // Kullanıcı bir dosya seçti mi?
            {
                // Seçilen dosyanın tam yolunu al
                selectedFilePath = openFileDialog.FileName;

                // Seçilen dosyanın yolunu göster
                MessageBox.Show($"Selected file: {selectedFilePath}");

                // Burada seçilen dosyanın yolu ile diğer işlemleri gerçekleştirebilirsiniz
                // Örneğin: dosyanın içeriğini okuma, önizleme vb.
            }
            else
            {
                MessageBox.Show("No file selected.");
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            SelectImageFile();

            upload();
        }

        private void tboxTC_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        // Sayfa ilk yüklendiğinde, en son eklenen 5 tane randevuyu ekrana alsın.
        private void tboxTC_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tboxTC.Text.Length==11)
            {
                GetAppointmentsByTC();
            }
            else
            {
                datagrid.Columns.Clear();
            }
        }

      
    }
}
