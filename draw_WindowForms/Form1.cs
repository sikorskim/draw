using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace draw_WindowForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChangeCulture(Thread.CurrentThread.CurrentUICulture);
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
        int drawMode = 0;
        Point startPoint;
        Point endPoint;

        void startup()
        {
            pictureBox1.BackColor = Color.White;
            pictureBox1.Cursor = Cursors.Cross;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            createColorPicker();

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
            startPoint = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            startPaint = false;
            lastPoint = new Point(0, 0);
            endPoint= new Point(e.X, e.Y);

            if (drawMode == 1)
            {
                drawLine();
            }
            else if (drawMode == 2)
            {
                drawRectangle();
            }
            else if (drawMode == 3)
            {
                drawElipse();
            }
        }



        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            showLocation(e.X, e.Y);
            if (drawMode == 0)
            {
                drawFreeLines(e);
            }
        }

        void drawFreeLines(MouseEventArgs e)
        {
            if (startPaint)
            {
                Pen p = new Pen(selectedColor, penSize);
                Point pointerLocation = new Point(e.X, e.Y);

                g = Graphics.FromImage(bmp);

                Brush b = new SolidBrush(selectedColor);
                g.DrawLine(p, pointerLocation, new Point(e.X + penSize, e.Y));
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

        void drawLine()
        {
                Pen p = new Pen(selectedColor, penSize);
                g = Graphics.FromImage(bmp);

                Brush b = new SolidBrush(selectedColor);
                g.DrawLine(p, startPoint,endPoint);

                pictureBox1.Image = bmp;
        }

        void drawRectangle()
        {
            Pen p = new Pen(selectedColor, penSize);
            g = Graphics.FromImage(bmp);

            Brush b = new SolidBrush(selectedColor);
            int locX = startPoint.X;
            int locY = startPoint.Y;

            int width = startPoint.X - endPoint.X;
            int height = startPoint.Y - endPoint.Y;

            if (width > 0)
            {
                locX -= width;
            }
            else
            {
                width = Math.Abs(width);
            }

            if (height > 0)
            {
                locY -= height;
            }
            else
            {
                height = Math.Abs(height);
            }

            g.DrawRectangle(p, locX, locY, width, height);

            pictureBox1.Image = bmp;
        }

        void drawElipse()
        {
            Pen p = new Pen(selectedColor, penSize);
            g = Graphics.FromImage(bmp);

            Brush b = new SolidBrush(selectedColor);
            int locX = startPoint.X;
            int locY = startPoint.Y;

            int width = startPoint.X - endPoint.X;
            int height = startPoint.Y - endPoint.Y;

            if (width > 0)
            {
                locX -= width;
            }
            else
            {
                width = Math.Abs(width);
            }

            if (height > 0)
            {
                locY -= height;
            }
            else
            {
                height = Math.Abs(height);
            }
            g.DrawEllipse(p, locX, locY, width, height);

            pictureBox1.Image = bmp;
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
            drawMode = 0;
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
                Pen p = new Pen(Color.Black, 2);
                Size size = new Size(1, 1);
                Point pointerLocation = new Point(e.X, e.Y);

                Rectangle rectangle = new Rectangle(pointerLocation, size);

                g.DrawRectangle(p, rectangle);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "bmp|*.bmp|jpg|*.jpg|png|*.png";
            DialogResult dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                Rectangle rect = new Rectangle(pictureBox1.Location, pictureBox1.Size);
                pictureBox1.DrawToBitmap(bmp, rect);

                if (saveFileDialog.FilterIndex == 0)
                {
                    bmp.Save(saveFileDialog.FileName);
                }
                else if (saveFileDialog.FilterIndex==1)
                {
                    bmp.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                }
                else if (saveFileDialog.FilterIndex == 2)
                {
                    bmp.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
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
            openFileDialog.Filter = "Images|*.bmp;*.jpg;*.png";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                bmp = (Bitmap)Image.FromFile(file);
                pictureBox1.Image = bmp;
            }
            else
            {
                file = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void asyncDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void polskiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCulture(new CultureInfo("pl"));
        }

        private void ChangeCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            resources.ApplyResources(this, "$this", culture);
            UpdateControlsCulture(this, resources, culture);
            polskiToolStripMenuItem.Checked = (culture.Name == "pl");
            englishToolStripMenuItem.Checked = (culture.Name == "en");
            DateTime dt = DateTime.Now;
            toolStripStatusLabel2.Text = dt.ToString("d", culture) + "    " + dt.ToString("t", culture);

            
            foreach (ToolStripItem tsi in menuStrip1.Items)
            {
                UpdateToolStripItemsCulture(tsi, resources, culture);
            }
        }

        private void UpdateControlsCulture(Control control,
ComponentResourceManager resourceProvider, CultureInfo culture)
        {
            resourceProvider.ApplyResources(control, control.Name, culture);
            foreach (Control ctrl in control.Controls)
            {
                UpdateControlsCulture(ctrl, resourceProvider, culture);
            }
        }

        private void UpdateToolStripItemsCulture(ToolStripItem item, ComponentResourceManager resourceProvider, CultureInfo culture)
        {
            resourceProvider.ApplyResources(item, item.Name, culture);
            if (item is ToolStripMenuItem)
            {
                foreach (ToolStripItem it in ((ToolStripMenuItem)item).DropDownItems)
                {
                    UpdateToolStripItemsCulture(it, resourceProvider, culture);
                }
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeCulture(new CultureInfo("en"));
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            drawMode = 1;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            drawMode = 2;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            drawMode = 3;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
        }
    }
}

