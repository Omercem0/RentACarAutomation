using RentACarProject.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentACarProject
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private string _userRole;
        private string _userName;

        public DashboardForm(string name, string role)
        {
            InitializeComponent();
            _userName = name;
            _userRole = role;
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "Hoşgeldin, " + _userName;

            if (_userRole != "Admin")
            {
                btnUsers.Visible = false;
            }

            loadForm(new HomeForm(_userRole));
        }

        public void loadForm(object Form)
        {
            if (this.pnlContent.Controls.Count > 0)
                this.pnlContent.Controls.RemoveAt(0); 

            Form f = Form as Form;
            f.TopLevel = false; 
            f.Dock = DockStyle.Fill; 
            this.pnlContent.Controls.Add(f); 
            this.pnlContent.Tag = f;
            f.Show(); 
        }

        bool move;
        int mouse_x;
        int mouse_y;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            mouse_x = e.X;
            mouse_y = e.Y;
        }
        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                this.SetDesktopLocation(MousePosition.X - mouse_x, MousePosition.Y - mouse_y);
            }
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {

                Button currentBtn = (Button)btnSender;

                pnlNav.Height = currentBtn.Height;
                pnlNav.Top = currentBtn.Top;
                pnlNav.Left = currentBtn.Left; 

                currentBtn.BackColor = Color.FromArgb(46, 51, 73); 
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            lblTitle.Text = "Ana Sayfa";

            loadForm(new HomeForm(_userRole));
        }

        private void btnCars_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            lblTitle.Text = "Araç İşlemleri";

            loadForm(new CarsForm());
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            lblTitle.Text = "Kiralama İşlemleri";

            loadForm(new RentalsForm());
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            lblTitle.Text = "Müşteriler";

            loadForm(new CustomersForm());
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            lblTitle.Text = "Kullanıcılar";

            loadForm(new UsersForm());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult secim = MessageBox.Show("Çıkış yapmak istediğinize emin misiniz?", "Oturumu Kapat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (secim == DialogResult.Yes)
            {
                LoginForm login = new LoginForm();
                login.Show();


                this.Close();
            }
        }

        

    }
}
