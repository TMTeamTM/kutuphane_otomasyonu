using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
namespace KutuphaneSistemi
{
    public partial class Form3 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        public Form3()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }
        private void DataGöster()
        {
            string query = "SELECT * FROM kitap";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri çekme hatası: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            DataGöster();
        }
        private int GetPreviousMaxID()
        {
            string query = "SELECT MAX(ID) FROM kitap";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
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
                return -1;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int previousMaxID = GetPreviousMaxID();
            int newID = previousMaxID + 1;
            string isbn = textBox1.Text;
            string kitapAdi = textBox2.Text;
            string yazar = textBox3.Text;
            string konu = textBox4.Text;
            string kategori = textBox5.Text;
            string sayfa = textBox6.Text;
            string spot = textBox7.Text;
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
                string query = "INSERT INTO kitap (ID,ISBN, Name, Author, Description, Category, No_of_pages, Spot) VALUES (@newID,@isbn, @kitapAdi, @yazar, @konu, @kategori, @sayfa, @spot)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@newID", newID);
                    cmd.Parameters.AddWithValue("@isbn", isbn);
                    cmd.Parameters.AddWithValue("@kitapAdi", kitapAdi);
                    cmd.Parameters.AddWithValue("@yazar", yazar);
                    cmd.Parameters.AddWithValue("@konu", konu);
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@sayfa", sayfa);
                    cmd.Parameters.AddWithValue("@spot", spot);
                    try
                    {
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Veri başarıyla eklendi.");
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Ekleme işlemi başarısız oldu.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message);
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
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

    }
}

