using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileDateChanger.Services
{
    class MsgBox : Form
    {
        public enum ThemeMode : byte
        {
            Dark,
            Light
        }

        private const int MaxHeight = 60;
        private const int MinWidth = 200;
        private const int MaxWidth = 500;

        private readonly Timer timer;
        private readonly StringFormat stringFormat;
        private readonly SolidBrush fontBrush;
        private ThemeMode currentTheme;
        private Font font;
        private string text;
        private int duration;

        public ThemeMode Theme
        {
            get
            {
                return currentTheme;
            }
            set
            {
                currentTheme = value;
                if (currentTheme == ThemeMode.Dark)
                {
                    BackColor = Color.FromArgb(30, 30, 30);
                    fontBrush.Color = Color.White;
                }
                else
                {
                    BackColor = Color.White;
                    fontBrush.Color = Color.Black;
                }
                Invalidate();
            }
        }

        public new Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                Invalidate();
            }
        }

        public new string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                Invalidate();
            }
        }

        public int Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value > 0 ? value : 0;
            }
        }

        public MsgBox() : this("") { }

        public MsgBox(string text) : this(text, 80) { }

        public MsgBox(string text, int duration, ThemeMode theme) : this(text, duration)
        {
            Theme = theme;
        }

        public MsgBox(string text, int duration)
        {
            font = new Font
                (
                base.Font.FontFamily,
                12,
                base.Font.Style,
                base.Font.Unit,
                base.Font.GdiCharSet,
                base.Font.GdiVerticalFont
                );
            stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            fontBrush = new SolidBrush(Color.White);
            BackColor = Color.FromArgb(30, 30, 30);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Opacity = 0;
            ShowIcon = false;
            ShowInTaskbar = false;
            Duration = duration;
            Height = MaxHeight;
            this.text = text;
            timer = new Timer();
            timer.Tick += new EventHandler(Tick);
            timer.Interval = 1;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Tick(object sender, EventArgs e)
        {
            if (duration > -1 && Opacity < 1)
            {
                Opacity += 0.1;
            }
            else
            {
                if (duration > -1)
                {
                    duration--;
                }
                else if (Opacity > 0)
                {
                    Opacity -= 0.01;
                }
                else
                {
                    Dispose();
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Dispose();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Owner.Width > MaxWidth)
            {
                Width = MaxWidth;
            }
            else if (Owner.Width < MinWidth)
            {
                Width = MinWidth;
            }
            else
            {
                Width = Owner.Width - 12;
            }
            Location = new Point(Owner.Location.X + (Owner.Width - Width) / 2, Owner.Location.Y + Owner.Height - Height - 6);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = new Rectangle(new Point(), Size);
            e.Graphics.DrawString(text, font, fontBrush, rect, stringFormat);
            timer.Start();
        }

        public new void Show()
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Owner is Null.\nUse Show(this)");
            }
        }

        public new void ShowDialog()
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Owner is Null.\nUse Show(this)");
            }
        }

        public new void Dispose()
        {
            stringFormat.Dispose();
            fontBrush.Dispose();
            timer.Dispose();
            font.Dispose();
            base.Dispose();
        }
    }
}