using System;
using System.Windows.Forms;

namespace KutuphaneSistemi
{
    internal static class Program
    {
        static readonly Form7 form7;
        static readonly Menu menu;
        static readonly UyelerUpdate uyelerupdate;
        static readonly YazarUpdate yazarupdate;
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu(form7, uyelerupdate));
            //Application.Run(new Loading());
        }
    }
}
