using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class YazarUpdate : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private Yazarlar yazar;
        public YazarUpdate(Yazarlar yazarreferences)
        {
            yazar = yazarreferences;
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            ResimCek();
        }
        public void ResimCek()
        {
            string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
            string query = "SELECT resim FROM yazarlar WHERE yazar_ID = @YazarID";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YazarID", textBox1.Text);

                    try
                    {
                        connection.Open();
                        byte[] imageBytes = (byte[])command.ExecuteScalar();

                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            pictureBox1.Image = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message);
                    }
                }
            }
        }


        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog.FileName;
        }
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            string ad = bunifuTextBox1.Text;
            string id = textBox1.Text;
            string telno = bunifuTextBox2.Text;
            string dogum = bunifuDatePicker1.Value.ToString("yyyy-MM-dd");
            byte[] imageBytes = null;
            Image image = pictureBox1.Image;

            if (image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }
            else
            {
                MessageBox.Show("Resim seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsNumber(telno))
            {
                MessageBox.Show("Telefon Numarası alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "UPDATE yazarlar SET Ad = @ad,Tel_No = @telno,Dogum_T = @dogum, resim = @resim WHERE yazar_ID = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@ad", ad);
                cmd.Parameters.AddWithValue("@resim", imageBytes);
                cmd.Parameters.AddWithValue("@telno", telno);
                cmd.Parameters.AddWithValue("@dogum", dogum);

                try
                {
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Yazar bilgileri başarıyla güncellendi.");
                        yazar.flowLayoutPanel1.Controls.Clear();
                        yazar.kartlar();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme işlemi başarısız oldu.");
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("MySQL Hatası: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Genel Hata: " + ex.Message);

                    if (ex.InnerException != null)
                    {
                        MessageBox.Show("Inner Hata: " + ex.InnerException.Message);
                    }
                }
            }
        }
        bool IsNumber(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit);
        }

    }
}
