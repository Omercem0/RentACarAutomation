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
    public partial class CustomersForm : Form
    {
        public CustomersForm()
        {
            InitializeComponent();
            LoadList();
        }
        private void LoadList()
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM Customers";

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvCustomers.DataSource = dt;
                    }
                }

                if (dgvCustomers.Columns.Count > 0)
                {
                    dgvCustomers.Columns["Id"].Visible = false; 
                    dgvCustomers.Columns["FirstName"].HeaderText = "Ad";
                    dgvCustomers.Columns["LastName"].HeaderText = "Soyad";
                    dgvCustomers.Columns["IdentityNumber"].HeaderText = "TC Kimlik";
                    dgvCustomers.Columns["Phone"].HeaderText = "Telefon";

                    dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            CustomerEditForm form = new CustomerEditForm();
            form.ShowDialog();

            LoadList();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek müşteriyi seçin.");
                return;
            }

            // 2. ID'yi Al
            int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Id"].Value);
            string name = dgvCustomers.SelectedRows[0].Cells["FirstName"].Value.ToString();

            // 3. Onay İste
            DialogResult answer = MessageBox.Show(name + " isimli müşteriyi silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes)
            {
                DeleteCustomer(customerId);
            }
        }

        private void DeleteCustomer(int id)
        {
            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Rentals WHERE CustomerId=@id";
                    using (SQLiteCommand cmdCheck = new SQLiteCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Bu müşterinin kiralama geçmişi olduğu için SİLİNEMEZ!", "Güvenlik Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    
                    string sql = "DELETE FROM Customers WHERE Id=@id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Müşteri silindi.");
                LoadList(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

    }
}
