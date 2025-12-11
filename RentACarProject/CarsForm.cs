using RentACarProject.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentACarProject
{
    public partial class CarsForm : Form
    {
        public CarsForm()
        {
            InitializeComponent();
            
            LoadCarList();
        }
        private void CarsForm_Load(object sender, EventArgs e)
        {
        }

        public void LoadCarList()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM Cars";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt); 

                        dgvCars.DataSource = dt; 
                    }
                }

                if (dgvCars.Columns.Count > 0)
                {
                    dgvCars.Columns["Id"].Visible = false; 
                    dgvCars.Columns["Brand"].HeaderText = "Marka";
                    dgvCars.Columns["Model"].HeaderText = "Model";
                    dgvCars.Columns["Year"].HeaderText = "Yıl";
                    dgvCars.Columns["DailyPrice"].HeaderText = "G.Ücret";
                    dgvCars.Columns["Status"].HeaderText = "Durum";
                    dgvCars.Columns["ImagePath"].Visible = false; 
                }

                if (dgvCars.Rows.Count > 0)
                {
                    dgvCars.Rows[0].Selected = true; 
                    dgvCars_CellClick(dgvCars, new DataGridViewCellEventArgs(0, 0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme hatası: " + ex.Message);
            }
        }
        private void btnAddCar_Click(object sender, EventArgs e)
        {
            CarEditForm addForm = new CarEditForm();

            addForm.ShowDialog();

            LoadCarList();
        }

        private void dgvCars_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvCars.Rows[e.RowIndex];

            string imageName = row.Cells["ImagePath"].Value.ToString();

            string folderPath = Path.Combine(Application.StartupPath, "Images");
            string fullPath = Path.Combine(folderPath, imageName);

            if (File.Exists(fullPath) && imageName != "bos")
            {
                pbSelectedCar.ImageLocation = fullPath; 
            }
            else
            {
                pbSelectedCar.Image = null;
            }
        }
        private void btnRentCar_Click(object sender, EventArgs e)
        {
            if (dgvCars.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen listeden bir araç seçin.");
                return;
            }

            DataGridViewRow row = dgvCars.SelectedRows[0];

            string status = row.Cells["Status"].Value.ToString();
            if (status == "Kirada" || status == "Bakımda")
            {
                MessageBox.Show("Bu araç şu an müsait değil!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(row.Cells["Id"].Value);
            string brandModel = row.Cells["Brand"].Value.ToString() + " " + row.Cells["Model"].Value.ToString();
            decimal price = Convert.ToDecimal(row.Cells["DailyPrice"].Value);
 
            RentCarForm rentForm = new RentCarForm(id, brandModel, price);
            rentForm.ShowDialog();

            LoadCarList();
        }

        private void seçilenAracıSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCars.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek aracı seçin.");
                return;
            }

            int carId = Convert.ToInt32(dgvCars.SelectedRows[0].Cells["Id"].Value);
            string carInfo = dgvCars.SelectedRows[0].Cells["Brand"].Value.ToString() + " " +
                             dgvCars.SelectedRows[0].Cells["Model"].Value.ToString();
            string status = dgvCars.SelectedRows[0].Cells["Status"].Value.ToString();

            if (status == "Kirada")
            {
                MessageBox.Show("DİKKAT! Bu araç şu an kirada (müşteride) görünüyor.\nKiradaki bir aracı sistemden silemezsiniz!", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult answer = MessageBox.Show(carInfo + " aracını filodan silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes)
            {
                DeleteCar(carId);
            }
        }
        private void DeleteCar(int id)
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Rentals WHERE CarId=@id";
                    using (SQLiteCommand cmdCheck = new SQLiteCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Bu aracın geçmiş kiralama kayıtları muhasebede duruyor.\nVeri bütünlüğü için bu aracı tamamen silemezsiniz, ancak durumunu 'Bakımda' yapabilirsiniz.", "Silemezsiniz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string sql = "DELETE FROM Cars WHERE Id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Araç başarıyla silindi.");
                LoadCarList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
