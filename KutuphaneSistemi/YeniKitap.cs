using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace KutuphaneSistemi
{
    public partial class YeniKitap : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        Menu menu;
        public YeniKitap(Menu menureferences)
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            menu = menureferences;
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

            if (!IsNumeric(sayfa))
            {
                MessageBox.Show("Sayfa alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(kitapAdi) || string.IsNullOrWhiteSpace(yazar) ||
                string.IsNullOrWhiteSpace(konu) || string.IsNullOrWhiteSpace(kategori) || string.IsNullOrWhiteSpace(sayfa) || string.IsNullOrWhiteSpace(spot))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
            }
            else
            {
                string checkQuery = "SELECT COUNT(*) FROM kitap WHERE Name = @kitapAdi";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@kitapAdi", kitapAdi);
                    try
                    {
                        connection.Open();
                        int existingRecordsCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingRecordsCount > 0)
                        {
                            MessageBox.Show("Bu kayıt zaten mevcut. Aynı bilgilerle tekrar ekleyemezsiniz.");
                        }
                        else
                        {
                            string query = "INSERT INTO kitap (ID, ISBN, Name, Author, Description, Category, No_of_pages, Spot, resim) VALUES (@newID, @isbn, @kitapAdi, @yazar, @konu, @kategori, @sayfa, @spot, @resim)";
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
                                cmd.Parameters.AddWithValue("@resim", imageBytes);

                                try
                                {
                                    int rowsAffected = cmd.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Veri başarıyla eklendi.");
                                        menu.bunifuPanel1.Visible = true;
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog.FileName;
            resimTxt.Text = openFileDialog.FileName;
        }
    }
}

