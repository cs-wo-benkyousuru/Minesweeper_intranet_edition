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

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        public Form1(ref IntranetForGame intranetForGame)
        {
            InitializeComponent();
            intranetCore = intranetForGame;

            ReceiveMsgAsync();
        }

        private IntranetForGame intranetCore;
        private async void ReceiveMsgAsync()
        {
            await Task.Run(() => { intranetCore.ExchangeMsg(DecreaseRivalUnopenedButtonsNum, ShowRivalOverMsg); });

            Close();
        }

        private int RivalUnopenedButtons = 100;
        private int UsedTime = 0;
        private void ShowComepeteInfo()
        {
            this.Text = "Used time : " + UsedTime.ToString() + "s, me" + UnopenedButtons.ToString() + " : rival" + RivalUnopenedButtons.ToString();
        }
        private void DecreaseRivalUnopenedButtonsNum(int x)
        {
            Invoke(new Action(() => 
            { 
                RivalUnopenedButtons = x;
                ShowComepeteInfo();
            }));
        }

        private void DecreaseUnopenedButtonsNum(int x)
        {
            UnopenedButtons = x;
            ShowComepeteInfo();
        }

        private void ShowRivalOverMsg(string str)
        {
            MessageBox.Show(str);
        }

        enum Enumbuttonstatus
        {
            Opened,      //被翻开
            Unopened,    //未翻开
            Marked,      //红旗标记
            Suspected    //问号标记
        }

        private bool Initialized = false;
        HashSet<int> MinesPosition = new HashSet<int>();
        private int UnopenedButtons = 100;
        Bitmap[] bitmaps = new Bitmap[]
        {
            
            new Bitmap(Resource1.suspected),
            new Bitmap(Resource1._1),
            new Bitmap(Resource1._2),
            new Bitmap(Resource1._3),
            new Bitmap(Resource1._4),
            new Bitmap(Resource1._5),
            new Bitmap(Resource1._6),
            new Bitmap(Resource1._7),
            new Bitmap(Resource1._8),
            new Bitmap(Resource1.boom),
            new Bitmap(Resource1.flag),
            new Bitmap(Resource1.empty),
        };

        private void ButtonDoubleClick(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            MessageBox.Show(button.TabIndex.ToString());
        }

        /// <summary>
        /// 鼠标左键点击按钮
        /// </summary>
        /// <param name="sender">消息的发送者</param>
        /// <param name="e">消息参数</param>
        private void MouseLeftButtonClick(object sender, MouseEventArgs e)
        {
            _Button CurrentButton = sender as _Button;
            if (CurrentButton.IsClicked) return;
            int Position = CurrentButton.TabIndex;
            if (!Initialized)
            {
                InitMinePanel(Position);
                Initialized = true;
            }
            //点击到地雷
            if (buttons[Position].ContainsMine)
            {
                ShowAllMines();
                intranetCore.SendLostMsg();
                MessageBox.Show("游戏结束！");
                Application.Exit();
            }
            else
            {
                //八个相邻的方向有地雷
                if (buttons[Position].AroundNumber != 0)
                {
                    buttons[Position].Image = bitmaps[buttons[Position].AroundNumber];
                    buttons[Position].IsClicked = true;
                    buttons[Position].Status = Enumbuttonstatus.Opened;
                    --UnopenedButtons;
                }
                //八个相邻的方向不含地雷
                else if (buttons[Position].AroundNumber == 0)
                {
                    buttons[Position].Enabled = false;
                    buttons[Position].IsClicked = true;
                    buttons[Position].Status = Enumbuttonstatus.Opened;
                    --UnopenedButtons;
                    BreadthFirstSearch(Position);
                }
            }
        }

        private void ShowAllMines()
        {
            foreach (var i in MinesPosition)
            {
                buttons[i].Image = bitmaps[9];
            }
        }

        /// <summary>
        /// 广度优先遍历，遍历所有相邻、周围雷数为0、未被翻开的方块
        /// </summary>
        /// <param name="Position">遍历的起点</param>
        private void BreadthFirstSearch(int Position)
        {
            Queue<int> AuxiliaryQueue = new Queue<int>();
            AuxiliaryQueue.Enqueue(Position);
            while (!(AuxiliaryQueue.Count == 0))
            {
                int QueueFront = AuxiliaryQueue.First();
                AuxiliaryQueue.Dequeue();
                foreach (var Around in GetRoundbuttons(QueueFront))
                {
                    if (!buttons[Around].IsClicked)
                    {
                        if (buttons[Around].AroundNumber == 0)
                        {
                            buttons[Around].Enabled = false;
                            buttons[Around].IsClicked = true;
                            buttons[Around].Status = Enumbuttonstatus.Opened;
                            --UnopenedButtons;
                            AuxiliaryQueue.Enqueue(Around);
                        }
                        else
                        {
                            buttons[Around].Image = bitmaps[buttons[Around].AroundNumber];
                            buttons[Around].IsClicked = true;
                            buttons[Around].Status = Enumbuttonstatus.Opened;
                            --UnopenedButtons;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标右键点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseRightButtonClick(object sender, MouseEventArgs e)
        {
            _Button button = sender as _Button;
            if (button.Status == Enumbuttonstatus.Unopened)
            {
                button.Status = Enumbuttonstatus.Marked;
                button.Image = bitmaps[10];
            }
            else if (button.Status == Enumbuttonstatus.Marked)
            {
                button.Status = Enumbuttonstatus.Suspected;
                button.Image = bitmaps[0];
            }
            else if (button.Status == Enumbuttonstatus.Suspected)
            {
                button.Status = Enumbuttonstatus.Unopened;
                button.Image = bitmaps[11];
            }
            else
            {

            }
        }

        /// <summary>
        /// 单击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSingleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseLeftButtonClick(sender, e);
                intranetCore.SendDecreasedNumber(UnopenedButtons);
                DecreaseUnopenedButtonsNum(UnopenedButtons);
            }
            if (e.Button == MouseButtons.Right) MouseRightButtonClick(sender, e);
            
            if (UnopenedButtons == MineNum)
            {
                intranetCore.SendWinMsg();
                ShowAllMines();
                MessageBox.Show("成功！");
                Application.Exit();
            }
        }
        private int MineNum = 10;
        /// <summary>
        /// 初始化雷区
        /// </summary>
        /// <param name="_Exception">初始点击的位置</param>
        private void InitMinePanel(int _Exception)
        {
            Random random = new Random();

            int i = 0;
            while(i < MineNum)
            {
                var x = random.Next(0, 100);
                var Around = GetRoundbuttons(x);
                if (!MinesPosition.Contains(x) && x != _Exception && !Around.Contains(_Exception)) 
                {
                    MinesPosition.Add(x);
                    buttons[x].ContainsMine = true;
                    foreach (var AroundBlock in GetRoundbuttons(x))
                    {
                        ++buttons[AroundBlock].AroundNumber;
                    }
                    ++i;
                }
            }
        }
        private IEnumerable<int> GetRoundbuttons(int i)
        {
            int x = i % 10;
            int y = i / 10;
            if (y >= 1) 
                yield return i - 10;
            if (y <= 8)
                yield return i + 10;
            if (x <= 8)
                yield return i + 1;
            if (x >= 1)
                yield return i - 1;
            if (x >= 1 && y >= 1)
                yield return i - 11;
            if (x >= 1 && y <= 8)
                yield return i + 9;
            if (x <= 8 && y <= 8)
                yield return i + 11;
            if (x <= 8 && y >= 1)
                yield return i - 9;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            intranetCore.SendQuitMsg();
            Environment.Exit(0);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Invoke(new Action(() => 
            {
                UsedTime++;
                ShowComepeteInfo();
            }
            ));
        }
    }
}
