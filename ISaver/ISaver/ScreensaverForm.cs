using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ISaver
{
    public partial class ScreensaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private Point _MouseLocation;
        private bool _Done = false;
        private const int NUM_SAVERS = 5;
        private bool previewMode = false;

        public ScreensaverForm()
        {
            InitializeComponent();
        }

        public ScreensaverForm(Rectangle ScreenBounds)
        {
            InitializeComponent();
            this.Bounds = ScreenBounds;
        }

        public ScreensaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();
 
            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);
 
            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));
 
            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);
 
            previewMode = true;
        }

        private void ScreensaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseLocation.IsEmpty)
            {
                // Terminate if mouse is moved a significant distance
                if (Math.Abs(_MouseLocation.X - e.X) > 5 ||
                    Math.Abs(_MouseLocation.Y - e.Y) > 5)
                {
                    if (!previewMode)
                    {
                        _Done = true;
                        Application.Exit();
                    }
                }
            }

            // Save the mouse location for use the next time the
            // even is raised.
            _MouseLocation = e.Location;
        }

        private void ScreensaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                _Done = true;
                Application.Exit();
            }
        }

        private void ScreensaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
            {
                _Done = true;
                Application.Exit();
            }
        }

        private void ScreensaverForm_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            this.TopMost = true;
            Random r = new Random();
            int whichSaver = r.Next(1, NUM_SAVERS);
            Console.WriteLine(whichSaver);

            DoSpirograph();

            //switch(whichSaver)
            //{
            //    case 1:
            //        DrawLines();
            //        break;
            //    case 2:
            //        DrawDots();
            //        break;
            //    default:
            //        DoSpirograph();
            //        break;
            //}
        }

        private void DrawLines()
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Brushes.White, 1);
            Random r = new Random();
            Point pt1 = new Point(5, 5);
            Point pt2 = new Point(50, 50);
            int numLines = 0;
            int maxLines = r.Next(100, 750);
            do
            {
                p.Width = r.Next(1,5);
                int red = r.Next(0,255);
                int green = r.Next(0,255);
                int blue = r.Next(0,255);
                p.Color = Color.FromArgb(red, green, blue);

                pt1.X = r.Next(0, this.Width - 1);
                pt1.Y = r.Next(0, this.Height - 1);
                pt2.X = r.Next(0, this.Width - 1);
                pt2.Y = r.Next(0, this.Height - 1);
                g.DrawLine(p, pt1, pt2);
                Application.DoEvents();
                numLines++;
                if (numLines > maxLines)
                {
                    numLines = 0;
                    maxLines = r.Next(100, 5000);
                    g.Clear(Color.Black);
                }

            } while (!_Done);
        }

        private void DrawDots()
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Brushes.White, 1);
            Random r = new Random();
            Point pt1 = new Point(5, 5);
            Point pt2 = new Point(50, 50);
            Rectangle rect = new Rectangle();

            int numLines = 0;
            int maxLines = r.Next(100, 750);
            do
            {
                int red = r.Next(0, 255);
                int green = r.Next(0, 255);
                int blue = r.Next(0, 255);
                SolidBrush b = new SolidBrush(Color.FromArgb(red, green, blue));
                int x = r.Next(0, this.Width);
                int y = r.Next(0, this.Height);
                int diameter = r.Next(5, 25);
                rect = new Rectangle(x, y, diameter, diameter);

                g.FillEllipse(b, rect);
                b.Dispose();

                Application.DoEvents();
                numLines++;
                if (numLines > maxLines)
                {
                    numLines = 0;
                    maxLines = r.Next(15000, 50000);
                    g.Clear(Color.Black);
                }

            } while (!_Done);
        }

        private void DoSpirograph()
        {
            // The random number generator
            // is for picking colors and setting
            // the number of reps before the
            // program will clear the screen and 
            // start a fresh set of shapes.
            Random r = new Random();

            // Center of the screen.
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            // For line colors.
            int red = r.Next(0, 255);
            int green = r.Next(0, 255);
            int blue = r.Next(0, 255);

            int iterations = r.Next(50, 100);
            int iterCount = 0;

            // A pen for drawing the lines.
            Pen p = new Pen(Color.FromArgb(red, green, blue), 1);

            Graphics g = this.CreateGraphics();

            // Variables for drawing the shapes.
            int a = 200; // 200
            int b = r.Next(5, 75); // 70
            int d = r.Next(25, 250); // 65
            int i;

            int rab, lines;
            float alpha, beta, aDif, aOverB, x, y;
            float startX, startY;

            do
            {
                for (iterCount = 0; iterCount < iterations; iterCount++)
                {
                    if (_Done)
                        break;
                    rab = a - b;
                    alpha = 0.0f;
                    aDif = (float)(Math.PI / 100.0f);
                    aOverB = a / b;
                    lines = 200 * (b / HighestCommonFactor(a, b));
                    startX = rab + d + centerX;
                    startY = centerY;
                    for (i = 1; i <= lines; i++)
                    {
                        alpha += aDif;
                        beta = alpha * aOverB;
                        x = (float)(rab * Math.Cos((double)alpha) + d * Math.Cos((double)beta));
                        y = (float)(rab * Math.Sin((double)alpha) - d * Math.Sin((double)beta));
                        if(!_Done)
                            g.DrawLine(p, startX, startY, x + centerX, y + centerY);
                        startX = x + centerX;
                        startY = y + centerY;
                        Application.DoEvents();
                    }

                    // set values for next iteration
                    d = r.Next(25, 250); // 65
                    red = r.Next(0, 255);
                    green = r.Next(0, 255);
                    blue = r.Next(0, 255);
                    p.Color = Color.FromArgb(red, green, blue);
                }
                // Determine how many iterations before
                // the next screen clear.
                if(!_Done)
                    g.Clear(Color.Black);
                iterations = r.Next(50, 150);
                b = r.Next(5, 75);
            } while (!_Done);
        }

        private int HighestCommonFactor(int a, int b)
        {
            int i;
            int j;
            int hcf;

            if (a < b)
            {
                hcf = 1;
                i = b;
            }
            else
            {
                hcf = b;
                i = a;
            }

            do
            {
                j = i % hcf;
                if (j != 0)
                {
                    i = hcf;
                    hcf = j;
                }
            } while(j != 0);
            return hcf;
        }
    }
}
