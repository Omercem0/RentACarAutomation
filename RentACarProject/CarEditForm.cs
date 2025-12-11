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
    public partial class CarEditForm : Form
    {
        public CarEditForm()
        {
            InitializeComponent();
        }

        public string SelectedImagePath = "";

        bool move;
        int mouse_x;
        int mouse_y;
        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            mouse_x = e.X;
            mouse_y = e.Y;
        }

        private void pnlHeader_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - mouse_x, MousePosition.Y - mouse_y);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp"; 
            ofd.Title = "Araç Resmi Seç";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pbCarImage.ImageLocation = ofd.FileName;

                SelectedImagePath = ofd.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text) || string.IsNullOrWhiteSpace(txtModel.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Lütfen Marka, Model ve Fiyat alanlarını doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string newFileName = "bos";

                if (!string.IsNullOrEmpty(SelectedImagePath)) 
                {
                    string targetFolder = Path.Combine(Application.StartupPath, "Images");

                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    string fileExtension = Path.GetExtension(SelectedImagePath); 
                    newFileName = Guid.NewGuid().ToString() + fileExtension;

                    string destPath = Path.Combine(targetFolder, newFileName);
                    File.Copy(SelectedImagePath, destPath);
                }

                // Veritabanına Kayıt (INSERT)
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO Cars (Brand, Model, Year, DailyPrice, Status, ImagePath) 
                           VALUES (@brand, @model, @year, @price, @status, @img)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@brand", txtBrand.Text.Trim());
                        cmd.Parameters.AddWithValue("@model", txtModel.Text.Trim());
                        cmd.Parameters.AddWithValue("@year", int.Parse(txtYear.Text)); // Sayıya çevir
                        cmd.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text)); // Paraya çevir
                        cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@img", newFileName); // Resmin yeni adını kaydediyoruz

                        cmd.ExecuteNonQuery(); // Sorguyu çalıştır
                    }
                }

                MessageBox.Show("Araç başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Formu Kapat ve Listeyi Yenilemesi için Sinyal Ver
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
