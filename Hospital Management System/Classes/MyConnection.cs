using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hospital_Management_System.Classes
{
    internal class MyConnection
    {
        public static SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=HospitalManagement;Integrated Security=True;TrustServerCertificate=True");
        
        public static void CheckConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed) 
            {
                connection.Open();
            }
        }
    }
}
