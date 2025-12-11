using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace RentACarProject.DataAccess
{
    public class DatabaseContext
    {
        private static string dbPath = Path.Combine(Application.StartupPath, "rentacar.db");

        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static void CheckConnection()
        {
            try
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası!\n\n" + ex.Message,
                                "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
