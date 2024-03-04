using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class UyelerUpdate : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private string selectedDesign = "NoNe";
        Menu menu;
        public UyelerUpdate(Menu menureferences)
        {
            menu = menureferences;
            InitializeComponent();
            InitializeDatabaseConnection();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
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
        private void button1_Click(object sender, EventArgs e)
        {
            string query;
            string ad = textBox1.Text;
            string soyad = textBox4.Text;
            string kk = textBox5.Text;
            string okulno = textBox3.Text;
            string dogum = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            int id2;

            if (!int.TryParse(textBox8.Text, out id2))
            {
                MessageBox.Show("ID Geçerli değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox1.Text == "statu seç")
            {
                MessageBox.Show("Statu seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int statu = comboBox1.SelectedIndex;
            string statu_adi = "Öğrenci Değil";

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
                if (!IsOnlyNumbers(okulno))
                {
                    MessageBox.Show("Okul No alanına sadece sayı girebilirsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                okulno = textBox3.Text;
            }
            else
            {
                okulno = statu_adi;
            }

            using (MySqlCommand cmd = new MySqlCommand())
            {
                query = "UPDATE uyeler SET Kk = @kk, Okulno = @okulno, Ad = @ad, Soyad = @soyad, statu = @statu, DogumT = @dogum WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", id2);
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
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        textBox1.Text = ""; textBox4.Text = ""; textBox8.Text = ""; textBox3.Text = ""; textBox5.Text = ""; comboBox1.SelectedIndex = -1;
                        MessageBox.Show("Veri başarıyla güncellendi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        menu.bunifuPanel1.Visible = true;
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme işlemi başarısız oldu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.ToString(), "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void UyelerUpdate_Load_1(object sender, EventArgs e)
        {


            VerileriComboBoxaYazdir();
        }
        private void CreatePdfDesign1(string ad, string soyad, string statu, string filePath, string kk)
        {
            ad = textBox1.Text;
            soyad = textBox4.Text;
            kk = textBox5.Text;
            statu = (string)comboBox1.SelectedItem;

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
                    document.Add(new Paragraph("Kimlik Kartı: " + kk));
                    document.Add(new Paragraph("Ad: " + ad));
                    document.Add(new Paragraph("Soyad: " + soyad));
                    document.Add(new Paragraph("Statu: " + statu));
                    document.Add(new Paragraph($"Olusturulma Tarihi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10));
                }
                MessageBox.Show($"PDF dosyası başarıyla oluşturuldu, 1. tasarım kullanıldı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void CreatePdfDesign2(string ad, string soyad, string statu, string filePath, string kk)
        {
            ad = textBox1.Text;
            soyad = textBox4.Text;
            kk = textBox5.Text;
            statu = (string)comboBox1.SelectedItem;

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
                    table.AddCell("Ad:");
                    table.AddCell(ad);
                    table.AddCell("Soyad:");
                    table.AddCell(soyad);
                    table.AddCell("Statu:");
                    table.AddCell(statu);
                    table.AddCell("Kimlik Kartı");
                    table.AddCell(kk);
                    document.Add(table);
                    document.Add(new Paragraph($"Olusturulma Tarihi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(10));
                }
            }

            MessageBox.Show($"PDF dosyası başarıyla oluşturuldu, 2. tasarım kullanıldı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
        private void CreatePdfDesign3(string ad, string soyad, string statu, string filePath, string kk)
        {
            ad = textBox1.Text;
            soyad = textBox4.Text;
            kk = textBox5.Text;
            statu = (string)comboBox1.SelectedItem;

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
                    contentCanvas.ShowTextAligned($"Kimlik Kartı: {kk}", x, y, TextAlignment.LEFT);
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
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

        private void button6_Click_1(object sender, EventArgs e)
        {
            if (selectedDesign != "NoNe")
            {
                string ad = textBox1.Text;
                string soyad = textBox4.Text;
                string kk = textBox5.Text;
                string statu = (string)comboBox1.SelectedItem;
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pdfFileName = $"{ad}_{soyad}_Bilgileri.pdf";
                string pdfFilePath = Path.Combine(desktopPath, pdfFileName);

                switch (selectedDesign)
                {
                    case "Tasarım1":
                        CreatePdfDesign1(kk, ad, soyad, statu, pdfFilePath);
                        break;
                    case "Tasarım2":
                        CreatePdfDesign2(kk, ad, soyad, statu, pdfFilePath);
                        break;
                    case "Tasarım3":
                        CreatePdfDesign3(kk, ad, soyad, statu, pdfFilePath);
                        break;
                }



            }
            else
            {
                MessageBox.Show("Lütfen bir pdf tasarımı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
