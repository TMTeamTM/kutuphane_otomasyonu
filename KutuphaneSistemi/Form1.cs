using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace KutuphaneSistemi
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private Timer timer;
        private int kalanSaniye , afkkontrol = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            Timer();
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
                catch
                {
          
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Arama();
            kalanSaniye = 0;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Silme();
            DataYenile();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DataGöster();
            timer.Start();
            WampCalistirEgerCalismiyorsa();
            
        }
       private void Arama()
        {
            string spot = textBox4.Text;
            string yazar = textBox3.Text;
            string isbn = textBox2.Text;
            string kitapAdi = textBox1.Text;
            if (string.IsNullOrEmpty(spot) && string.IsNullOrEmpty(yazar) && string.IsNullOrEmpty(isbn) && string.IsNullOrEmpty(kitapAdi))
            {
                MessageBox.Show("Lütfen en az bir alanı doldurun.");
                return;
            }
            string query = "SELECT * FROM kitap WHERE 1=1";
            if (!string.IsNullOrEmpty(spot))
            {
                query += " AND spot LIKE @spot";
            }
            if (!string.IsNullOrEmpty(yazar))
            {
                query += " AND Author LIKE @yazar";
            }
            if (!string.IsNullOrEmpty(isbn))
            {
                query += " AND ISBN LIKE @isbn";
            }
            if (!string.IsNullOrEmpty(kitapAdi))
            {
                query += " AND Name LIKE @kitapAdi";
            }
            MySqlCommand cmd = new MySqlCommand(query, connection);
            if (!string.IsNullOrEmpty(spot))
            {
                cmd.Parameters.AddWithValue("@spot", "%" + spot + "%");
            }
            if (!string.IsNullOrEmpty(yazar))
            {
                cmd.Parameters.AddWithValue("@yazar", "%" + yazar + "%");
            }
            if (!string.IsNullOrEmpty(isbn))
            {
                cmd.Parameters.AddWithValue("@isbn", "%" + isbn + "%");
            }
            if (!string.IsNullOrEmpty(kitapAdi))
            {
                cmd.Parameters.AddWithValue("@kitapAdi", "%" + kitapAdi + "%");
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
                int ProgressBar1 = toolStripProgressBar1.Value;
                if(ProgressBar1 != 100) {
                toolStripProgressBar1.Increment(12);
                Progress();
                }
            }
        }
        private void Progress()
        {
            int ProgressBar1 = toolStripProgressBar1.Value;
            if (ProgressBar1 == 100)
            {
                MessageBox.Show("ProgressBar'ı doldurdun baş kazandın :D", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Silme()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int seciliID = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string query = "DELETE FROM kitap WHERE ID = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", seciliID);
                    try
                    {
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Veri başarıyla silindi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Silme işlemi başarısız oldu.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else
            {
                MessageBox.Show("Lütfen silinecek bir satır seçin. Verinin solu :D", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            Form3 Form3 = new Form3();
            Form3.Show();
            this.Hide();
        }
        private void DataYenile()
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
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form1 Form = new Form1();
            Form.Show();
            this.Hide();
        }
        private void Timer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1 saniye
            timer.Tick += timer1_Tick;
            kalanSaniye = 0;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(afkkontrol == 1) { 
            kalanSaniye++;
         
            if (kalanSaniye >= 31)
            {
                    timer.Stop();
                MessageBox.Show("AFK kaldın, atıldın!");

                Close();
            }
            }
        }
        private void Form1_Click(object sender, EventArgs e)
        {
            kalanSaniye = 0;
        }

        private void afkKontrolüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(afkkontrol == 1)
            {
                afkkontrol = 0;
                afkKontrolüToolStripMenuItem.Text = "Afk Kontrolü (Kapalı)";
                kalanSaniye = 0;
            }
           else if (afkkontrol == 0)
            {
                afkkontrol = 1;
                afkKontrolüToolStripMenuItem.Text = "Afk Kontrolü (Açık)";
                kalanSaniye = 0;
            }
        }
        private void WampCalistirEgerCalismiyorsa()
        {
            if (!IsWampServerRunning())
            {
                WampCalistir();
            }
        }
        private bool IsWampServerRunning()
        {
            Process[] processes = Process.GetProcessesByName("wampmanager");
            return processes.Length > 0;
        }
        private void button5_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip Aciklama = new System.Windows.Forms.ToolTip();
            Aciklama.ToolTipTitle = "Dikkat!";
            Aciklama.ToolTipIcon = ToolTipIcon.Warning;
            Aciklama.IsBalloon = true;
            Aciklama.SetToolTip(button5, "Verileri Siler");
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip Aciklama = new System.Windows.Forms.ToolTip();
            Aciklama.ToolTipTitle = "Buton Bilgi!";
            Aciklama.ToolTipIcon = ToolTipIcon.Warning;
            Aciklama.IsBalloon = true;
            Aciklama.SetToolTip(button6, "Verileri Güncellemek İçindir");
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip Aciklama = new System.Windows.Forms.ToolTip();
            Aciklama.ToolTipTitle = "Buton Bilgi!";
            Aciklama.ToolTipIcon = ToolTipIcon.Warning;
            Aciklama.IsBalloon = true;
            Aciklama.SetToolTip(button7, "Kayıt formuna gider");
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip Aciklama = new System.Windows.Forms.ToolTip();
            Aciklama.ToolTipTitle = "Buton Bilgi!";
            Aciklama.ToolTipIcon = ToolTipIcon.Warning;
            Aciklama.IsBalloon = true;
            Aciklama.SetToolTip(button1, "Veritabanında arama yaparsınız, herhangi bir boşluğu doldurmayı unutmayın!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form4 Form4 = new Form4();
            Form4.Show();
            this.Hide();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            Form1.Show();
            this.Hide();
        }

        private void WampCalistir()
        {
            try
            {
                string wampServerPath = @"""C:\wamp64\wampmanager.exe""";
                Process.Start(wampServerPath);
                MessageBox.Show("Wampserveri başlattım, program sorunsuz aktif olduğunda sol üstten verileri yenile!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("WampServer başlatılırken bir hata oluştu: " + ex.Message);
            }
        }
    }
}