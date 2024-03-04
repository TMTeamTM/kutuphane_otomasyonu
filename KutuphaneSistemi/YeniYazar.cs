using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class YeniYazar : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private Yazarlar yazar;
        public YeniYazar(Yazarlar yazarreferences)
        {
            yazar = yazarreferences;
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {

            string query;
            string ad = bunifuTextBox1.Text;
            string telno = bunifuTextBox2.Text;
            string dogum = bunifuDatePicker1.Value.ToString("yyyy-MM-dd");

            if (!IsNumber(telno))
            {
                MessageBox.Show("Telefon Numarası alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            if (string.IsNullOrWhiteSpace(ad))
            {
                MessageBox.Show("Ad ve soyad girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string checkQuery = "SELECT COUNT(*) FROM yazarlar WHERE Ad = @ad";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@ad", ad);

                try
                {
                    connection.Open();
                    int existingRecordsCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existingRecordsCount > 0)
                    {
                        MessageBox.Show("Bu yazar zaten mevcut. Aynı bilgilerle tekrar ekleyemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            query = "INSERT INTO yazarlar (Ad, Tel_No, Dogum_T, resim) VALUES (@ad, @telno, @dogum, @resim)";
                            cmd.CommandText = query;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@telno", telno);
                            cmd.Parameters.AddWithValue("@resim", imageBytes);
                            cmd.Parameters.AddWithValue("@dogum", dogum);

                            try
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Veri başarıyla eklendi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    yazar.flowLayoutPanel1.Controls.Clear();
                                    yazar.kartlar();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Ekleme işlemi başarısız oldu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Hata: " + ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

        }
        bool IsNumber(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit);
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog.FileName;
        }
    }
}
