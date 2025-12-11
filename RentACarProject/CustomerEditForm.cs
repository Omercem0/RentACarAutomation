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
    public partial class CustomerEditForm : Form
    {
        public CustomerEditForm()
        {
            InitializeComponent();
        }

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || !txtIdentity.MaskCompleted)
            {
                MessageBox.Show("Lütfen Ad ve TC Kimlik alanlarını eksiksiz doldurun.");
                return;
            }

            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string checkSql = "SELECT COUNT(*) FROM Customers WHERE IdentityNumber = @tc";
                    using (SQLiteCommand cmdCheck = new SQLiteCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@tc", txtIdentity.Text.Trim());
                        int count = Convert.ToInt32(cmdCheck.ExecuteScalar()); 

                        if (count > 0) 
                        {
                            MessageBox.Show("Bu TC Kimlik Numarası ile kayıtlı bir müşteri zaten var!", "Mükerrer Kayıt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; 
                        }
                    }

                    string sql = @"INSERT INTO Customers (FirstName, LastName, IdentityNumber, Phone) 
                                   VALUES (@name, @surname, @tc, @phone)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@surname", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@tc", txtIdentity.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Müşteri başarıyla eklendi!");
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
