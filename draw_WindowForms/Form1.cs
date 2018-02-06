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
        Point lastPoint = new Point(0, 0);
        Stack<DrawPath> operations = new Stack<DrawPath>();
        Color selectedColor = Color.Black;
        Bitmap bmp;
        int penSize;

        void startup()
        {
            pictureBox1.BackColor = Color.White;
            pictureBox1.Cursor = Cursors.Cross;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            createColorPicker();

            toolStripButton1.Image = SystemIcons.Error.ToBitmap();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            numericUpDown1.Value = 3;
        }

        void createColorPicker()
        {
            List<Color> colors = new List<Color>();
            colors.Add(Color.Black);
            colors.Add(Color.Gray);
            colors.Add(Color.White);
            colors.Add(Color.DarkBlue);
            colors.Add(Color.Blue);
            colors.Add(Color.BlueViolet);
            colors.Add(Color.Green);
            colors.Add(Color.SeaGreen);
            colors.Add(Color.LightGreen);
            colors.Add(Color.Brown);
            colors.Add(Color.Red);
            colors.Add(Color.Pink);
            colors.Add(Color.Orange);
            colors.Add(Color.Yellow);
            colors.Add(Color.Olive);

            int row = 0;
            int col = 0;
            foreach (Color c in colors)
            {
                Panel p = new Panel();
                p.BackColor = c;
                p.Click += panel_click;
                tableLayoutPanel1.Controls.Add(p, col, row);
                col++;

                if (col % 3 == 0)
                {
                    row++;
                    col = 0;
                }
            }

            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void panel_click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            selectedColor = p.BackColor;
            panel2.BackColor = selectedColor;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPaint = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            startPaint = false;
            lastPoint = new Point(0, 0);
        }



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            showLocation(e.X, e.Y);
            if (startPaint)
            {
                Pen p = new Pen(selectedColor, penSize);
                Point pointerLocation = new Point(e.X, e.Y);

                g = Graphics.FromImage(bmp);

                Brush b = new SolidBrush(selectedColor);
                g.DrawLine(p, pointerLocation, new Point(e.X+penSize, e.Y));
                operations.Push(new DrawPath(p, pointerLocation, new Point(e.X + penSize, e.Y)));


                if (lastPoint.X != 0 || lastPoint.Y != 0)
                {
                    p.Width = p.Width;
                    g.DrawLine(p, lastPoint, pointerLocation);
                    operations.Push(new DrawPath(p, lastPoint, pointerLocation));
                }
                lastPoint = pointerLocation;

                pictureBox1.Image = bmp;
            }
        }

        void showLocation(int x, int y)
        {
            toolStripStatusLabel1.Text = "X=" + x + "," + "Y=" + y;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            Bitmap oldBmp = bmp;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            g.DrawImage(oldBmp, new Point(0, 0));
            pictureBox1.Image = bmp;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (operations.Count > 0)
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                g = Graphics.FromImage(bmp);
                operations.Pop();

                while (operations.Count > 0)
                {
                    DrawPath drawPath = operations.Pop();
                    g.DrawLine(drawPath.pen, drawPath.startPoint, drawPath.endPoint);
                }
                pictureBox1.Image = bmp;
            }
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (startPaint)
            {
                //Pen p = new Pen(Color.Black, 2);
                //Size size = new Size(1, 1);
                //Point pointerLocation = new Point(e.X, e.Y);

                //Rectangle rectangle = new Rectangle(pointerLocation, size);
                //operations.Push(rectangle);
                //g.DrawRectangle(p, rectangle);

                //while (operations.Count > 0)
                //{
                //    Rectangle r = operations.Pop();
                //    g.DrawRectangle(p, r);
                //}
                //if (lastPoint.X != 0 || lastPoint.Y != 0)
                //{
                //    g.DrawLine(p, lastPoint, pointerLocation);
                //}
                //lastPoint = pointerLocation;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmaps |*.bmp";
            DialogResult dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                Rectangle rect = new Rectangle(pictureBox1.Location, pictureBox1.Size);
                pictureBox1.DrawToBitmap(bmp, rect);
                bmp.Save(saveFileDialog.FileName);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            penSize = (int)numericUpDown1.Value;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Bitmaps |*.bmp"; 
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                file = openFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("No file selecetd!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                file = null;
            }

            bmp = (Bitmap)Image.FromFile(file);
            pictureBox1.Image = bmp;
        }
    }
}
