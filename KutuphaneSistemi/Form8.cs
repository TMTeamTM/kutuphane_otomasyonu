using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class Form8 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private DataTable dataTable;
        private MySqlDataAdapter mySqlDataAdapter;
        private Form6 form6;

        public Form8(Form6 form6Reference)
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            form6 = form6Reference;
            dataTable = new DataTable();
            mySqlDataAdapter = new MySqlDataAdapter("SELECT ID, Name, Author, ISBN FROM kitap", connection);
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
                string selectedBookInfo = $"{row.Cells["Name"].Value}";
                string ID = $"{row.Cells["ID"].Value}";
                form6.label2.Text = selectedBookInfo;
                form6.label7.Text = ID;
                this.Close();
            }
        }

        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            if (bunifuTextBox1.Text.Length >= 1)
            {
                GuncelleDataGrid(bunifuTextBox1.Text);
                bunifuLabel3.Visible = true;
            }
        }

        public void GuncelleDataGrid(string searchText)
        {
            string filter = bunifuTextBox1.Text;
            DataView dv = dataTable.DefaultView;
            dv.RowFilter = $"Name LIKE '%{filter}%'";
            bunifuDataGridView1.DataSource = dv.ToTable();
        }

        private void Form8_Load(object sender, EventArgs e)
        {

        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {
            Form4 form = new Form4();
            form.Show();
            this.Hide();
        }
    }
}
