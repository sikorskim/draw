using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace draw_WindowForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            startup();
        }

        bool startPaint = false;
        Graphics g;
        Point lastPoint=new Point(0,0);

        void startup()
        {
            pictureBox1.BackColor = Color.Aqua;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPaint = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            startPaint = false;
            lastPoint = new Point(0,0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            showLocation(e.X, e.Y);
            if (startPaint)
            {
                Pen p = new Pen(Color.Black, 2);
                Size size = new Size(1, 1);
                Point pointerLocation = new Point(e.X, e.Y);

                Rectangle rectangle = new Rectangle(pointerLocation, size);
                g.DrawRectangle(p, rectangle);
                if (lastPoint.X!=0||lastPoint.Y!=0)
                {
                    g.DrawLine(p, lastPoint, pointerLocation);
                }
                lastPoint = pointerLocation;
            }
        }

        void showLocation(int x, int y)
        {
            toolStripStatusLabel1.Text="X="+x+","+"Y="+y;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
