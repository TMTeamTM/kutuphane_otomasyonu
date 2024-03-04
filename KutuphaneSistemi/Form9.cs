using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class Form9 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private DataTable dataTable;
        private MySqlDataAdapter mySqlDataAdapter;
        private Form6 form6;

        public Form9(Form6 form6Reference)
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            form6 = form6Reference;
            dataTable = new DataTable();
            mySqlDataAdapter = new MySqlDataAdapter("SELECT uyeler.ID, uyeler.Ad, uyeler.Soyad, statu.statu_adi,uyeler.DogumT FROM uyeler JOIN statu ON uyeler.statu = statu.ID", connection);
            mySqlDataAdapter.Fill(dataTable);
            bunifuDataGridView1.DataSource = dataTable;
            bunifuDataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void InitializeDatabaseConnection()
        {
            connection = new MySqlConnection(connectionString);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = bunifuDataGridView1.Rows[e.RowIndex];
                string selecteduserInfo = $"{row.Cells["Ad"].Value} " + $"{row.Cells["Soyad"].Value}";
                string ID = $"{row.Cells["ID"].Value} ";
                form6.label8.Text = ID;
                form6.label3.Text = selecteduserInfo;
                this.Close();
            }
        }
        public void GuncelleDataGrid(string searchText)
        {
            string filter = bunifuTextBox1.Text;
            DataView dv = dataTable.DefaultView;
            dv.RowFilter = $"Ad LIKE '%{filter}%'";
            bunifuDataGridView1.DataSource = dv.ToTable();
        }

        private void Form9_Load(object sender, EventArgs e)
        {

        }

        private void bunifuTextBox1_TextChange_1(object sender, EventArgs e)
        {
            if (bunifuTextBox1.Text.Length >= 1)
            {
                GuncelleDataGrid(bunifuTextBox1.Text);
                bunifuLabel3.Visible = true;
            }
        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {
            Form4 form = new Form4();
            form.Show();
            this.Hide();
        }
    }
}
