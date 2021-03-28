using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Windows.Forms;

namespace Communication
{
    public class IntranetForGame
    {
        public static string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        private UdpClient udpclient;
        public IPEndPoint LocalHost { get; set; }
        public IPEndPoint RemoteHost { get; set; }
        public IntranetForGame()
        {

        }
        public void Init()
        {
            udpclient = new UdpClient(this.LocalHost);
        }
        private string Greeting = "Hello";
        private string Response = "Hi";
        private string Win = "I won it";
        private string Lost = "I lost it";
        private string Quit = "I quit it";
        private string Decrease = "Number has decrease to";
        //private string Error = "Oops, haven't initialized yet";
        private bool Initialized = false;
        public void Close()
        {
            udpclient.Close();
        }
        public void Connect()
        {
            SendMsg(Greeting);
        }
        private void SendMsg(string str)
        {
            byte[] SendBytes = Encoding.UTF8.GetBytes(str);
            udpclient.Send(SendBytes, SendBytes.Length, RemoteHost);
        }
        public void SendDecreasedNumber(int x)
        {
            if (!Initialized) return;
            if (x >= 90 && x <= 0) return;
            SendMsg(Decrease + x.ToString());
        }
        public void SendLostMsg()
        {
            if (!Initialized) return;
            SendMsg(Lost);
        }
        public void SendWinMsg()
        {
            if (!Initialized) return;
            SendMsg(Win);
        }
        public void SendQuitMsg()
        {
            if (!Initialized) return;
            SendMsg(Quit);
        }
        public delegate void ActionWithInt(int x);
        public delegate void ActionWithString(string Msg);

        /// <summary>
        /// 与远程主机交换游戏信息
        /// </summary>
        /// <param name="f1">接受一个整形参数而不返回值的委托，获取对方减少的区域数</param>
        /// <param name="f2">接受一个字符串参数而不返回值的委托，获取对方的最终状态</param>
        public void ExchangeMsg(ActionWithInt f1, ActionWithString f2)
        {
            bool ExitFlag = false;
            
            IPEndPoint tmp = new IPEndPoint(IPAddress.Any, 0);
            while(!ExitFlag)
            {
                byte[] RecvByte = udpclient.Receive(ref tmp);

                string RecvStr = Encoding.UTF8.GetString(RecvByte);
                //MessageBox.Show(RecvStr);
                switch (RecvStr)
                {
                    case "Hello":
                        RemoteHost = tmp;
                        SendMsg(Response);
                        Initialized = true;
                        ExitFlag = true;
                        break;
                    case "I quit it":
                        f2("Rival has been quitted");
                        ExitFlag = true;
                        break;
                    case "I won it":
                        f2("Rival has won");
                        ExitFlag = true;
                        break;
                    case "I lost it":
                        f2("Rival has lost");
                        ExitFlag = true;
                        break;
                    case "Hi":
                        ExitFlag = true;
                        Initialized = true;
                        break;
                    default:
                        string Sub = RecvStr.Substring(RecvStr.Length - 2, 2);
                        f1(int.Parse(Sub));
                        break;
                }
            }
        }
    }
}