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
    public partial class UserEditForm : Form
    {
        public UserEditForm()
        {
            InitializeComponent();
            cmbRole.SelectedIndex = 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Users WHERE Username=@u";
                    using (SQLiteCommand cmdCheck = new SQLiteCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@u", txtUsername.Text.Trim());
                        int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string sql = "INSERT INTO Users (Username, Password, Role) VALUES (@u, @p, @r)";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@p", txtPassword.Text.Trim()); 
                        cmd.Parameters.AddWithValue("@r", cmbRole.SelectedItem.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kullanıcı oluşturuldu!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
