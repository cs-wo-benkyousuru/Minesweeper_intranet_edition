using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication;

namespace MineSweeper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                intranetCore = new IntranetForGame();
                Application.Run(new Login(ref intranetCore));
                Application.Run(new Form1(ref intranetCore));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, caption: "Exception", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }
        private static IntranetForGame intranetCore;
    }
}
