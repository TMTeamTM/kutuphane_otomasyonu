using System;
using System.Drawing;
using System.Windows.Forms;
namespace KutuphaneSistemi
{
    public partial class Loading : Form
    {
        private Form7 form7;
        private UyelerUpdate uyelerupdate;
        public Loading()
        {
            InitializeComponent();
        }
        Random rastgele = new Random();
        private void Loading_Load(object sender, EventArgs e)
        {
            bunifuCircleProgress1.Value = 25;
            loadingtimer.Start();
        }
        private void loadingtimer_Tick(object sender, EventArgs e)
        {
            int x = rastgele.Next(255);
            int y = rastgele.Next(255);
            int z = rastgele.Next(255);
            int sayi = rastgele.Next(5, 20);
            if (bunifuCircleProgress1.Value >= 80)
            {
                this.Hide();
                loadingtimer.Stop();
                Menu form5 = new Menu(form7, uyelerupdate);
                form5.Show();
            }
            else
            {
                bunifuCircleProgress1.Value += sayi;
                bunifuCircleProgress1.ProgressColor = Color.FromArgb(x, y, z);
                if (bunifuCircleProgress1.Value >= 80)
                {
                    bunifuCircleProgress1.Value = 100;
                }
                bunifuLabel1.Text += ".";
                if (bunifuLabel1.Text.Length >= 14)
                {
                    bunifuLabel1.Text = "YÜKLENİYOR";
                }
            }
        }
    }
}
