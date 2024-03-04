using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    public partial class OduncUye : Form
    {
        private MySqlConnection connection;
        private string connectionString = "Server=localhost;Database=kütüphane sistemi;Uid=root;Pwd='';";
        private DataTable dataTable;
        private MySqlDataAdapter mySqlDataAdapter;
        private OduncVer form6;
        Menu menu;
        public OduncUye(OduncVer form6Reference)
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            form6 = form6Reference;
            dataTable = new DataTable();
            mySqlDataAdapter = new MySqlDataAdapter("SELECT uyeler.ID, uyeler.Ad, uyeler.Soyad, statu.statu_adi,uyeler.DogumT FROM uyeler JOIN statu ON uyeler.statu = statu.ID", connection);
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

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {
            YeniUye form = new YeniUye(menu);
            form.Show();
            this.Hide();
        }
    }
}
