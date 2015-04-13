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
        string info;
        enum brushColor{
            black = 0,
            green = 1,
            yellow = 2,
            red = 3
        };

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
            picture.MouseUp += new MouseEventHandler(picture_MouseUp);
            picture.Cursor = Cursors.Hand;
            Controls.Add(picture);
            pictureBoxes.Add(picture);
        }
        
        // Metoda sluzaca do narysowania na ekranie pojedynczej polaczenia miedzy urzadzeniami
        private void drawLine(Point startloc, Point endloc, char color)
        {
            startloc.X = startloc.X + baseStation.Width / 2;
            startloc.Y = startloc.Y + baseStation.Height / 2;

            endloc.X = endloc.X + nodeStation.Width / 2;
            endloc.Y = endloc.Y + nodeStation.Height / 2;

            if (color == 'b')
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 3), startloc, endloc);
            else if (color == 'g')
                this.CreateGraphics().DrawLine(new Pen(Brushes.Green, 3), startloc, endloc);
            else if (color == 'y')
                this.CreateGraphics().DrawLine(new Pen(Brushes.Yellow, 3), startloc, endloc);
            else if (color == 'r')
                this.CreateGraphics().DrawLine(new Pen(Brushes.Red, 3), startloc, endloc);
        }

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji roboczej
        private void drawNode(Point loc, int ID)
        {
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

            picture.MouseClick += new MouseEventHandler((sender, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right) 
                { 
                    int range = (int)numericUpDown3.Value;
                    this.CreateGraphics().DrawEllipse(new Pen(Brushes.Green, 2), pictureBoxes[ID].Location.X - range, pictureBoxes[ID].Location.Y - range, 2 * range, 2 * range);
                
                }
            });
 
            picture.MouseDoubleClick += new MouseEventHandler((sender, e) => 
            {
                Point loca = nodes[ID].getLoc();
                List<int> nb = nodes[ID].getConnDev();
                string nbhood = string.Join(", ", nb);

                info = "ID: " + ID + " \nPolozenie: " + pictureBoxes[ID].Location.X + " " + pictureBoxes[ID].Location.Y + " \nSasiedzi: " + nbhood;
                MessageBox.Show(info, nazwa);           
            });
            picture.MouseDown += new MouseEventHandler(picture_MouseDown);
            picture.MouseMove += new MouseEventHandler(picture_MouseMove);
            picture.MouseUp += new MouseEventHandler(picture_MouseUp);
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
            copyLocation();
            connections();
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

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            copyLocation();
            connections();
        }

        private void copyLocation()
        {
            for (int i = 0; i < nodes.Count(); i++)
            {
                nodes[i].setLoc(pictureBoxes[i].Location);
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            connections();

            for (int i = 0; i < nodes.Count; i++)
            {
                Point begin = nodes[i].getLoc();
                List<int> neighbours = nodes[i].getConnDev();

                for (int j = 0; j < neighbours.Count; j++)
                {
                    Point end = nodes[neighbours[j]].getLoc();
                    drawLine(begin, end, 'b');
                }
            }
        }

        private int getLenght(Point start, Point end)
        { 
            double lenght;
            lenght = Math.Sqrt(Math.Pow((end.Y - start.Y), 2) + Math.Pow((end.X - start.X), 2));
            return (int)lenght;
        }

        private void connections()
        {
            for (int i = 0; i < nodes.Count(); i++)
            {
                nodes[i].clearConnDev();
            }

            maxlenght = (int)numericUpDown3.Value;

            for (int i = 0; i < nodes.Count(); i++)
            {
                List<int> neighbours = new List<int>();
                for (int j = 0; j < nodes.Count(); j++)
                {
                    if (nodes[i].getID() != nodes[j].getID())
                    {
                        int len = getLenght(nodes[i].getLoc(), nodes[j].getLoc());
                        if (len < maxlenght)
                        {
                            neighbours.Add(nodes[j].getID());
                        }
                    }
                }
                nodes[i].setConnDev(neighbours);
            }
        }

     }
}
