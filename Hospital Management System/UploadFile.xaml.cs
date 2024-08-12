using System;
using System.Collections.Generic;
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
       

        string patient_tc = "12345678900";
        string appointment_date = "120824";
        int clinic_id = 0;

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
                Name =patient_tc+appointment_date+clinic_id.ToString(),
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
        }
        public UploadFile()
        {
            InitializeComponent();
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
    }
}
