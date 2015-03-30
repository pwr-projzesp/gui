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
        int index=0;
        int maxlenght = 0;  
        Point loc = new Point();                                // zmienna sluzaca do przechowywania polozenia ( (0;0) - gorny prawy rog okna aplikacji)
        List<Node> nodes = new List<Node>();                    // kolekcja zawierajace wszystkie urzadzenia

        Image baseStation = Image.FromFile("base.png");         // grafika stacji bazowej
        Image nodeStation = Image.FromFile("node.png");         // grafika stacji roboczej
        Font font = new Font("Arial", 8);                       // typ oraz rozmiar czcionki uzywanej do podpisywania elementow na rysunku
        SolidBrush brush = new SolidBrush(Color.Black);         // kolor pedzla sluzacego do rysowania polaczen miedzy urzedzeniami

        private bool isDragging = false;                        // zmienna sluzaca do przechowywania informacji czy aktualnie jakis obraz jest przenoszony
        private int xPos, yPos;                         // zmienne sluzace do przechowywania aktualnego polozenia
        private PictureBox picture;
        private List<PictureBox> pictureBoxes = new List<PictureBox>();

        public Form1()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();          
        }

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji bazowej
        private void drawBase(Point loc, int ID)
        {
            //this.CreateGraphics().DrawImage(baseStation, loc.X, loc.Y);
            //this.CreateGraphics().DrawString(ID + " Base", font, brush, loc.X+10, loc.Y+baseStation.Height-20);
            String nazwa = "Base";
            nazwa += ID;
            picture = new PictureBox();
            //picture.SizeMode = PictureBoxSizeMode.StretchImage;
            picture.Location = new System.Drawing.Point(loc.X, loc.Y);
            picture.Image = new Bitmap("base.png");
            picture.Size = picture.Image.Size;
            picture.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(nazwa, Font, Brushes.Black, 0, 0);
            });
            picture.MouseDoubleClick += new MouseEventHandler((sender, e) =>
            {
                MessageBox.Show("Info", nazwa);
            });
            picture.MouseDown += new MouseEventHandler(picture_MouseDown);
            picture.MouseMove += new MouseEventHandler(picture_MouseMove);
            picture.Cursor = Cursors.Hand;
            Controls.Add(picture);
            pictureBoxes.Add(picture);
        }
        
        // Metoda sluzaca do narysowania na ekranie pojedynczej polaczenia miedzy urzadzeniami
        private void drawLine(Point startloc, Point endloc)
        {
            startloc.X = startloc.X + baseStation.Width / 2;
            startloc.Y = startloc.Y + baseStation.Height / 2;

            endloc.X = endloc.X + nodeStation.Width / 2;
            endloc.Y = endloc.Y + nodeStation.Height / 2;

            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 3), startloc, endloc);
        }

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji roboczej
        private void drawNode(Point loc, int ID)
        {
            // this.CreateGraphics().DrawImage(nodeStation, loc.X, loc.Y);
            // this.CreateGraphics().DrawString(ID + " Node", font, brush, loc.X+5, loc.Y+nodeStation.Height-20);
            String nazwa = "Node";
            nazwa += ID;
            picture = new PictureBox();
            picture.Location = new System.Drawing.Point(loc.X, loc.Y);
            picture.Image = new Bitmap("node.png");
            picture.Size = picture.Image.Size;
            picture.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(nazwa, Font, Brushes.Black, 0, 0);
            });
            picture.MouseDoubleClick += new MouseEventHandler((sender, e) => 
            {
                MessageBox.Show("Info", nazwa);           
            });
            picture.MouseDown += new MouseEventHandler(picture_MouseDown);
            picture.MouseMove += new MouseEventHandler(picture_MouseMove);
            picture.Cursor = Cursors.Hand;
            Controls.Add(picture);
            pictureBoxes.Add(picture);

        }

        // Metoda wywolywana po nacisnieciu przycisku "Dodaj urzadzenie"
        // dodaje nowe urzadzenie o podanych parametrach do kolekcji urzadzen
        private void button1_Click(object sender, EventArgs e)
        {
            loc = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            Node station = new Node(nodes.Count(), loc, checkType.Checked);
            //int[] tab = { 0 };
            //station.setConnDev(tab);
            nodes.Add(station);
            count++;
        }

        // Metoda wywolywana po nacisnieciu przycisku "Rysuj"
        // wizualizuje na ekranie wszystkie obiekty kolekcji z urzadzeniami
        private void button2_Click(object sender, EventArgs e)
        {
            //pictureBoxes.Clear();
            for (int i = index; i < nodes.Count(); i++)
            {
                Node temp = nodes[i];
                if (!temp.getType())
                {
                    drawNode(temp.getLoc(), temp.getID());
                }
                else
                {
                    drawBase(temp.getLoc(), temp.getID());
                }
            }
            index = nodes.Count;

        }


        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                xPos = e.X;
                yPos = e.Y;
            }
        }

        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox p = sender as PictureBox;

            if (p != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    p.Top += (e.Y - yPos);
                    p.Left += (e.X - xPos);
                    this.Invalidate();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Connections();
            for (int i = 0; i < nodes.Count; i++)
            {
                List<int> neighbours = nodes[i].getConnDev();
                for (int j = 0; j < neighbours.Count; j++)
                {
                    Point begin = new Point();
                    Point end = new Point();
                    begin.X = pictureBoxes[j].Location.X + pictureBoxes[j].Width / 2;
                    begin.Y = pictureBoxes[j].Location.Y + pictureBoxes[j].Height / 2;
                    end.X = pictureBoxes[i].Location.X + pictureBoxes[i].Width / 2;
                    end.Y = pictureBoxes[i].Location.Y + pictureBoxes[i].Height / 2;
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Green, 2), begin,end);
                }
            }
        }
        private int getlenght(Point start, Point end)
        { 
            double lenght;
            lenght = Math.Sqrt(Math.Pow((end.Y - start.Y), 2) + Math.Pow((end.X - start.X), 2));
            return (int)lenght;
        }
        private void Connections()
        {
            int len;
            maxlenght = (int)numericUpDown3.Value;
            for (int i = 0; i < nodes.Count; i++)
            {    
                List<Node> neighbours = new List<Node>();
                for (int j = 0; j < nodes.Count; j++)
                {
                    len = getlenght(nodes[i].getLoc(), nodes[j].getLoc());
                    if (len < maxlenght && !nodes[j].getisconnected())
                    {
                        neighbours.Add(nodes[j]);
                        nodes[j].setisconnected(true);
                    }
                }
                int []tab = new int [neighbours.Count];
                for(int k=0;k<neighbours.Count;k++)
                    tab[k]=neighbours[k].getID();
                nodes[i].setConnDev(tab);
            }
        }

     }
}
