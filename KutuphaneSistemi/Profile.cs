using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class Profile : Form
    {
        private Menu form5;
        private Form7 form7;
        private UyelerUpdate uyelerupdate;
        public Profile(Menu form5Reference)
        {
            InitializeComponent();
            form5 = form5Reference;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Profile_Load(object sender, EventArgs e)
        {
            if (form5 != null)
            {
                labelAdminName.Text = form5.labelAdminName.Text;
                string tarih = GetTarih(labelAdminName.Text);
                string perm = GetPerm(labelAdminName.Text);
                string hakkimda = GetHakkimda(labelAdminName.Text);
                label3.Text = tarih; label4.Text = perm.ToString();
                if (hakkimda.Length <= 75)
                {
                    label6.Text = hakkimda;
                }
                else
                {
                    label6.Text = hakkimda.Substring(0, 75);
                    label6.Text += "...";
                }
            }
        }

        private string GetTarih(string username)
        {
            string tarih = "Tarih Bulunamadı";
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';"))
            {
                connection.Open();
                string query = "SELECT Tarih FROM admin WHERE Name = @isim";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isim", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime dateTime = Convert.ToDateTime(reader["Tarih"]);
                            tarih = dateTime.ToString("yyyy-MM-dd");
                        }
                    }
                }
            }

            return tarih;
        }
        private string GetHakkimda(string username)
        {
            string profile = "Bilgi Bulunamadı";
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';"))
            {
                connection.Open();
                string query = "SELECT info FROM admin WHERE Name = @isim";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isim", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["info"] != DBNull.Value)
                            {
                                profile = reader["info"].ToString();
                            }
                            else
                            {
                                label6.Text = "Bilgi Eklenmemiş";
                            }
                        }
                    }
                }
            }

            return profile;
        }

        private string GetPerm(string username)
        {
            string perm = "Perm Bulunamadı";

            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';"))
            {
                connection.Open();
                string query = "SELECT yetkiler.yetki_adi FROM yetkiler " +
                               "INNER JOIN admin ON yetkiler.yetki_id = admin.yetkiler " +
                               "WHERE admin.Name = @isim";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isim", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            perm = reader["yetki_adi"].ToString();
                        }
                    }
                }
            }

            return perm;
        }

        private void ıconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
            form5.Close();
            form5 = new Menu(form7, uyelerupdate);
            form5.Show();
        }
    }
}
