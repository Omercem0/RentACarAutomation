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
    public partial class UsersForm : Form
    {
        public UsersForm()
        {
            InitializeComponent();
            LoadUserList();
        }
        public void LoadUserList()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM Users"; 

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvUsers.DataSource = dt;
                    }
                }

                if (dgvUsers.Columns.Count > 0)
                {
                    dgvUsers.Columns["Id"].Visible = false;      
                    dgvUsers.Columns["Password"].Visible = false; 

                    dgvUsers.Columns["Username"].HeaderText = "Kullanıcı Adı";
                    dgvUsers.Columns["Role"].HeaderText = "Yetki (Rol)";

                    dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            UserEditForm form = new UserEditForm();
            form.ShowDialog();
            LoadUserList();
        }

        private void kullanıcıyıSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek kullanıcıyı seçin.");
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            if (username.ToLower() == "admin" || userId == 1)
            {
                MessageBox.Show("Ana Yönetici (Admin) silinemez! Bu sistemin güvenliği için engellenmiştir.", "İşlem Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult answer = MessageBox.Show(username + " adlı kullanıcıyı silmek istediğinize emin misiniz?\nBu işlem geri alınamaz.", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes)
            {
                DeleteUser(userId);
            }
        }

        private void DeleteUser(int id)
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM Users WHERE Id=@id";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kullanıcı başarıyla silindi.");
                LoadUserList(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}
