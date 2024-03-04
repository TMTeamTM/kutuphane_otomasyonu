using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class Admin : Form
    {
        private Menu form5;
        private Form7 form7;
        private UyelerUpdate uyelerupdate;
        public Admin(Menu form5Reference)
        {
            InitializeComponent();
            form5 = form5Reference;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Menu form5 = new Menu(form7, uyelerupdate);
            form5.Show();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string newImagePath = Path.Combine(downloadsFolderPath, "profile.png");
            string newImagePath2 = Path.Combine(downloadsFolderPath, "crowns.png");
            string girilenName = bunifuTextBox1.Text;
            string girilenPassword = bunifuTextBox2.Text;

            bool dogrulamaSonucu = Dogrula(girilenName, girilenPassword);

            if (dogrulamaSonucu)
            {
                Admin adminForm = (Admin)Application.OpenForms["Admin"];
                if (adminForm != null)
                {
                    adminForm.Close();
                    Properties.Settings.Default.SonCalisma = DateTime.Now;
                    Properties.Settings.Default.Save();
                }
                string soyad = GetLastName(girilenName);
                form5 = new Menu(form7, uyelerupdate);
                form5.Show();
                form5.labelAdminName.Text = girilenName;
                form5.bunifuPictureBox1.Image = Image.FromFile(newImagePath);
                form5.bunifuPictureBox2.Image = Image.FromFile(newImagePath2);
                form5.bunifuPictureBox2.Image.Tag = "beta.png";
                form5.label7.Text = "Hoşgeldin \n" + girilenName + " " + soyad;
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
            }
        }
        private string GetLastName(string username)
        {
            string Soyad = string.Empty;

            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';"))
            {
                connection.Open();
                string query = "SELECT Soyad FROM admin WHERE Name = @isim";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isim", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Soyad = reader["Soyad"].ToString();
                        }
                    }
                }
            }

            return Soyad;
        }
        private bool Dogrula(string kullaniciAdi, string sifre)
        {
            bool dogrulamaSonucu = false;
            string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = conn;
                    string sorgu = "SELECT COUNT(*) FROM admin WHERE Name = @Name AND Password = @Password";
                    command.Parameters.AddWithValue("@Name", kullaniciAdi);
                    command.Parameters.AddWithValue("@Password", sifre);
                    command.CommandText = sorgu;
                    conn.Open();
                    int rowCount = Convert.ToInt32(command.ExecuteScalar());
                    dogrulamaSonucu = (rowCount > 0);
                    conn.Close();
                }
            }

            return dogrulamaSonucu;
        }
    }
}
