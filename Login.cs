using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace MineSweeper
{
    public partial class Login : Form
    {
        public Login(ref IntranetForGame intranetCore)
        {
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            intranetCore = this.intranetCore;
            InitializeComponent();
            this.intranetCore.LocalHost = new IPEndPoint(IPAddress.Parse(IntranetForGame.GetLocalIp()), 8848);
            label1.Text = "本地IP地址 : " + IntranetForGame.GetLocalIp();
            this.intranetCore.Init();
            ReceiveMsgAsync();
        }
        private IntranetForGame intranetCore = new IntranetForGame();
        private void button1_Click(object sender, EventArgs e)
        {
            Regex r = new Regex(@"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))");
            if(!r.IsMatch(textBox1.Text))
            {
                MessageBox.Show(text: "\"" + textBox1.Text + "\" is not a valid IPV4 address.", caption: "Error", MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }
            intranetCore.RemoteHost = new IPEndPoint(IPAddress.Parse(textBox1.Text), 8848);
            intranetCore.Connect();
        }

        /// <summary>
        /// 异步地监听UDP消息。
        /// </summary>
        private async void ReceiveMsgAsync()
        {
            await Task.Run(() => { intranetCore.ExchangeMsg(null, null); });
            Close();
        }
    }
}
