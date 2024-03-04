using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class OduncKitap : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private DataTable dataTable;
        private MySqlDataAdapter mySqlDataAdapter;
        private OduncVer form6;
        Menu menu;

        public OduncKitap(OduncVer form6Reference)
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            form6 = form6Reference;
            dataTable = new DataTable();
            mySqlDataAdapter = new MySqlDataAdapter("SELECT ID,ISBN, Name,Description, Author, No_of_pages,spot FROM kitap", connection);
            mySqlDataAdapter.Fill(dataTable);
            bunifuDataGridView1.DataSource = dataTable;
            bunifuDataGridView1.CellClick += DataGridView1_CellClick;
            if (bunifuDataGridView1.Columns.Contains("ID"))
            {
                bunifuDataGridView1.Columns["ID"].Visible = false;
            }
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
            string aramaKelimesi = bunifuTextBox1.Text.ToLower();
            DataView dv = ((DataTable)bunifuDataGridView1.DataSource).DefaultView;
            StringBuilder filterExpression = new StringBuilder();
            foreach (DataColumn column in dv.Table.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    if (filterExpression.Length > 0)
                        filterExpression.Append(" OR ");
                    filterExpression.Append($"{column.ColumnName} LIKE '%{aramaKelimesi}%'");
                }
                else if (column.DataType == typeof(DateTime))
                {
                    if (filterExpression.Length > 0)
                        filterExpression.Append(" OR ");
                    filterExpression.Append($"CONVERT({column.ColumnName}, 'System.String') LIKE '%{aramaKelimesi}%'");
                }
            }
            dv.RowFilter = filterExpression.ToString();
        }
        private void Form8_Load(object sender, EventArgs e)
        {

        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {
            YeniUye form = new YeniUye(menu);
            form.Show();
            this.Hide();
        }
    }
}
