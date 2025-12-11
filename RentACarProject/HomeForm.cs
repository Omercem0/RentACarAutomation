using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RentACarProject.DataAccess;
using System.Data.SQLite;

namespace RentACarProject
{
    public partial class HomeForm : Form
    {
        string _userRole;
        public HomeForm(string role)
        {
            InitializeComponent();
            _userRole = role;
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            if (_userRole != "Admin")
            {
                chartCars.Visible = false;

                Label lblMsg = new Label();
                lblMsg.Text = "İyi Çalışmalar!";
                lblMsg.Font = new Font("Segoe UI", 20, FontStyle.Bold);
                lblMsg.ForeColor = Color.Gray;
                lblMsg.AutoSize = true;
                lblMsg.Location = new Point(50, 50); 
                this.Controls.Add(lblMsg);
                label2.Visible = false;
                chartCars.Visible = false;
            }
            else
            {
                LoadDashboardChart();
                label2.Visible = true;
                LoadBrandChart();
            }
        }
        private void LoadDashboardChart()
        {
            chartCars.Series[0].Points.Clear();
            chartCars.Titles.Clear();
            chartCars.Titles.Add("Araç Durum Grafiği");

            try
            {
                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT Status, COUNT(*) as Count FROM Cars GROUP BY Status";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                int count = Convert.ToInt32(reader["Count"]);
                                chartCars.Series[0].Points.AddXY(status, count);
                            }
                        }
                    }
                }
                chartCars.Series[0].IsValueShownAsLabel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Grafik hatası: " + ex.Message);
            }
        }

        private void LoadBrandChart()
        {
            try
            {
                chartIncome.Series.Clear();   
                chartIncome.Titles.Clear();    

                System.Windows.Forms.DataVisualization.Charting.Series seri = new System.Windows.Forms.DataVisualization.Charting.Series("Markalar");
                seri.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column; 
                chartIncome.Series.Add(seri); 

                chartIncome.Titles.Add("Filo Marka Dağılımı");
                chartIncome.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                chartIncome.Titles[0].ForeColor = Color.DimGray;

                using (SQLiteConnection conn = DatabaseContext.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT UPPER(Brand) as BrandName, COUNT(*) as Count FROM Cars GROUP BY UPPER(Brand)";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string brand = reader["BrandName"].ToString();
                                int count = Convert.ToInt32(reader["Count"]);

                                chartIncome.Series[0].Points.AddXY(brand, count);
                            }
                        }
                    }
                }
                chartIncome.Series[0]["PointWidth"] = "0.6";

                chartIncome.Series[0].IsValueShownAsLabel = true;

                if (chartIncome.ChartAreas.Count > 0)
                {
                    chartIncome.ChartAreas[0].AxisY.Interval = 1;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Marka grafiği hatası: " + ex.Message + "\n" + ex.StackTrace);
            }
        }


    }
}
