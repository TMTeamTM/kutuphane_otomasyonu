using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
namespace KutuphaneSistemi
{
    public partial class Form7 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        public Form7()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string isbn = textBox1.Text;
            string kitapAdi = textBox2.Text;
            string yazar = textBox3.Text;
            string konu = textBox4.Text;
            string kategori = textBox5.Text;
            string sayfa = textBox6.Text;
            string spot = textBox7.Text;
            string id = textBox8.Text;
            if (!IsNumeric(sayfa))
            {
                MessageBox.Show("Sayfa alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsOnlyLetters(kitapAdi) || !IsOnlyLetters(yazar) || !IsOnlyLetters(konu) || !IsOnlyLetters(kategori))
            {
                MessageBox.Show("Kitap Adı, Yazar, Konu ve Kategori alanlarına sadece harf girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(kitapAdi) || string.IsNullOrWhiteSpace(yazar) ||
                string.IsNullOrWhiteSpace(konu) || string.IsNullOrWhiteSpace(kategori) || string.IsNullOrWhiteSpace(sayfa) || string.IsNullOrWhiteSpace(spot))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
            }
            else
            {
                string oduncCheckQuery = "SELECT COUNT(*) FROM odunc_kitaplar WHERE Kitap_ID = @KitapID AND (Alinan_Tarih IS NULL)";
                using (MySqlCommand oduncCheckCmd = new MySqlCommand(oduncCheckQuery, connection))
                {
                    oduncCheckCmd.Parameters.AddWithValue("@KitapID", id);

                    try
                    {
                        connection.Open();
                        int oduncRowCount = Convert.ToInt32(oduncCheckCmd.ExecuteScalar());

                        if (oduncRowCount > 0)
                        {
                            MessageBox.Show("Bu kitap ödünç verildiği için güncellenemez.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ödünç durumu kontrol edilirken hata oluştu: " + ex.Message);
                        return;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                string query = "UPDATE kitap SET Name = @kitapAdi, ISBN = @isbn, Author = @yazar, Description = @konu, Category = @kategori, No_of_pages = @sayfa, Spot = @spot WHERE ID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@kitapAdi", kitapAdi);
                    cmd.Parameters.AddWithValue("@yazar", yazar);
                    cmd.Parameters.AddWithValue("@konu", konu);
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@sayfa", sayfa);
                    cmd.Parameters.AddWithValue("@spot", spot);
                    cmd.Parameters.AddWithValue("@isbn", isbn);

                    try
                    {
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Kitap bilgileri başarıyla güncellendi.");
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme işlemi başarısız oldu.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message);

                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Inner Hata: " + ex.InnerException.Message);
                        }
                    }
                    finally
                    {
                        connection.Close();
                        button1.Enabled = false;
                    }
                }
            }
        }
        bool IsOnlyLetters(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.All(char.IsLetter);
        }
        bool IsNumeric(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

