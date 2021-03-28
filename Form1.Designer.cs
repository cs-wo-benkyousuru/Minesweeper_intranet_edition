using System;
using System.Windows.Forms;

namespace MineSweeper
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 550);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Minesweeper";
            this.Text = "Minesweeper";
            this.ResumeLayout(false);
            timer.Tick += Timer_Tick;
            FormClosing += Form1_FormClosing;
            timer.Interval = 1000;

            for (int i = 0; i < 100; i++)
            {
                buttons[i] = new _Button();
                buttons[i].TabIndex = i;
                buttons[i].Width = 50;
                buttons[i].Height = 50;
                buttons[i].Text = " ";
                buttons[i].Left = Left + 25;
                buttons[i].Top = Top + 25;
                buttons[i].MouseDown += new MouseEventHandler(ButtonSingleClick);
                this.Controls.Add(buttons[i]);
                Left += 50;
                if (Left >= 500)
                {
                    Left = 0;
                    Top += 50;
                }
            }
            timer.Enabled = true;
        }
        #endregion
        _Button[] buttons = new _Button[100];
        Timer timer = new Timer();

        class _Button : Button
        {
            public bool IsClicked { get; set; } = false;
            public bool ContainsMine { get; set; } = false;
            public int AroundNumber { get; set; } = 0;
            public Enumbuttonstatus Status { get; set; } = Enumbuttonstatus.Unopened;
            //0--未翻开
            //1--已翻开
            //2--红旗标记
            //3--问号标记
        }
    }
}

