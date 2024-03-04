using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Bunifu;
using Bunifu.UI.WinForms.BunifuButton;
namespace KutuphaneSistemi
{
    public partial class Yazarlar : Form
    {
        string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        YazarUpdate yazarlarupdate;
        public Yazarlar(YazarUpdate yazarupdatereferences)
        {
            yazarlarupdate = yazarupdatereferences;
            InitializeComponent();
            kartlar();
            Yedek();

        }
        private void Yedek()
        {
            string masaustuDizin = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dosyaYolu = Path.Combine(masaustuDizin, "kitap.sql");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    using (MySqlBackup backup = new MySqlBackup(command))
                    {
                        try
                        {
                            command.Connection = conn;
                            conn.Open();
                            if (!File.Exists(dosyaYolu))
                            {
                                using (File.Create(dosyaYolu)) { }
                            }
                            backup.ExportToFile(dosyaYolu);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Yedekleme hatası: " + ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }

        private void YedekYukle()
        {
            string masaustuDizin = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dosyaYolu = Path.Combine(masaustuDizin, "kitap.sql");
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    using (MySqlBackup backup = new MySqlBackup(command))
                    {
                        try
                        {
                            command.Connection = conn;
                            conn.Open();
                            backup.ImportFromFile(dosyaYolu);
                            // MessageBox.Show("Geri alma başarılı!", "Kütüphane Sistemi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            kartlar();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Yedek geri yükleme hatası: " + ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }
        public void kartlar()
        {
            flowLayoutPanel1.Controls.Clear();
            string yazarQuery = "SELECT * FROM yazarlar";
            string kitapQuery = "SELECT COUNT(*) FROM kitap WHERE Author = @YazarAdi";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand yazarCommand = new MySqlCommand(yazarQuery, connection))
                {
                    using (MySqlDataReader yazarReader = yazarCommand.ExecuteReader())
                    {
                        while (yazarReader.Read())
                        {
                            Bunifu.Framework.UI.BunifuCards card = new Bunifu.Framework.UI.BunifuCards();
                            card.Size = new System.Drawing.Size(200, 150);
                            card.BackColor = System.Drawing.Color.WhiteSmoke;
                            card.Margin = new Padding(10);

                            PictureBox pictureBox = new PictureBox();
                            byte[] imageBytes = (byte[])yazarReader["resim"];
                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    Image image = Image.FromStream(ms);
                                    pictureBox.Image = new Bitmap(image, new Size(200, 130));
                                }
                            }
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox.Dock = DockStyle.Top;

                            Label nameLabel = new Label();
                            nameLabel.Text = $"{yazarReader["Ad"]}";
                            nameLabel.Dock = DockStyle.Top;
                            nameLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                            nameLabel.TextAlign = ContentAlignment.MiddleCenter;

                            Label yazarLabel = new Label();
                            yazarLabel.Text = $"{yazarReader["yazar_ID"]}";
                            yazarLabel.Visible = false;
                            yazarLabel.Dock = DockStyle.Top;
                            yazarLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                            yazarLabel.TextAlign = ContentAlignment.MiddleCenter;
                            Label telLabel = new Label();
                            telLabel.Text = $"{yazarReader["Tel_No"]}";
                            telLabel.Visible = false;
                            telLabel.Dock = DockStyle.Top;
                            telLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                            telLabel.TextAlign = ContentAlignment.MiddleCenter;
                            Label dogumT = new Label();
                            dogumT.Text = $"{yazarReader["Dogum_T"]}";
                            dogumT.Visible = false;
                            dogumT.Dock = DockStyle.Top;
                            dogumT.Font = new Font("Arial", 10, FontStyle.Italic);
                            dogumT.TextAlign = ContentAlignment.MiddleCenter;

                            using (MySqlConnection kitapConnection = new MySqlConnection(connectionString))
                            {
                                kitapConnection.Open();
                                using (MySqlCommand kitapCommand = new MySqlCommand(kitapQuery, kitapConnection))
                                {
                                    kitapCommand.Parameters.AddWithValue("@YazarAdi", yazarReader["Ad"]);
                                    int kitapSayisi = Convert.ToInt32(kitapCommand.ExecuteScalar());
                                    Label kitapLabel = new Label();
                                    kitapLabel.Text = $"Kitap Sayısı: {kitapSayisi}";
                                    kitapLabel.Dock = DockStyle.Bottom;
                                    kitapLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                                    kitapLabel.TextAlign = ContentAlignment.MiddleCenter;
                                    BunifuButton deleteButton = new BunifuButton();
                                    deleteButton.IdleFillColor = Color.DarkRed;
                                    deleteButton.Dock = DockStyle.Top;
                                    deleteButton.Cursor = Cursors.Hand;
                                    deleteButton.Text = "Sil";
                                    deleteButton.Size = new Size(30, 20);
                                    deleteButton.Click += (sender, e) =>
                                    {
                                        DialogResult result = MessageBox.Show("Bu yazarı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                        if (result == DialogResult.Yes)
                                        {
                                            SilYazari(yazarLabel.Text);
                                            flowLayoutPanel1.Controls.Remove(card);
                                        }
                                    };
                                    BunifuButton updateButton = new BunifuButton();
                                    updateButton.IdleFillColor = Color.DarkGreen;
                                    updateButton.Dock = DockStyle.Top;
                                    updateButton.Cursor = Cursors.Hand;
                                    updateButton.Text = "Güncelle";
                                    updateButton.Size = new Size(30, 20);
                                    updateButton.Click += (sender, e) =>
                                    {
                                        YazarUpdate yazarlarupdate = new YazarUpdate(this);
                                        yazarlarupdate.Show();
                                        yazarlarupdate.textBox1.Text = yazarLabel.Text;
                                        yazarlarupdate.bunifuTextBox1.Text = nameLabel.Text;
                                        yazarlarupdate.bunifuTextBox2.Text = telLabel.Text;
                                        if (DateTime.TryParse(dogumT.Text, out DateTime dogumTarihi))
                                        {
                                            yazarlarupdate.bunifuDatePicker1.Value = dogumTarihi;
                                        }
                                    };
                                    card.Controls.Add(pictureBox);
                                    card.Controls.Add(nameLabel);
                                    card.Controls.Add(yazarLabel);
                                    card.Controls.Add(kitapLabel);
                                    card.Controls.Add(deleteButton);
                                    card.Controls.Add(updateButton);

                                    flowLayoutPanel1.Controls.Add(card);
                                }
                            }
                        }
                    }
                }
            }

            flowLayoutPanel1.AutoScroll = true;
        }
        private void SilYazari(string yazarAdi)
        {
            string silQuery = "DELETE FROM yazarlar WHERE yazar_ID = @YazarAdi";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand silCommand = new MySqlCommand(silQuery, connection))
                {
                    silCommand.Parameters.AddWithValue("@YazarAdi", yazarAdi);
                    silCommand.ExecuteNonQuery();
                }
            }
        }
        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            string searchText = bunifuTextBox1.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                flowLayoutPanel1.Controls.Clear();
                kartlar();
                return;
            }
            string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
            string query = "SELECT * FROM yazarlar WHERE Ad LIKE @SearchText";
            string kitapQuery = "SELECT COUNT(*) FROM kitap WHERE Author = @YazarAdi";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                    using (MySqlDataReader yazarReader = command.ExecuteReader())
                    {
                        flowLayoutPanel1.Controls.Clear();

                        while (yazarReader.Read())
                        {
                            Bunifu.Framework.UI.BunifuCards card = new Bunifu.Framework.UI.BunifuCards();
                            card.Size = new System.Drawing.Size(200, 150);
                            card.BackColor = System.Drawing.Color.WhiteSmoke;
                            card.Margin = new Padding(10);

                            PictureBox pictureBox = new PictureBox();
                            byte[] imageBytes = (byte[])yazarReader["resim"];
                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    Image image = Image.FromStream(ms);
                                    pictureBox.Image = new Bitmap(image, new Size(200, 130));
                                }
                            }
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            pictureBox.Dock = DockStyle.Top;

                            Label nameLabel = new Label();
                            nameLabel.Text = $"{yazarReader["Ad"]}";
                            nameLabel.Dock = DockStyle.Top;
                            nameLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                            nameLabel.TextAlign = ContentAlignment.MiddleCenter;

                            Label yazarLabel = new Label();
                            yazarLabel.Text = $"{yazarReader["yazar_ID"]}";
                            yazarLabel.Visible = false;
                            yazarLabel.Dock = DockStyle.Top;
                            yazarLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                            yazarLabel.TextAlign = ContentAlignment.MiddleCenter;
                            Label telLabel = new Label();
                            telLabel.Text = $"{yazarReader["Tel_No"]}";
                            telLabel.Visible = false;
                            telLabel.Dock = DockStyle.Top;
                            telLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                            telLabel.TextAlign = ContentAlignment.MiddleCenter;
                            Label dogumT = new Label();
                            dogumT.Text = $"{yazarReader["Dogum_T"]}";
                            dogumT.Visible = false;
                            dogumT.Dock = DockStyle.Top;
                            dogumT.Font = new Font("Arial", 10, FontStyle.Italic);
                            dogumT.TextAlign = ContentAlignment.MiddleCenter;

                            using (MySqlConnection kitapConnection = new MySqlConnection(connectionString))
                            {
                                kitapConnection.Open();
                                using (MySqlCommand kitapCommand = new MySqlCommand(kitapQuery, kitapConnection))
                                {
                                    kitapCommand.Parameters.AddWithValue("@YazarAdi", yazarReader["Ad"]);
                                    int kitapSayisi = Convert.ToInt32(kitapCommand.ExecuteScalar());
                                    Label kitapLabel = new Label();
                                    kitapLabel.Text = $"Kitap Sayısı: {kitapSayisi}";
                                    kitapLabel.Dock = DockStyle.Bottom;
                                    kitapLabel.Font = new Font("Arial", 10, FontStyle.Italic);
                                    kitapLabel.TextAlign = ContentAlignment.MiddleCenter;
                                    BunifuButton deleteButton = new BunifuButton();
                                    deleteButton.IdleFillColor = Color.DarkRed;
                                    deleteButton.Dock = DockStyle.Top;
                                    deleteButton.Cursor = Cursors.Hand;
                                    deleteButton.Text = "Sil";
                                    deleteButton.Size = new Size(30, 20);
                                    deleteButton.Click += (senderr, ee) =>
                                    {
                                        DialogResult result = MessageBox.Show("Bu yazarı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                        if (result == DialogResult.Yes)
                                        {
                                            SilYazari(yazarLabel.Text);
                                            flowLayoutPanel1.Controls.Remove(card);
                                        }
                                    };
                                    BunifuButton updateButton = new BunifuButton();
                                    updateButton.IdleFillColor = Color.DarkGreen;
                                    updateButton.Dock = DockStyle.Top;
                                    updateButton.Cursor = Cursors.Hand;
                                    updateButton.Text = "Güncelle";
                                    updateButton.Size = new Size(30, 20);
                                    updateButton.Click += (senderr, ee) =>
                                    {
                                        YazarUpdate yazarlarupdate = new YazarUpdate(this);
                                        yazarlarupdate.Show();
                                        yazarlarupdate.textBox1.Text = yazarLabel.Text;
                                        yazarlarupdate.bunifuTextBox1.Text = nameLabel.Text;
                                        yazarlarupdate.bunifuTextBox2.Text = telLabel.Text;
                                        if (DateTime.TryParse(dogumT.Text, out DateTime dogumTarihi))
                                        {
                                            yazarlarupdate.bunifuDatePicker1.Value = dogumTarihi;
                                        }
                                    };
                                    card.Controls.Add(pictureBox);
                                    card.Controls.Add(nameLabel);
                                    card.Controls.Add(yazarLabel);
                                    card.Controls.Add(kitapLabel);
                                    card.Controls.Add(deleteButton);
                                    card.Controls.Add(updateButton);

                                    flowLayoutPanel1.Controls.Add(card);
                                }
                            }
                        }
                    }
                }
            }

            flowLayoutPanel1.AutoScroll = true;
        }
        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            YedekYukle();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            YeniYazar yazar = new YeniYazar(this);
            yazar.Show();
        }
    }
}
