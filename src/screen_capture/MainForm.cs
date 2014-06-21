/*
 *  feedback while sending
 *  "x" should be minimize
 *  capture should work from icon's context menu
 *  
 * 
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Web;

namespace btnet
{


    public partial class MainForm : Form
    {

        // Native stuff

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        // The original bitmap
        Bitmap bitmap = null;

        // The bitmap after we've drawn on it
        Bitmap bitmapWithStrokes = null;

        // For drawing
        List<Stroke> strokes = new List<Stroke>();
        Stroke currentStroke = null;
        Pen penRedMarker = new Pen(Color.Red, 3);
        Pen penRedArrow = new Pen(Color.Red, 3);
        Pen penYellowHighlighter = null;
        byte yellowsRedComponent = 0xFF; // Color.Yellow.R;


        // Declare an array to hold the bytes of the bitmap.
        int numberOfBytes = 0;

        bool reallyClose = false;

        bool rbfIsBeingShown = false;

        delegate void SimpleDelegeate();

        public MainForm()
        {

            //Ash <2010-08-03>
            // Removing warning for Obsolete class (After Framework 2.0)
            // Basically All certificates will be accepted.
            //ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    delegate
                    {
                        return true;
                    }
                 );
            //End Ash <2010-08-03>

            penRedArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6);
            //Color transparentYellowColor = Color(Color.Yellow);
            penYellowHighlighter = new Pen(Color.Yellow, 14);

            InitializeComponent();
            this.toolStripComboBoxPenType.SelectedIndex = 0;

            EnableDisable(false);

            this.Width = Program.main_window_width;
            this.Height = Program.main_window_height;

            this.KeyPreview = true; // for capturing CTRL-C

            this.pictureBox1.Cursor = Cursors.Hand; // really, I should have a Sharpie cursor

            // For the notify icon
            this.Resize += new EventHandler(MainForm_Resize);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);

            saveFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            // Add menu items to context menu.
            ContextMenu cm = new ContextMenu();
            MenuItem notifyIconOpen = cm.MenuItems.Add("Open");
            notifyIconOpen.Click += new EventHandler(notifyIcon1_DoubleClick);

            MenuItem notifyIconCapture = cm.MenuItems.Add("Capture");
            notifyIconCapture.Click += new EventHandler(this.buttonCapture_Click);

            MenuItem notifyIconExit = cm.MenuItems.Add("Exit");
            notifyIconExit.Click += new EventHandler(buttonExit_Click);

            notifyIcon1.ContextMenu = cm;

        }

        private void EnableDisable(bool EnabledOrDisabled)
        {
            // disable toolbar
            this.toolStripButtonCopy.Enabled = EnabledOrDisabled;
            this.toolStripButtonSaveAs.Enabled = EnabledOrDisabled;
            this.toolStripButtonUndo.Enabled = EnabledOrDisabled;
            this.toolStripComboBoxPenType.Enabled = EnabledOrDisabled;

            // disable send fields
            buttonSend.Enabled = EnabledOrDisabled;
            labelDescription.Enabled = EnabledOrDisabled;
            textBoxShortDescription.Enabled = EnabledOrDisabled;
            radioButtonCreateNew.Enabled = EnabledOrDisabled;
            radioButtonUpdateExisting.Enabled = EnabledOrDisabled;
            labelBugId.Enabled = EnabledOrDisabled;
            textBoxBugId.Enabled = EnabledOrDisabled;

            enableDisableDelay();


        }

        private Bitmap getBitmap()
        {
            if (bitmapWithStrokes != null)
                return bitmapWithStrokes;
            else
                return bitmap;
        }


        private void ShowRubberBandForm()
        {
            if (rbfIsBeingShown)
                return;

            rbfIsBeingShown = true;

            if (bitmap != null)
            {
                this.pictureBox1.Image = null;
                bitmap.Dispose();
                strokes.Clear();
            }


            using (RubberBandForm rbf = new RubberBandForm(this))
            {

                rbf.ShowDialog();

                //Ash <2010-08-03>
                // To remove the "marshal-by-reference" warning we declare the last size as
                // a local variable.
                Size sLastSize = rbf.lastSize;

                //if (rbf.lastSize.Width > 0 && rbf.lastSize.Height > 0)
                if (sLastSize.Width > 0 && sLastSize.Height > 0)
                {
                    Rectangle r = new Rectangle();
                    r.Location = rbf.lastLoc;
                    //r.Size = rbf.lastSize;
                    r.Size = sLastSize;
                    CaptureBitmap(r);
                }
                //End Ash <2010-08-03>
            }

            this.Show();
            rbfIsBeingShown = false;
        }

        private void CaptureBitmap(Rectangle r)
        {
            bitmap = new Bitmap(r.Width, r.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(r.Location, new Point(0, 0), r.Size);
            }
            
            this.pictureBox1.Image = bitmap;

            if (bitmapWithStrokes != null)
            {
                bitmapWithStrokes.Dispose();
                bitmapWithStrokes = null;
            }

            System.Drawing.Imaging.BitmapData bitmapData =
                bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

            numberOfBytes = Math.Abs(bitmapData.Stride) * bitmap.Height;

            bitmap.UnlockBits(bitmapData);

            EnableDisable(true);

        }

        private void Delay()
        {
            // delay...
            System.Threading.Thread.Sleep(500 + (1000 * (int)numericUpDownDelay.Value));
        }
        
        private void CaptureForeground()
        {

            Delay();
            
            // Get foreground window rect using native calls
            IntPtr hWnd = GetForegroundWindow();
            RECT rct = new RECT();
            GetWindowRect(hWnd, ref rct);

            Rectangle r = new Rectangle();
            r.Location = new Point(rct.Left, rct.Top);
            r.Size = new Size(rct.Right - rct.Left, rct.Bottom - rct.Top);
            CaptureBitmap(r);

            this.Show();
        }


        private void CaptureFull()
        {
            Delay();

            // Current screen
            Screen screen = Screen.FromControl(this);
            CaptureBitmap(screen.Bounds);

            this.Show();
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (radioButtonArea.Checked)
            {
                this.Hide();
                ShowRubberBandForm();
            }
            else if (radioButtonForeground.Checked)
            {
                this.Hide();
                BeginInvoke(new SimpleDelegeate(CaptureForeground));
            }
            else
            {
                this.Hide();
                BeginInvoke(new SimpleDelegeate(CaptureFull));
            }
        }

        private void notifyIcon1_DoubleClick(object sender,
                                             System.EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void MainForm_Resize(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (bitmap == null)
                return;

            currentStroke = new Stroke();
            strokes.Add(currentStroke);
            currentStroke.points.Add(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                // arrow
                currentStroke.drawingMode = Stroke.DrawingMode.RedArrow;
                currentStroke.points.Add(e.Location); // and the line's endpoint
            }
            else
            {
                if (this.toolStripComboBoxPenType.Text == "red arrow")
                {
                    // arrow
                    currentStroke.drawingMode = Stroke.DrawingMode.RedArrow;
                    currentStroke.points.Add(e.Location); // and the line's endpoint
                }
                else if (this.toolStripComboBoxPenType.Text == "red marker")
                {
                    currentStroke.drawingMode = Stroke.DrawingMode.RedMarker;
                }
                else
                {
                    currentStroke.drawingMode = Stroke.DrawingMode.YellowHighlighter;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (bitmap == null)
                return;


            if (currentStroke != null)
            {
                if (currentStroke.drawingMode == Stroke.DrawingMode.RedArrow)
                {
                    currentStroke.points[currentStroke.points.Count - 1] = e.Location; // replace endpoint
                }
                else
                {
                    currentStroke.points.Add(e.Location);
                }
            }

            drawStrokes();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (bitmap == null)
                return;


            if (currentStroke != null)
            {
                if (currentStroke.drawingMode == Stroke.DrawingMode.RedArrow)
                {
                    currentStroke.points[currentStroke.points.Count - 1] = e.Location; // replace endpoint
                }
                else
                {
                    currentStroke.points.Add(e.Location);
                }
            }
            currentStroke = null;
            drawStrokes();
        }

        private void drawStrokes()
        {
            if (bitmapWithStrokes != null)
                bitmapWithStrokes.Dispose();

            bitmapWithStrokes = new Bitmap(bitmap);

            using (Graphics g = Graphics.FromImage(bitmapWithStrokes))
            {
                for (int i = 0; i < strokes.Count; i++)
                {
                    Stroke stroke = strokes[i];
                    if (stroke.drawingMode == Stroke.DrawingMode.RedArrow)
                    {
                        g.DrawLine(penRedArrow, stroke.points[0], stroke.points[1]);
                    }
                    else if (stroke.drawingMode == Stroke.DrawingMode.RedMarker)
                    {
                        g.DrawLines(penRedMarker, stroke.points.ToArray());
                    }
                    else if (stroke.drawingMode == Stroke.DrawingMode.YellowHighlighter)
                    {
                        using (Bitmap tempBitmap = new Bitmap(bitmap.Width, bitmap.Height))
                        {
                            using (Graphics tempG = Graphics.FromImage(tempBitmap))
                            {

                                tempG.DrawLines(penYellowHighlighter, stroke.points.ToArray());

                                // get the raw bits of the source and target and remove the blue from every
                                // bit of the target where there is a yellow bit of the source
                                Rectangle rect = new Rectangle(0, 0, bitmapWithStrokes.Width, bitmapWithStrokes.Height);

                                // lock
                                System.Drawing.Imaging.BitmapData sourceData =
                                    tempBitmap.LockBits(
                                        rect,
                                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                        tempBitmap.PixelFormat);

                                System.Drawing.Imaging.BitmapData targetData =
                                    bitmapWithStrokes.LockBits(
                                        rect,
                                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                        bitmapWithStrokes.PixelFormat);

                                // Get the address of the first line.
                                IntPtr sourcePtr = sourceData.Scan0;
                                IntPtr targetPtr = targetData.Scan0;

                                // loop thru the source bytes
                                unsafe
                                {
                                    byte* s = (byte*) sourcePtr.ToPointer();
                                    byte* t = (byte*) targetPtr.ToPointer();

                                    for (int p = 2; p < numberOfBytes; p += 4)
                                    {
                                        // if the source's red is yellows's red
                                        if (s[p] == yellowsRedComponent)
                                        {
                                            // wipe out the target's blue
                                            t[p-2] = 0;
                                        }
                                    }
                                }

                                // Unlock the bits.
                                tempBitmap.UnlockBits(sourceData);
                                bitmapWithStrokes.UnlockBits(targetData);
                            }
                        }
                    }
                }
            }
            pictureBox1.Image = bitmapWithStrokes;
            //pictureBox1.Invalidate();

            this.toolStripButtonUndo.Enabled = strokes.Count > 0;

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (radioButtonUpdateExisting.Checked)
            {
                if (textBoxBugId.Text == "")
                {
                    MessageBox.Show("Please enter a Bug ID#");
                    return;
                }
            }


            if (String.IsNullOrEmpty(Program.url)
            || String.IsNullOrEmpty(Program.username)
            || String.IsNullOrEmpty(Program.password))
            {
                buttonConfigure_Click(null, null);
            }

            if (String.IsNullOrEmpty(Program.url)
            || String.IsNullOrEmpty(Program.username)
            || String.IsNullOrEmpty(Program.password))
            {
                // skip send button
            }
            else
            {

                // labelWaiting.Text = "Waiting for response...";
                this.Cursor = Cursors.WaitCursor;

                // The domain, windows authentication stuff here is from Lars Wuckel.  It's pretty
                // difficult code for me, so I only want to go down that logical path if I have to.
                if (Program.domain != "")
                {
                    System.Threading.Thread thread = new System.Threading.Thread(threadproc_with_domain);
                    thread.Start(this);
                }
                else
                {
                    System.Threading.Thread thread = new System.Threading.Thread(threadproc);
                    thread.Start(this);
                }

            }
        }

        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            using (ConfigForm dlg = new ConfigForm())
            {
                dlg.ShowDialog();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            reallyClose = true;
            this.Close();
        }

        private void radioButtonArea_CheckedChanged(object sender, EventArgs e)
        {
            enableDisableDelay();
        }

        private void radioButtonForeground_CheckedChanged(object sender, EventArgs e)
        {
            enableDisableDelay();
        }

        private void radioButtonDesktop_CheckedChanged(object sender, EventArgs e)
        {
            enableDisableDelay();
        }

        private void enableDisableDelay()
        {
            if (radioButtonArea.Checked)
            {
                labelDelay.Enabled = false;
                numericUpDownDelay.Enabled = false;
            }
            else
            {
                labelDelay.Enabled = true;
                numericUpDownDelay.Enabled = true;
            }
        }

        private void radioButtonCreateNew_CheckedChanged(object sender, EventArgs e)
        {
            enableDisableBugId();
        }

        private void radioButtonUpdateExisting_CheckedChanged(object sender, EventArgs e)
        {
            enableDisableBugId();
        }

        private void enableDisableBugId()
        {
            if (radioButtonCreateNew.Checked)
            {
                labelBugId.Enabled = false;
                textBoxBugId.Enabled = false;
            }
            else if (radioButtonUpdateExisting.Checked)
            {
                labelBugId.Enabled = true;
                textBoxBugId.Enabled = true;
            }
        }

        delegate void ResponseDelegate(object obj);

        void handleResponse(object obj)
        {
            HttpWebResponse res = null;
            Exception e = null;

            if (obj is Exception)
            {
                e = (Exception)obj;
            }
            else
            {
                res = (HttpWebResponse)obj;
            }



            if (e != null)
            {
                MessageBox.Show("Sending of screenshot failed.\n\n" + e.Message);
            }
            else if (res != null)
            {

                int http_status = (int)res.StatusCode;

                string http_response_header = res.Headers["BTNET"];
                res.Close();

                if (http_response_header != null)
                {
                    if (http_response_header.IndexOf("OK") == 0)
                    {
                        string bugid = http_response_header.Substring(3);
                        DialogResult result = MessageBox.Show("Posted screenshot to Bug ID# "
                            + bugid
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Go to the BugTracker.NET website?", "BugTracker.NET", MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(Program.url + "/edit_bug.aspx?id=" + bugid);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sending of screenshot failed.\n\n" + http_response_header);
                    }
                }
            }

            //labelWaiting.Text = "";
            this.Cursor = Cursors.Default;

        }

        public static void threadproc(object obj)
        {
            MainForm frm = (MainForm)obj;

            StringBuilder sb = new StringBuilder();

            sb.Append("username=" + HttpUtility.UrlEncode(Program.username));
            sb.Append("&password=" + HttpUtility.UrlEncode(Program.password));
            sb.Append("&short_desc=" + HttpUtility.UrlEncode(frm.textBoxShortDescription.Text));
            sb.Append("&projectid=" + Convert.ToString(Program.project_id));
            if (frm.radioButtonUpdateExisting.Checked)
            {
                sb.Append("&bugid=" + frm.textBoxBugId.Text);
            }
            sb.Append("&attachment_content_type=image/jpg");
            sb.AppendFormat("&attachment_filename=screenshot_{0}.jpg", DateTime.Now.ToString("yyyyMMdd'_'HHmmss"));

            

            //Ash <2010-08-03>
            //sb.Append("&attachment_desc=screenshot");
            sb.Append("&attachment_desc=" + HttpUtility.UrlEncode(frm.textBoxShortDescription.Text));
            //End Ash <2010-08-03>
            sb.Append("&attachment=");

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            frm.getBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            string base64 = System.Convert.ToBase64String(ms.ToArray());
            ms.Close();
            ms.Dispose();
            sb.Append(HttpUtility.UrlEncode(base64));

            //  System.Byte[] byte_array2 = System.Convert.FromBase64String(base64);

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

            // send request to web server
            HttpWebResponse res = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)System.Net.WebRequest.Create(Program.url + "/insert_bug.aspx");


                req.Credentials = CredentialCache.DefaultCredentials;
                req.PreAuthenticate = true;

                //req.Timeout = 200; // maybe?
                //req.KeepAlive = false; // maybe?

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bytes.Length;
                System.IO.Stream request_stream = req.GetRequestStream();
                request_stream.Write(bytes, 0, bytes.Length);
                request_stream.Close();

                res = (HttpWebResponse)req.GetResponse();
                frm.BeginInvoke(new MainForm.ResponseDelegate(frm.handleResponse), res);
            }
            catch (Exception e2)
            {
                frm.BeginInvoke(new MainForm.ResponseDelegate(frm.handleResponse), e2);
            }


        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (reallyClose)
            {
                return;
            }

            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (FormWindowState.Normal == WindowState)
                {
                    Program.main_window_width = this.Size.Width;
                    Program.main_window_height = this.Size.Height;
                    ConfigForm.WriteConfig();
                }
                Hide();
                e.Cancel = true;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    if (bitmap != null)
                    {
                        Clipboard.SetImage(this.getBitmap());
                    }
                }
                else if (e.KeyCode == Keys.Z)
                {
                    toolStripButtonUndo_Click(null, null);
                }
            }
        }

        private void toolStripButtonSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = String.Format("btnet_screenshot_{0}.jpg", DateTime.Now.ToString("yyyyMMdd'_'HHmmss")); 

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream myStream;
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    Bitmap b = this.getBitmap();
                    b.Save(myStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    myStream.Close();
                }
            }

        }

        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(this.getBitmap());
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            if (strokes.Count == 0)
                return;

            strokes.RemoveAt(strokes.Count - 1);
            drawStrokes();

        }

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm dlg = new AboutForm())
            {
                dlg.ShowDialog();
            }
        }

        static void send(MainForm frm)
        {
            Uri URL = new Uri(Program.url + "/insert_bug.aspx");

            ExtendedWebClient extendedWebClient = new ExtendedWebClient();
            CredentialCache myCredCache = new CredentialCache();
            myCredCache.Add(URL, "Basic", new NetworkCredential(Program.username, Program.password));
            myCredCache.Add(URL, "NTLM", new NetworkCredential(Program.username, Program.password, Program.domain));
            extendedWebClient.Credentials = myCredCache;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            frm.getBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            string base64 = System.Convert.ToBase64String(ms.ToArray());
            ms.Close();
            ms.Dispose();

            try
            {
                // Anmelden per POST; anonymer Typ als Parameterobjekt
                WebRequest req = extendedWebClient.Post(URL, new
                {
                    username = Program.username,
                    password = Program.password,
                    bugid = frm.textBoxBugId.Text,
                    short_desc = frm.textBoxShortDescription.Text,
                    projectid = Convert.ToString(Program.project_id),
                    attachment_content_type = "image/jpg",
                    attachment_filename = String.Format("screenshot_{0}.jpg", DateTime.Now.ToString("yyyyMMdd'_'HHmmss")),
                    attachment = base64
                });

                WebResponse res = (WebResponse)req.GetResponse();
                frm.BeginInvoke(new MainForm.ResponseDelegate(frm.handleResponse), res);
            }
            catch (Exception e2)
            {
                frm.BeginInvoke(new MainForm.ResponseDelegate(frm.handleResponse), e2);
            }
        }

        public static void threadproc_with_domain(object obj)
        {
            MainForm frm = (MainForm)obj;
            send(frm);
        }

        static bool EnablePreAuthentication(Uri uri, string authenticationType)
        {
            System.Collections.IEnumerator e = AuthenticationManager.RegisteredModules;

            while (e.MoveNext())
            {
                IAuthenticationModule module = e.Current as IAuthenticationModule;

                if (string.Compare(module.AuthenticationType, authenticationType, true) == 0)
                {

                    System.Reflection.MethodInfo mi = typeof(AuthenticationManager).GetMethod("BindModule",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                    mi.Invoke(null, new object[] { uri, new Authorization(null), module });

                    return true;
                }
            }
            return false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ifdefined.com/Donate_to_BugTracker.NET.html");
        }

    }

    class Stroke
    {
        public enum DrawingMode
        {
            RedArrow,
            RedMarker,
            YellowHighlighter,
        };
        public DrawingMode drawingMode;
        public List<Point> points = new List<Point>();
    };


}



// For siliently accepting suspicious SSL certificates
class AcceptAllCertificatePolicy : ICertificatePolicy
{
    public AcceptAllCertificatePolicy()
    {
    }

    public bool CheckValidationResult(
    ServicePoint service_point,
    System.Security.Cryptography.X509Certificates.X509Certificate cert,
    WebRequest web_request,
    int certificate_problem)
    {
        // Always accept
        return true;
    }
}