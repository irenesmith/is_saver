using System;
using System.Windows.Forms;

namespace ISaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            string arg1 = string.Empty;
            string arg2 = string.Empty;

            if (args.Length > 0)
            {
                arg1 = args[0].Trim().ToLower();
                if (arg1.Length > 2)
                {
                    // The argument will be longer than 2
                    // characters when it includes an hwnd
                    // value as well as a parameter.
                    arg2 = arg1.Substring(3).Trim();

                    // Just keep the first two characters as
                    // the rest have been assigned to arg2.
                    arg1 = arg1.Substring(0, 2);
                }
                else if (args.Length > 1)
                {
                    arg2 = args[1];
                }
            }
            else
            {
                // If there are no arguments, we should go into
                // screensaver mode. This way, there's only one
                // test to make for that.
                arg1 = "/s";
            }

            switch(arg1)
            {
                case "/p": // Preview mode.
                    if (arg2 == null)
                    {
                        MessageBox.Show("Sorry, but the expected window handle was not provided.",
                            "ScreenSaver", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    } 
                    IntPtr previewWndHandle = new IntPtr(long.Parse(arg2));
                    Application.Run(new ScreensaverForm(previewWndHandle));
                    break;
                case "/s": // Screensaver mode.
                    ShowScreensaver();
                    Application.Run();
                    break;
                case "/c": // Configuration mode.
                    // TODO add real configuration!
                    MessageBox.Show("Sorry, configuration not implemented yet.", "ScreenSaver",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                default:
                    MessageBox.Show("Sorry, but the command line argument \"" + arg1 + " " + arg2 +
                        "\" is not valid.", "ScreenSaver",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
            Cursor.Show();
        }

        static void ShowScreensaver()
        {
            foreach(Screen screen in Screen.AllScreens)
            {
                ScreensaverForm screensaver = new ScreensaverForm();
                screensaver.Show();
            }
        }
    }
}
