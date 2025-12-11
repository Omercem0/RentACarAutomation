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
    public partial class RentCarForm : Form
    {

        public int _carId;
        public decimal _dailyPrice;
        public RentCarForm(int carId, string carInfo, decimal dailyPrice)
        {
            InitializeComponent();
            LoadCustomers();

            _carId = carId;
            _dailyPrice = dailyPrice;

            lblCarInfo.Text = carInfo;
            lblDailyPrice.Text = "Günlük: " + dailyPrice.ToString("C2");

            dtpRentDate.ValueChanged += (s, e) => CalculatePrice();
            dtpReturnDate.ValueChanged += (s, e) => CalculatePrice();

            CalculatePrice();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void LoadCustomers()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT Id, FirstName || ' ' || LastName AS FullName FROM Customers";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            cmbCustomer.DisplayMember = "FullName"; 
                            cmbCustomer.ValueMember = "Id";         
                            cmbCustomer.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteri listesi yüklenirken hata: " + ex.Message);
            }
        }
        private void CalculatePrice()
        {
            DateTime rentDate = dtpRentDate.Value.Date; 
            DateTime returnDate = dtpReturnDate.Value.Date;

            if (returnDate < rentDate)
            {
                lblTotalPrice.Text = "Hatalı Tarih!";
                return;
            }

            TimeSpan ts = returnDate - rentDate;
            int days = ts.Days;

            if (days == 0) days = 1; 

            decimal total = days * _dailyPrice;
            lblTotalPrice.Text = total.ToString("C2"); 
            lblTotalPrice.Tag = total; 
        }
        private void dtpRentDate_ValueChanged(object sender, EventArgs e) { CalculatePrice(); }
        private void dtpReturnDate_ValueChanged(object sender, EventArgs e) { CalculatePrice(); }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir müşteri seçin.");
                return;
            }

            if (lblTotalPrice.Text == "Hatalı Tarih!")
            {
                MessageBox.Show("İade tarihi alış tarihinden önce olamaz.");
                return;
            }

            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    using (SQLiteTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            string sqlRent = @"INSERT INTO Rentals (CarId, CustomerId, RentDate, ReturnDate, TotalPrice, Status) 
                   VALUES (@carId, @custId, @rDate, @retDate, @price, 'Devam Ediyor')";

                            using (SQLiteCommand cmd = new SQLiteCommand(sqlRent, conn))
                            {
                                cmd.Parameters.AddWithValue("@carId", _carId);
                                cmd.Parameters.AddWithValue("@custId", cmbCustomer.SelectedValue); 
                                cmd.Parameters.AddWithValue("@rDate", dtpRentDate.Value.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@retDate", dtpReturnDate.Value.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(lblTotalPrice.Tag));
                                cmd.ExecuteNonQuery();
                            }

                            string sqlUpdate = "UPDATE Cars SET Status='Kirada' WHERE Id=@id";
                            using (SQLiteCommand cmd = new SQLiteCommand(sqlUpdate, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", _carId);
                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit();

                            MessageBox.Show("Kiralama işlemi başarılı!", "Bilgi");
                            this.DialogResult = DialogResult.OK;
                            this.Close();
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
    }
}
