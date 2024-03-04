using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KutuphaneSistemi
{
    public partial class info : Form
    {
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        public info()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void info_Load(object sender, EventArgs e)
        {
            KitaplarUyeler(); Uyeler(); Staff(); Chart(); Chart3(); Odunc();
        }
        private void KitaplarUyeler()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string queryBooks = "SELECT COUNT(*) FROM kitap";
                using (MySqlCommand commandBooks = new MySqlCommand(queryBooks, connection))
                {
                    int totalBooks = Convert.ToInt32(commandBooks.ExecuteScalar());
                    label1.Text = totalBooks.ToString();
                    chart1.Series["Kitaplar"].Points.AddXY("", totalBooks);
                }
                string queryMembers = "SELECT COUNT(*) FROM uyeler";
                using (MySqlCommand commandMembers = new MySqlCommand(queryMembers, connection))
                {
                    int totalMembers = Convert.ToInt32(commandMembers.ExecuteScalar());
                    chart1.Series["Üyeler"].Points.AddXY("", totalMembers);
                }
                string queryOduncBook = "SELECT COUNT(*) FROM odunc_kitaplar WHERE Alinan_Tarih IS NULL";
                using (MySqlCommand commandMembers = new MySqlCommand(queryOduncBook, connection))
                {
                    int totalBook = Convert.ToInt32(commandMembers.ExecuteScalar());
                    chart1.Series["Ödünç_Kitaplar"].Points.AddXY("", totalBook);
                }
            }
        }
        private void Uyeler()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM uyeler";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int totalUsers = Convert.ToInt32(command.ExecuteScalar());
                    label4.Text = totalUsers.ToString();
                }
            }
        }
        private void Staff()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM admin";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int totalUsers = Convert.ToInt32(command.ExecuteScalar());
                    label6.Text = totalUsers.ToString();
                }
            }
        }
        private void Odunc()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM odunc_kitaplar WHERE Alinan_Tarih IS NULL";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    int totalUsers = Convert.ToInt32(command.ExecuteScalar());
                    label10.Text = totalUsers.ToString();
                }
            }
        }
        private void Chart()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string tableCountsQuery = "SELECT table_name, table_rows FROM information_schema.tables WHERE table_schema = 'kütüphane sistemi' AND table_name IN ('kitap', 'uyeler', 'odunc_kitaplar')";

                using (MySqlCommand tableCountsCommand = new MySqlCommand(tableCountsQuery, connection))
                {
                    using (MySqlDataReader tableCountsReader = tableCountsCommand.ExecuteReader())
                    {
                        List<string> tableNames = new List<string>();
                        List<int> recordCounts = new List<int>();
                        while (tableCountsReader.Read())
                        {
                            string tableName = tableCountsReader["table_name"].ToString();
                            int recordCount;
                            if (tableName.Equals("odunc_kitaplar"))
                            {
                                tableNames.Add(tableName);
                                recordCounts.Add(0);
                            }
                            else
                            {
                                recordCount = Convert.ToInt32(tableCountsReader["table_rows"]);
                                tableNames.Add(tableName);
                                recordCounts.Add(recordCount);
                            }
                        }
                        tableCountsReader.Close();
                        for (int i = 0; i < tableNames.Count; i++)
                        {
                            if (tableNames[i].Equals("odunc_kitaplar"))
                            {
                                recordCounts[i] = GetRecordCountForOduncKitaplarWithNullDate(connection);
                            }
                        }
                        chart2.Series.Clear();
                        chart2.Series.Add("Kayıt Sayısı");
                        chart2.Series["Kayıt Sayısı"].Points.DataBindXY(tableNames, recordCounts);
                        chart2.Series["Kayıt Sayısı"].ChartType = SeriesChartType.Doughnut;
                    }
                }
            }
        }

        private int GetRecordCountForOduncKitaplarWithNullDate(MySqlConnection connection)
        {
            string oduncKitaplarQuery = "SELECT COUNT(*) FROM odunc_kitaplar WHERE Alinan_Tarih IS NULL";
            using (MySqlCommand oduncKitaplarCommand = new MySqlCommand(oduncKitaplarQuery, connection))
            {
                int recordCount = Convert.ToInt32(oduncKitaplarCommand.ExecuteScalar());
                return recordCount;
            }
        }


        private void Chart3()
        {
            DateTime SonCalisma = Properties.Settings.Default.SonCalisma;
            label8.Text = SonCalisma.ToString();
        }
    }
}
