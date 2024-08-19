using Hospital_Management_System.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hospital_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_login_Click(object sender, RoutedEventArgs e)
        {
            MyConnection.CheckConnection();
            SqlCommand command_login = new SqlCommand("SELECT * FROM TableUser WHERE UserName=@pusername AND UserPassword=@ppass",MyConnection.connection);
            command_login.Parameters.AddWithValue("@pusername",tboxUsername.Text);
            command_login.Parameters.AddWithValue("@ppass",pbox.Password);
            SqlDataReader data_reader = command_login.ExecuteReader();

           
            // Giriş başarılıysa bu ifin içerisi çalışacak.
            if (data_reader.HasRows)
            {
                int user_role = 0;
                while (data_reader.Read())
                {
                    LoggedUserInfos.LoggedUserID =Convert.ToInt32(data_reader[0]);
                    LoggedUserInfos.LoggedUserName = data_reader[1].ToString();
                    user_role = Convert.ToInt32(data_reader[3]);
                }

                data_reader.Close();


                // Ana sayfaya yönlendirme
                if (user_role==1)
                {
                    DoctorPanel panel = new DoctorPanel();
                    panel.Show();
                    this.Hide();
                }
                else if (user_role == 2)
                {
                    Main panel = new Main();
                    panel.Show();
                    this.Hide();
                }
                else if (user_role == 3)
                {
                    UploadFile panel = new UploadFile();
                    panel.Show();
                    this.Hide();
                }
            }
        }
    }
}
