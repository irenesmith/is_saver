using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ISaver
{
    public partial class MainForm : Form
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
        private const int NUM_SAVERS = 3;
        private bool previewMode = false;
        private List<GraphicsInfo> Contexts;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(IntPtr PreviewWndHandle)
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

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!_MouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(_MouseLocation.X - e.X) > 5 ||
                        Math.Abs(_MouseLocation.Y - e.Y) > 5)
                    {
                        Program.Done = true;
                        Application.Exit();
                    }
                }
                // Save the mouse location for use the next time the
                // even is raised.
                _MouseLocation = e.Location;
            }

        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                Program.Done = true;
                Application.Exit();
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
            {
                Program.Done = true;
                Application.Exit();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Prepare the list of graphic contexts, one for
            // each screen that is attached to the computer.
            Contexts = new List<GraphicsInfo>();
            Contexts.Add(new GraphicsInfo(this.CreateGraphics(), this.Width, this.Height));
            if(!previewMode)
            {
                // We only need the extra contexts if we are not drawing
                // on the preview screen in the Windows dialog box.
                for (int i = 1; i < Screen.AllScreens.Length; i++)
                {
                    MultiMonitorForm newForm = new MultiMonitorForm();
                    newForm.WindowState = FormWindowState.Maximized;
                    newForm.Bounds = Screen.AllScreens[i].Bounds;
                    newForm.Show();
                    Contexts.Add(new GraphicsInfo(newForm.CreateGraphics(), newForm.Width, newForm.Height));
                }
            }

            Cursor.Hide();
            this.TopMost = true;

            int whichSaver = 3;

            switch (whichSaver)
            {
                case 1:
                    DrawLines();
                    break;
                case 2:
                    DrawDots();
                    break;
                case 3:
                    DoSpirograph();
                    break;
                default:
                    DoSpirograph();
                    break;
            }
        }

        private void DrawLines()
        {
            Pen p = new Pen(Brushes.White, 1);
            Random r = new Random();
            Point pt1 = new Point(5, 5);
            Point pt2 = new Point(50, 50);
            int numLines = 0;
            int maxLines = r.Next(100, 750);
            do
            {
                foreach (GraphicsInfo g in Contexts)
                {
                    p.Width = r.Next(1, 5);
                    int red = r.Next(0, 255);
                    int green = r.Next(0, 255);
                    int blue = r.Next(0, 255);
                    p.Color = Color.FromArgb(red, green, blue);

                    pt1.X = r.Next(0, g.Width - 1);
                    pt1.Y = r.Next(0, g.Height - 1);
                    pt2.X = r.Next(0, g.Width - 1);
                    pt2.Y = r.Next(0, g.Height - 1);
                    if (!Program.Done)
                    {
                        g.Context.DrawLine(p, pt1, pt2);
                        Application.DoEvents();
                        numLines++;
                        if (numLines > maxLines)
                        {
                            numLines = 0;
                            maxLines = r.Next(100, 5000);
                            g.Context.Clear(Color.Black);
                        }
                    }
                }

            } while (!Program.Done);
        }

        private void DrawDots()
        {
            Pen p = new Pen(Brushes.White, 1);
            Random r = new Random();
            Point pt1 = new Point(5, 5);
            Point pt2 = new Point(50, 50);
            Rectangle rect = new Rectangle();

            int numLines = 0;
            int maxLines = r.Next(100, 750);
            do
            {
                foreach(GraphicsInfo g in Contexts)
                {
                    int red = r.Next(0, 255);
                    int green = r.Next(0, 255);
                    int blue = r.Next(0, 255);
                    SolidBrush b = new SolidBrush(Color.FromArgb(red, green, blue));
                    int x = r.Next(0, g.Width);
                    int y = r.Next(0, g.Height);
                    int diameter = r.Next(5, 25);
                    rect = new Rectangle(x, y, diameter, diameter);

                    if (!Program.Done)
                        g.Context.FillEllipse(b, rect);
                    b.Dispose();

                    Application.DoEvents();
                    numLines++;
                    if (numLines > maxLines)
                    {
                        numLines = 0;
                        maxLines = r.Next(15000, 50000);
                        if(!Program.Done)
                            g.Context.Clear(Color.Black);
                    }
                }
            } while (!Program.Done);
        }

        /// <summary>
        /// This routine is based on Delphi Code written
        /// as a graphic programming example by Jeff Duntemann
        /// for the book "Delphi Programming Explorer"
        /// </summary>
        private void DoSpirograph()
        {
            // The random number generator
            // is for picking colors and setting
            // the number of reps before the
            // program will clear the screen and 
            // start a fresh set of shapes.
            Random r = new Random();

            // For line colors.
            int red, green, blue;

            // A pen for drawing the lines.
            Pen p = new Pen(Color.White, 2);

            // Iterations and IterCount are used
            // to randomly determine how many reps
            // will be performed before clearing
            // the screen and starting fresh.
            int iterations, iterCount = 0;

            // Variables for drawing the shapes.
            int a = 200;

            // These two values will be set
            // randomly at the top of the iteration.
            int b, d;

            int rab, lines;
            float alpha, beta, aDif, aOverB, x, y;
            float startX, startY;
            int centerX, centerY;

            do
            {
                // Determine how many iterations before
                // the next screen clear.
                iterations = r.Next(50, 150);
                for (iterCount = 0; iterCount < iterations; iterCount++)
                {
                    // set values for next iteration
                    b = r.Next(5, 75); // 70
                    d = r.Next(25, 250); // 65
                    red = r.Next(0, 255);
                    green = r.Next(0, 255);
                    blue = r.Next(0, 255);
                    p.Color = Color.FromArgb(red, green, blue);

                    rab = a - b;
                    alpha = 0.0f;
                    aDif = (float)(Math.PI / 100.0f);
                    aOverB = a / b;
                    lines = 200 * (b / HighestCommonFactor(a, b));

                    foreach (GraphicsInfo g in Contexts)
                    {
                        if (Program.Done)
                            break;
                        // Center of the screen.
                        centerX = g.Width / 2;
                        centerY = g.Height / 2;

                        // Starting point where the first
                        // line originates.
                        startX = rab + d + centerX;
                        startY = centerY;

                        // Here is where we do the drawing.
                        for (int i = 1; i <= lines; i++)
                        {
                            alpha += aDif;
                            beta = alpha * aOverB;
                            x = (float)(rab * Math.Cos((double)alpha) + d * Math.Cos((double)beta));
                            y = (float)(rab * Math.Sin((double)alpha) - d * Math.Sin((double)beta));

                            if (!Program.Done)
                                g.Context.DrawLine(p, startX, startY, x + centerX, y + centerY);
                            startX = x + centerX;
                            startY = y + centerY;

                            Application.DoEvents();
                        }

                    }
                }
                // Have to do the loop to clear all of the
                // screens at the end of each iteration. If
                // I put this inside the foreach loop above,
                // the screen would be cleared after each
                // iteration.
                foreach (GraphicsInfo g in Contexts)
                    if (!Program.Done)
                        g.Context.Clear(Color.Black);

            } while (!Program.Done);
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
