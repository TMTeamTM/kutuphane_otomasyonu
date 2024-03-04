using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
namespace KutuphaneSistemi
{

    public partial class OduncVer : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        Menu menu;
        public OduncVer(Menu menureferences)
        {
            menu = menureferences;
            InitializeComponent();
            InitializeDatabaseConnection();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }
        private void Form6_Load(object sender, EventArgs e)
        {
            bunifuDatePicker1.ValueChanged += bunifuDatePicker1_ValueChanged;
            bunifuDatePicker2.ValueChanged += bunifuDatePicker2_ValueChanged;
        }
        private void Form6_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                OduncKitap form = new OduncKitap(this);
                form.Show();

            }
            if (e.KeyCode == Keys.F3)
            {
                OduncUye form = new OduncUye(this);
                form.Show();

            }
        }
        private void bunifuDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = bunifuDatePicker1.Value;
            DateTime nextDate = selectedDate.AddDays(30);

            if (selectedDate.DayOfWeek == DayOfWeek.Saturday || selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Cumartesi veya Pazar günü seçilemez!");
                bunifuDatePicker1.Value = DateTime.Now;
            }
            else
            {
                while (nextDate.DayOfWeek == DayOfWeek.Saturday || nextDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    nextDate = nextDate.AddDays(1);
                }

                bunifuDatePicker2.Value = nextDate;
            }


        }

        private void bunifuDatePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            try
            {

                int kitapID = Convert.ToInt32(label7.Text);
                int uyeID = Convert.ToInt32(label8.Text);
                DateTime verilenTarih = bunifuDatePicker1.Value;
                DateTime alinacakTarih = bunifuDatePicker2.Value;
                if (string.IsNullOrEmpty(kitapID.ToString()) && string.IsNullOrEmpty(uyeID.ToString()) && string.IsNullOrEmpty(verilenTarih.ToString()) && string.IsNullOrEmpty(alinacakTarih.ToString()))
                {
                    MessageBox.Show("İstenilen bilgileri doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (kitapID <= 0 || uyeID <= 0)
                    {
                        MessageBox.Show("Kitap ID ve Üye ID geçerli değil!");
                        return;
                    }

                    if (verilenTarih >= alinacakTarih)
                    {
                        MessageBox.Show("Verilen tarih, alınacak tarihten önce olmalıdır!");
                        return;
                    }

                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            string checkQuery = "SELECT COUNT(*) FROM odunc_kitaplar WHERE Kitap_ID = @KitapID AND Alinan_Tarih IS NULL";
                            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@KitapID", kitapID);
                                checkCmd.Parameters.AddWithValue("@UyeID", uyeID);

                                int rowCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (rowCount > 0)
                                {
                                    MessageBox.Show("Bu kitap zaten ödünç alınmış!");
                                    return;
                                }
                            }
                            string insertQuery = "INSERT INTO odunc_kitaplar (Kitap_ID, Uye_ID, Verilen_Tarih, Alinacak_Tarih) VALUES (@KitapID, @UyeID, @VerilenTarih, @AlinacakTarih)";
                            using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@KitapID", kitapID);
                                cmd.Parameters.AddWithValue("@UyeID", uyeID);
                                cmd.Parameters.AddWithValue("@VerilenTarih", verilenTarih);
                                cmd.Parameters.AddWithValue("@AlinacakTarih", alinacakTarih);

                                int affectedRows = cmd.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    MessageBox.Show("Ödünç kitap başarıyla eklendi!");
                                    label2.Text = "Bilinmiyor[F2]"; label3.Text = "Bilinmiyor[F3]";
                                    bunifuDatePicker1.Value = DateTime.Now;
                                    bunifuDatePicker2.Value = DateTime.Now;
                                    menu.bunifuPanel1.Visible = true;
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Ödünç kitap eklenirken bir hata oluştu!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Bir hata oluştu: " + ex.Message);
                    }
                }
            }
            catch
            {
                MessageBox.Show("İstenilen bilgileri doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
