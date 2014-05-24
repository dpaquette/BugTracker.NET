using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace btnet
{
    public partial class RubberBandForm : Form
    {
        public Point lastLoc;
        public Size lastSize;

        bool mouseDown = false;
        Point mouseDownPoint = Point.Empty;
        Point mousePoint = Point.Empty;
        MainForm mainform;
        Pen pen;
        Rectangle bounds = new Rectangle();

        public RubberBandForm(MainForm mainform)
        {
            this.mainform = mainform;
            InitializeComponent();
            this.TopMost = true;
            this.Opacity = .30;
            this.TransparencyKey = System.Drawing.Color.White;
            this.Location = new Point(0, 0);
            DoubleBuffered = true;
            pen = new Pen(System.Drawing.Color.DarkRed, 3);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            int maxX = 0;
            int maxY = 0;

            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                int x = screen.Bounds.X + screen.Bounds.Width;
                if (x > maxX)
                    maxX = x;
                int y = screen.Bounds.Y + screen.Bounds.Height;
                if (y > maxY)
                    maxY = y;

            }
            bounds.X = 0;
            bounds.Y = 0;
            bounds.Width = maxX;
            bounds.Height = maxY;

            this.Size = new Size(bounds.Width, bounds.Height);

        }



        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;
            mousePoint = mouseDownPoint = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;

            // corey
            this.lastLoc = new Point(Math.Min(mouseDownPoint.X, mousePoint.X), Math.Min(mouseDownPoint.Y, mousePoint.Y));
            this.lastSize = new Size(Math.Abs(mouseDownPoint.X - mousePoint.X), Math.Abs(mouseDownPoint.Y - mousePoint.Y));
            this.Close();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousePoint = e.Location;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            Region region = new Region(bounds);

            if (mouseDown)
            {

                Rectangle selectionWindow = new Rectangle(
                    Math.Min(mouseDownPoint.X, mousePoint.X),
                    Math.Min(mouseDownPoint.Y, mousePoint.Y),
                    Math.Abs(mouseDownPoint.X - mousePoint.X),
                    Math.Abs(mouseDownPoint.Y - mousePoint.Y));

                // make a hole, where we can see thru this form
                region.Xor(selectionWindow);

                e.Graphics.FillRegion(Brushes.Black, region);

            }
            else
            {
                e.Graphics.FillRegion(Brushes.LightGray, region);
                e.Graphics.DrawLine(pen,
                    mousePoint.X, 0,
                    mousePoint.X, this.Size.Height);
                e.Graphics.DrawLine(pen,
                    0, mousePoint.Y,
                    this.Size.Width, mousePoint.Y);

            }
        }


    }
}
