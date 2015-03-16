using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graf
{
    public partial class Form1 : Form
    {
        int count = 0;
        Point[] pts = new Point[1000];

        Image baseStation = Image.FromFile("base.png");
        Image nodeStation = Image.FromFile("node.png");

        Font font = new Font("Arial", 8);
        SolidBrush brush = new SolidBrush(Color.Black);

        public Form1()
        {
            InitializeComponent();
        }

        private void drawBase(Point pts)
        {
            this.CreateGraphics().DrawImage(baseStation, pts.X, pts.Y);
            this.CreateGraphics().DrawString("Base", font, brush, pts.X+18, pts.Y+baseStation.Height-20);
        }

        private void drawLine(Point startPts, Point endPts)
        {
            startPts.X = startPts.X + baseStation.Width / 2;
            startPts.Y = startPts.Y + baseStation.Height / 2;

            endPts.X = endPts.X + nodeStation.Width / 2;
            endPts.Y = endPts.Y + nodeStation.Height / 2;

            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 3), startPts, endPts);
        }

        private void drawNode(Point pts)
        {
            this.CreateGraphics().DrawImage(nodeStation, pts.X, pts.Y);
            this.CreateGraphics().DrawString("Node", font, brush, pts.X+13, pts.Y+nodeStation.Height-20);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pts[count++] = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);

            for (int i=count-1; i >=0; i--)
            {
                if (i != 0)
                {
                    drawLine(pts[0], pts[i]);
                    drawNode(pts[i]);
                }
                else
                    drawBase(pts[0]);
            }
        }
    }
}
