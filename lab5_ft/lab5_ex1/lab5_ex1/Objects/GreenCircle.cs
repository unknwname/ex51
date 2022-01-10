using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace lab5_ex1.Objects
{
    class GreenCircle : BaseObject
    {
        public int time = 0;
        public GreenCircle(float x, float y, float angle) : base(x, y, angle)
        {

        }
        public override void Render(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.YellowGreen), 0, 0, 30, 30);
            g.DrawString($"{time}", new Font("Arial Black", 8), new SolidBrush(Color.Crimson), 3, 7);
        }
        public override GraphicsPath GetGraphicsPath()
        {
            var path = base.GetGraphicsPath();
            path.AddEllipse(0, 0, 30, 30);
            return path;
        }
    }
}
