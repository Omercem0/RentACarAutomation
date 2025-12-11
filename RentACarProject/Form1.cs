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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        bool move;
        int mouse_x;
        int mouse_y;

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            mouse_x = e.X;
            mouse_y = e.Y;
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - mouse_x, MousePosition.Y - mouse_y);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string uName = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(uName) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT * FROM Users WHERE Username=@user AND Password=@pass";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", uName);
                        cmd.Parameters.AddWithValue("@pass", pass);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                string dbRole = reader["Role"].ToString();
                                string dbUser = reader["Username"].ToString();

                                MessageBox.Show("Giriş Başarılı! Yönlendiriliyorsunuz...", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                DashboardForm mainForm = new DashboardForm(dbUser, dbRole);

                                mainForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
