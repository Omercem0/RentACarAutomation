using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using RentACarProject.DataAccess;

namespace RentACarProject
{
    public partial class RentalsForm : Form
    {
        public RentalsForm()
        {
            InitializeComponent();
            LoadRentalList();
        }
        public void LoadRentalList()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            Rentals.Id,
                            Rentals.CarId,
                            Rentals.Status,
                            Cars.Brand || ' ' || Cars.Model AS CarInfo, 
                            Customers.FirstName || ' ' || Customers.LastName AS CustomerInfo,
                            Rentals.RentDate, 
                            Rentals.ReturnDate, 
                            Rentals.TotalPrice 
                        FROM Rentals
                        INNER JOIN Cars ON Rentals.CarId = Cars.Id
                        INNER JOIN Customers ON Rentals.CustomerId = Customers.Id
                        ORDER BY Rentals.Id DESC"; // En son kiralanan en üstte

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvRentals.DataSource = dt;
                    }
                }

                if (dgvRentals.Columns.Count > 0)
                {
                    dgvRentals.Columns["Id"].HeaderText = "İşlem No";
                    dgvRentals.Columns["CarInfo"].HeaderText = "Araç";
                    dgvRentals.Columns["CustomerInfo"].HeaderText = "Müşteri";
                    dgvRentals.Columns["RentDate"].HeaderText = "Alış Tarihi";
                    dgvRentals.Columns["ReturnDate"].HeaderText = "İade Tarihi";
                    dgvRentals.Columns["TotalPrice"].HeaderText = "Tutar";
                    dgvRentals.Columns["CarId"].Visible = false;
                    dgvRentals.Columns["Status"].HeaderText = "Durum";

                    dgvRentals.Columns["TotalPrice"].DefaultCellStyle.Format = "C2";

                    dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void teslimAlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen işlem yapılacak satırı seçin.");
                return;
            }

            int carId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["CarId"].Value);
            int rentalId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["Id"].Value);
            string carInfo = dgvRentals.SelectedRows[0].Cells["CarInfo"].Value.ToString();

            DialogResult answer = MessageBox.Show(carInfo + " aracı teslim alınarak 'Müsait' durumuna getirilecek.\nOnaylıyor musunuz?", "Teslim Alma", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                CompleteRental(rentalId, carId); 
                LoadRentalList();
            }
        }
        private void CompleteRental(int rentalId, int carId)
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    using (SQLiteTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            string sqlCar = "UPDATE Cars SET Status='Müsait' WHERE Id=@cid";
                            using (SQLiteCommand cmd = new SQLiteCommand(sqlCar, conn))
                            {
                                cmd.Parameters.AddWithValue("@cid", carId);
                                cmd.ExecuteNonQuery();
                            }

                            string sqlRent = "UPDATE Rentals SET Status='Tamamlandı' WHERE Id=@rid";
                            using (SQLiteCommand cmd = new SQLiteCommand(sqlRent, conn))
                            {
                                cmd.Parameters.AddWithValue("@rid", rentalId);
                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit(); 
                            MessageBox.Show("Araç teslim alındı, işlem kapatıldı.");
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void dgvRentals_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRentals.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                if (e.Value.ToString() == "Devam Ediyor")
                {
                    e.CellStyle.BackColor = Color.Orange;      
                    e.CellStyle.ForeColor = Color.White;       
                    e.CellStyle.SelectionBackColor = Color.DarkOrange;
                }
                else if (e.Value.ToString() == "Tamamlandı")
                {
                    e.CellStyle.BackColor = Color.LightGreen;  
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

    }
}
