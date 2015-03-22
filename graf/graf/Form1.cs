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
        Point loc = new Point();                                // zmienna sluzaca do przechowywania polozenia ( (0;0) - gorny prawy rog okna aplikacji)
        List<Node> nodes = new List<Node>();                    // kolekcja zawierajace wszystkie urzadzenia

        Image baseStation = Image.FromFile("base.png");         // grafika stacji bazowej
        Image nodeStation = Image.FromFile("node.png");         // grafika stacji roboczej
        Font font = new Font("Arial", 8);                       // typ oraz rozmiar czcionki uzywanej do podpisywania elementow na rysunku
        SolidBrush brush = new SolidBrush(Color.Black);         // kolor pedzla sluzacego do rysowania polaczen miedzy urzedzeniami

        private bool isDragging = false;                        // zmienna sluzaca do przechowywania informacji czy aktualnie jakis obraz jest przenoszony
        private int currentX, currentY;                         // zmienne sluzace do przechowywania aktualnego polozenia
        private PictureBox picture;
        private List<PictureBox> pictureBoxes = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
        }

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji bazowej
        private void drawBase(Point loc, int ID)
        {
            //this.CreateGraphics().DrawImage(baseStation, loc.X, loc.Y);
            //this.CreateGraphics().DrawString(ID + " Base", font, brush, loc.X+10, loc.Y+baseStation.Height-20);
            
            picture = new PictureBox();
            //picture.SizeMode = PictureBoxSizeMode.StretchImage;
            picture.Location = new System.Drawing.Point(loc.X, loc.Y);
            picture.Image = new Bitmap("base.png");
            picture.Size = picture.Image.Size;
            picture.MouseDown += new MouseEventHandler(picture_MouseDown);
            picture.MouseUp += new MouseEventHandler(picture_MouseUp);
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
            picture = new PictureBox();
            //picture.SizeMode = PictureBoxSizeMode.StretchImage;
            picture.Location = new System.Drawing.Point(loc.X, loc.Y);
            picture.Image = new Bitmap("node.png");
            picture.Size = picture.Image.Size;
            picture.MouseDown += new MouseEventHandler(picture_MouseDown);
            picture.MouseUp += new MouseEventHandler(picture_MouseUp);
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
            nodes.Add(station);
            count++;
        }

        // Metoda wywolywana po nacisnieciu przycisku "Rysuj"
        // wizualizuje na ekranie wszystkie obiekty kolekcji z urzadzeniami
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < nodes.Count(); i++)
            {
                Node temp = nodes[i];
                if (!temp.getType())
                {
                    drawNode(temp.getLoc(), temp.getID());
                }
                else
                    drawBase(temp.getLoc(), temp.getID());
            }

        }


        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;

            currentX = e.X;
            currentY = e.Y;
        }

        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Top = this.Top + (e.Y - currentY);
                this.Left = this.Left + (e.X - currentX);
            }
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

    }
}
