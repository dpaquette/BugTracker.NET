using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace btnet
{
    static class Program
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static string url;
        public static string username;
        public static string password;
        public static string domain;
        public static string save_password;
        public static int main_window_width;
        public static int main_window_height;
        public static int project_id;

        [STAThread]
        static void Main()
        {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "MyApplicationName", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    // fetch settings
                    url = ConfigurationManager.AppSettings["url"];
                    username = ConfigurationManager.AppSettings["username"];
                    password = ConfigurationManager.AppSettings["password"];
                    domain = ConfigurationManager.AppSettings["domain"];
                    save_password = ConfigurationManager.AppSettings["save_password"];
                    string tmp = ConfigurationManager.AppSettings["main_window_width"];
                    if (!String.IsNullOrEmpty(tmp))
                        main_window_width = Convert.ToInt32(tmp);
                    tmp = ConfigurationManager.AppSettings["main_window_height"];
                    if (!String.IsNullOrEmpty(tmp))
                        main_window_height = Convert.ToInt32(tmp);
                    tmp = ConfigurationManager.AppSettings["project_id"];
                    if (!String.IsNullOrEmpty(tmp))
                        project_id = Convert.ToInt32(tmp);
                    else
                        project_id = 0;


                    Application.Run(new MainForm());
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }
    }
}
