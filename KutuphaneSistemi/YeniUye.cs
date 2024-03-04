using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class YeniUye : Form
    {

        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private string selectedDesign = "NoNe";
        Menu menu;
        public YeniUye(Menu menureferences)
        {
            menu = menureferences;
            InitializeComponent();
            InitializeDatabaseConnection();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }
        private void DataGöster()
        {
            string query = "SELECT uyeler.ID,uyeler.Ad,uyeler.Soyad,statu.statu_adi,uyeler.DogumT,uyeler.Kk,uyeler.Okulno " +
                "FROM uyeler JOIN statu ON uyeler.statu = statu.ID";
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

        private bool CheckIfIDExists()
        {
            int id = Convert.ToInt32(textBox8.Text);
            string checkQuery = "SELECT COUNT(*) FROM uyeler WHERE ID = @id";

            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                connection.Close();
                return count > 0;
            }
        }

        private int GetPreviousMaxID()
        {
            string query = "SELECT MAX(ID) FROM uyeler";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        textBox8.Text = result + 1.ToString();
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
        private void button1_Click_1(object sender, EventArgs e)
        {
            int previousMaxID = GetPreviousMaxID();
            string query;
            string ad = textBox1.Text;
            string soyad = textBox4.Text;
            string kk = textBox5.Text;
            string okulno;
            string statu_adi = "Öğrenci Değil";
            string dogum = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            int id2 = previousMaxID + 1;
            int statu = comboBox1.SelectedIndex;

            if (!IsOnlyLetters(ad) || !IsOnlyLetters(soyad))
            {
                MessageBox.Show("Ad ve Soyad alanlarına sadece harf girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsOnlyNumbers(kk))
            {
                MessageBox.Show("Kimlik Kartı alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1.SelectedIndex == 1)
            {
                okulno = textBox3.Text;
                if (!IsOnlyNumbers(okulno))
                {
                    MessageBox.Show("Okul No alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                okulno = statu_adi;
            }

            string checkQuery = "SELECT COUNT(*) FROM uyeler WHERE Ad = @ad AND Soyad = @soyad";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@ad", ad);
                checkCmd.Parameters.AddWithValue("@soyad", soyad);

                try
                {
                    connection.Open();
                    int existingRecordsCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existingRecordsCount > 0)
                    {
                        MessageBox.Show("Bu kayıt zaten mevcut. Aynı bilgilerle tekrar ekleyemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            query = "INSERT INTO uyeler (ID, Kk, Okulno, Ad, Soyad, statu, DogumT) VALUES (@id, @kk, @okulno, @ad, @soyad, @statu, @dogum)";
                            cmd.Parameters.AddWithValue("@id", id2);
                            textBox1.Text = ""; textBox4.Text = ""; textBox8.Text = ""; textBox3.Text = ""; textBox5.Text = ""; comboBox1.SelectedIndex = -1;

                            cmd.CommandText = query;
                            cmd.Connection = connection;
                            cmd.Parameters.AddWithValue("@kk", kk);
                            cmd.Parameters.AddWithValue("@okulno", okulno);
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@soyad", soyad);
                            cmd.Parameters.AddWithValue("@statu", statu);
                            cmd.Parameters.AddWithValue("@dogum", dogum);

                            try
                            {
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Veri başarıyla eklendi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    menu.bunifuPanel1.Visible = true;
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
        bool IsOnlyLetters(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(char.IsLetter);
        }
        bool IsOnlyNumbers(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(char.IsDigit);
        }

        private void Form4_Load_1(object sender, EventArgs e)
        {

            DataGöster();
            VerileriComboBoxaYazdir();
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = true;
            string query = "SELECT uyeler.ID,uyeler.Ad,uyeler.Soyad,statu.statu_adi,uyeler.DogumT,uyeler.Kk,uyeler.Okulno " +
                "FROM uyeler JOIN statu ON uyeler.statu = statu.ID";

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
        public void VerileriComboBoxaYazdir()
        {
            try
            {
                comboBox2.Items.Add("Tasarım1");
                comboBox2.Items.Add("Tasarım2");
                comboBox2.Items.Add("Tasarım3");
                connection.Open();
                string query = "SELECT * FROM statu";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    string veri = reader["statu_adi"].ToString();

                    comboBox1.Items.Add(veri);
                    i++;
                }
                reader.Close();
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
        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Arama();
        }
        private void Arama()
        {
            string ad = textBox1.Text;
            string soyad = textBox4.Text;
            string selectedStatus = comboBox1.SelectedItem?.ToString();
            int statum = comboBox1.SelectedIndex;
            if (string.IsNullOrEmpty(ad) && string.IsNullOrEmpty(soyad) && string.IsNullOrEmpty(selectedStatus))
            {
                MessageBox.Show("Lütfen en az bir alanı doldurun.");
                return;
            }

            string query = "SELECT uyeler.*, statu.statu_adi FROM uyeler LEFT JOIN statu ON uyeler.statu = statu.ID WHERE 1=1";

            if (!string.IsNullOrEmpty(ad))
            {
                query += " AND Ad LIKE @ad";
            }

            if (!string.IsNullOrEmpty(soyad))
            {
                query += " AND Soyad LIKE @soyad";
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                query += " AND statu_adi = @statu_adi";
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(ad))
            {
                cmd.Parameters.AddWithValue("@ad", "%" + ad + "%");
            }

            if (!string.IsNullOrEmpty(soyad))
            {
                cmd.Parameters.AddWithValue("@soyad", "%" + soyad + "%");
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                cmd.Parameters.AddWithValue("@statu_adi", selectedStatus);
            }

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
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (selectedDesign != "NoNe")
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    string id = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                    string ad = dataGridView1.SelectedRows[0].Cells["Ad"].Value.ToString();
                    string soyad = dataGridView1.SelectedRows[0].Cells["Soyad"].Value.ToString();
                    string statu = dataGridView1.SelectedRows[0].Cells["statu_adi"].Value.ToString();
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string pdfFileName = $"{ad}_{soyad}_Bilgileri.pdf";
                    string pdfFilePath = Path.Combine(desktopPath, pdfFileName);

                    switch (selectedDesign)
                    {
                        case "Tasarım1":
                            CreatePdfDesign1(id, ad, soyad, statu, pdfFilePath);
                            break;
                        case "Tasarım2":
                            CreatePdfDesign2(id, ad, soyad, statu, pdfFilePath);
                            break;
                        case "Tasarım3":
                            CreatePdfDesign3(id, ad, soyad, statu, pdfFilePath);
                            break;
                    }


                }
                else
                {
                    MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir pdf tasarımı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CreatePdfDesign1(string id, string ad, string soyad, string statu, string filePath)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                id = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                ad = dataGridView1.SelectedRows[0].Cells["Ad"].Value.ToString();
                soyad = dataGridView1.SelectedRows[0].Cells["Soyad"].Value.ToString();
                statu = dataGridView1.SelectedRows[0].Cells["statu_adi"].Value.ToString();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pdfFileName = $"{ad}_{soyad}_Bilgileri.pdf";
                string pdfFilePath = Path.Combine(desktopPath, pdfFileName);

                using (PdfWriter writer = new PdfWriter(pdfFilePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        document.Add(new Paragraph("Kisi Bilgileri")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(16)
                            .SetBold());
                        document.Add(new Paragraph("ID: " + id));
                        document.Add(new Paragraph("Ad: " + ad));
                        document.Add(new Paragraph("Soyad: " + soyad));
                        document.Add(new Paragraph("Statu: " + statu));
                        document.Add(new Paragraph($"Olusturulma Tarihi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(10));
                    }
                }

                MessageBox.Show($"PDF dosyası başarıyla oluşturuldu, 1. tasarım kullanıldı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CreatePdfDesign2(string id, string ad, string soyad, string statu, string filePath)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                id = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                ad = dataGridView1.SelectedRows[0].Cells["Ad"].Value.ToString();
                soyad = dataGridView1.SelectedRows[0].Cells["Soyad"].Value.ToString();
                statu = dataGridView1.SelectedRows[0].Cells["statu_adi"].Value.ToString();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pdfFileName = $"{ad}_{soyad}_Bilgileri.pdf";
                string pdfFilePath = Path.Combine(desktopPath, pdfFileName);

                using (PdfWriter writer = new PdfWriter(pdfFilePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        document.Add(new Paragraph("Kisi Bilgileri")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(16)
                            .SetBold());
                        Table table = new Table(2, false);
                        table.SetWidth(UnitValue.CreatePercentValue(80));
                        table.AddCell("ID:");
                        table.AddCell(id);
                        table.AddCell("Ad:");
                        table.AddCell(ad);
                        table.AddCell("Soyad:");
                        table.AddCell(soyad);
                        table.AddCell("Statu:");
                        table.AddCell(statu);
                        document.Add(table);
                        document.Add(new Paragraph($"Olusturulma Tarihi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(10));
                    }
                }

                MessageBox.Show($"PDF dosyası başarıyla oluşturuldu, 2. tasarım kullanıldı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CreatePdfDesign3(string id, string ad, string soyad, string statu, string filePath)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                id = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();
                ad = dataGridView1.SelectedRows[0].Cells["Ad"].Value.ToString();
                soyad = dataGridView1.SelectedRows[0].Cells["Soyad"].Value.ToString();
                statu = dataGridView1.SelectedRows[0].Cells["statu_adi"].Value.ToString();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pdfFileName = $"{ad}_{soyad}_Bilgileri.pdf";
                string pdfFilePath = Path.Combine(desktopPath, pdfFileName);

                using (PdfWriter writer = new PdfWriter(pdfFilePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        PdfPage page = pdf.AddNewPage();
                        PdfCanvas canvas = new PdfCanvas(page);
                        Canvas contentCanvas = new Canvas(canvas, page.GetPageSize());
                        float x = 50f;
                        float y = page.GetPageSize().GetTop() - 100f;
                        float lineHeight = 20f;
                        contentCanvas.ShowTextAligned("Kisi Bilgileri", page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetTop() - 20, TextAlignment.CENTER);
                        contentCanvas.ShowTextAligned($"ID: {id}", x, y, TextAlignment.LEFT);
                        contentCanvas.ShowTextAligned($"Ad: {ad}", x, y - lineHeight, TextAlignment.LEFT);
                        contentCanvas.ShowTextAligned($"Soyad: {soyad}", x, y - 2 * lineHeight, TextAlignment.LEFT);
                        contentCanvas.ShowTextAligned($"Statu: {statu}", x, y - 3 * lineHeight, TextAlignment.LEFT);
                        contentCanvas.ShowTextAligned($"Olusturulma Tarihi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}", x, y - 5 * lineHeight, TextAlignment.LEFT);
                        contentCanvas.ShowTextAligned($"Olusturan Bilgisayar Adı: {Environment.MachineName}", x, 30, TextAlignment.LEFT);
                        contentCanvas.Close();
                    }
                }
                MessageBox.Show($"PDF dosyası başarıyla oluşturuldu, 3. tasarım kullanıldı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                selectedDesign = "Tasarım1";
            }
            if (comboBox2.SelectedIndex == 1)
            {
                selectedDesign = "Tasarım2";
            }
            if (comboBox2.SelectedIndex == 2)
            {
                selectedDesign = "Tasarım3";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    return;
                }

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                if (selectedRow.Cells["ID"].Value == null ||
                    selectedRow.Cells["Ad"].Value == null ||
                    selectedRow.Cells["Kk"].Value == null ||
                    selectedRow.Cells["Soyad"].Value == null ||
                    selectedRow.Cells["Okulno"].Value == null ||
                    selectedRow.Cells["DogumT"].Value == null ||
                    selectedRow.Cells["statu_adi"].Value == null)
                {
                    MessageBox.Show("Seçilen satır boş veri içeriyor. Lütfen geçerli bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                textBox8.Text = selectedRow.Cells["ID"].Value.ToString();
                textBox1.Text = selectedRow.Cells["Ad"].Value.ToString();
                textBox5.Text = selectedRow.Cells["Kk"].Value.ToString();
                textBox4.Text = selectedRow.Cells["Soyad"].Value.ToString();
                textBox3.Text = selectedRow.Cells["Okulno"].Value.ToString();

                DateTime? dateValue = selectedRow.Cells["DogumT"].Value as DateTime?;
                if (dateValue.HasValue)
                {
                    dateTimePicker1.Value = dateValue.Value;
                }
                else
                {
                    dateTimePicker1.Value = DateTime.Today;
                }

                object statuAdiValue = selectedRow.Cells["statu_adi"].Value;
                if (statuAdiValue != null)
                {
                    switch (statuAdiValue.ToString())
                    {
                        case "Ögretmen":
                            comboBox1.SelectedIndex = 0;
                            break;
                        case "Diger":
                            comboBox1.SelectedIndex = 2;
                            break;
                        case "Ögrenci":
                            comboBox1.SelectedIndex = 1;
                            break;
                        case "idare":
                            comboBox1.SelectedIndex = 3;
                            break;
                        default:
                            comboBox1.SelectedIndex = -1;
                            break;
                    }
                }
                else
                {
                    comboBox1.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                label3.Visible = true; textBox3.Visible = true;
                textBox3.Text = "";
            }
            else
            {
                label3.Visible = false; textBox3.Visible = false;
            }

        }
    }
}


