using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draw_WindowForms
{
    class DrawPath
    {
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public Pen pen { get; set; }

        public DrawPath(Pen pen, Point startPoint, Point endPoint)
        {
            this.pen = pen;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}
