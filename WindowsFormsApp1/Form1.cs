using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // 设置窗体的起始位置为手动
            this.StartPosition = FormStartPosition.Manual;

            // 获取屏幕的工作区域尺寸
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;

            // 设置窗体的位置
            this.Location = new Point(screen.Width - this.Width, 0);

            // 将label1放置在窗体的右上角
            label1.Location = new Point(this.ClientSize.Width - label1.Width - 80, 20);

            // 置顶
            this.TopMost = true;

            // 设置透明背景
            this.BackColor = Color.LimeGreen; // 可以选择任何不常用的颜色
            this.TransparencyKey = Color.LimeGreen;

            // 设置窗体无边框
            this.FormBorderStyle = FormBorderStyle.None;

            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer1.Interval = 1000;
            UpdateSettings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private TimeSpan countdownTime = TimeSpan.FromMinutes(20); // 例如30分钟倒计时

        private void timer1_Tick(object sender, EventArgs e)
        {
       

            countdownTime = countdownTime.Subtract(TimeSpan.FromSeconds(1));
            label1.Text = countdownTime.ToString(@"hh\:mm\:ss");

            // 检查是否还剩5分钟
            if (countdownTime.TotalMinutes == 5 && countdownTime.Seconds == 0)
            {
                MessageBox.Show("还剩5分钟！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // 检查是否还剩1分钟
            if (countdownTime.TotalMinutes == 1 && countdownTime.Seconds == 0)
            {
                MessageBox.Show("还剩1分钟！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // 检查倒计时是否结束
            if (countdownTime.TotalSeconds <= 0)
            {
                timer1.Stop();
                Logout();
            }

            if (countdownTime.TotalSeconds <= 0)
            {
                timer1.Stop();
                Logout();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Logout()
        {
            Debug.WriteLine("This is a debug message");
            System.Diagnostics.Process.Start("shutdown", "/l /f");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentLevel == 2 || (currentLevel == 4 && add5ClickCount < 2))
            {
                countdownTime = countdownTime.Add(TimeSpan.FromMinutes(addTime5Min));
                add5ClickCount++;
                if (currentLevel == 2 || add5ClickCount >= 2)
                {
                    button1.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            countdownTime = countdownTime.Add(TimeSpan.FromMinutes(addTime10Min)); // 增加10分钟
            button2.Enabled = false; // 禁用按钮
        }

        // 更改级别的方法（根据你的应用逻辑来调用）
        private void ChangeLevel(int newLevel)
        {
            currentLevel = newLevel;
            add5ClickCount = 0; // 重置点击计数
            SetButtonState();
        }

        // 根据级别设置按钮状态
        private void SetButtonState()
        {
            switch (currentLevel)
            {
                case 1:
                    button1.Visible = false;
                    button2.Visible = false;
                    break;
                case 2:
                    button1.Visible = true;
                    button1.Enabled = true;
                    button2.Visible = false;
                    break;
                case 3:
                    button1.Visible = false;
                    button2.Visible = true;
                    button2.Enabled = true;
                    break;
                case 4:
                    button1.Visible = true;
                    button1.Enabled = true;
                    button2.Visible = true;
                    button2.Enabled = true;
                    break;
            }
        }

        private int currentLevel = 1; // 当前级别
        private int add5ClickCount = 0; // 用于跟踪增加5分钟按钮的点击次数
        private int addTime5Min = 5;
        private int addTime10Min = 10;
        private int totalTime = 30;

        public async Task<Settings> GetSettingsAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("http://192.168.5.4:33502/api/settings");
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(content);
                return JsonConvert.DeserializeObject<Settings>(content);
            }
        }

        private async void UpdateSettings()
        {
            try
            {
                var settings = await GetSettingsAsync();
                // 更新倒计时总时间
                // 更新按钮增加的时间
                addTime5Min = settings.AddTime5Min;
                addTime10Min = settings.AddTime10Min;
                totalTime = settings.TotalTime;
                countdownTime = TimeSpan.FromMinutes(totalTime);
                var currentLevel = settings.CurrentLevel;
                Console.WriteLine($"{currentLevel}", $"{totalTime}");
                this.button1.Text = $"+{addTime5Min}分钟";
                this.button2.Text = $"+{addTime10Min}分钟";

                ChangeLevel(currentLevel);
                this.timer1.Start();
            }
            catch (Exception ex)
            {
                // 处理错误
                Console.WriteLine(ex.ToString());
            }
        }

    }

    public class Settings
    {
        public int CurrentLevel { get; set; }
        public int TotalTime { get; set; }
        public int AddTime5Min { get; set; }
        public int AddTime10Min { get; set; }
    }

}
