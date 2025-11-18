using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace thekaushikls
{
    public partial class Uniclock : Form
    {
        private const int W_RADIUS = 30;

        private readonly Timer timer;
        private string _lastDisplayedMinute = "";

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public Uniclock()
        {
            InitializeComponent();
            InitializeDisplay();

            // Initialize Timer
            timer = new Timer()
            {
                Interval = 2000 // ms
            };
            timer.Tick += OnTimerTick;
            timer.Start();

            this.MouseDown += OnMouseDown;
            SetRoundedRegion(W_RADIUS);
        }

        private void InitializeDisplay()
        {
            timeLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            dayLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular);

            timeLabel.MouseDown += OnMouseDown;
            dayLabel.MouseDown += OnMouseDown;
        }

        private void SetRoundedRegion(int radius)
        {
            if (radius > 0)
            {
                var path = new GraphicsPath();
                int w = this.Width;
                int h = this.Height;

                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(w - radius, 0, radius, radius, 270, 90);
                path.AddArc(w - radius, h - radius, radius, radius, 0, 90);
                path.AddArc(0, h - radius, radius, radius, 90, 90);
                path.CloseAllFigures();

                this.Region = new Region(path);
            }            
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            // Format: hours and minutes, 12-hour clock with AM/PM
            string now = DateTime.Now.ToString("hh:mm tt");
            string today = DateTime.Now.DayOfWeek.ToString().ToUpper();
            if (now != _lastDisplayedMinute) // Update only if the minute changed
            {
                timeLabel.Text = now;
                dayLabel.Text = today;
                _lastDisplayedMinute = now;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetRoundedRegion(W_RADIUS);
        }
    }
}
