using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tinyos.sdk;

namespace graf
{
    public partial class Form1 : Form
    {
        int range = 0;                                          // zmienna przechowujaca wartosc zasiegu
        Point loc = new Point();                                // zmienna sluzaca do przechowywania polozenia ( (0;0) - gorny prawy rog okna aplikacji)
        List<Node> nodes = new List<Node>();                    // kolekcja zawierajace wszystkie urzadzenia
        private PictureBox picture;                             // pojedynczy picturebox reprezentujacy fizyczny obiekt
        private List<PictureBox> pictureBoxes = new List<PictureBox>();     // kolekcja przechowujaca pictureboxy z wezlami
        Listener listener;                              // nowy obiekt klasy Listener


// Funckje rysujące
// --------------------------------------------------------------------------------------------------

        private string info;
        private int xPos, yPos;                                 // zmienne sluzace do przechowywania aktualnego polozenia

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji bazowej
        private void drawBase(Point loc, int ID)
        {
            String nazwa = "    Base";
            //nazwa += ID;
            picture = new PictureBox();
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
                Point loca = nodes[ID].getLoc();
                List<int> nb = nodes[ID].getConnDev();
                string nbhood = string.Join(", ", nb);

                info = "ID: " + ID + " \nPolozenie: " + pictureBoxes[ID].Location.X + " " + pictureBoxes[ID].Location.Y + " \nSasiedzi: " + nbhood;
                MessageBox.Show(info, nazwa);
            });

            picture.MouseDown += new MouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    xPos = e.X;
                    yPos = e.Y;
                }
                if (e.Button == MouseButtons.Right)
                {
                    groupBox4.Invalidate();
                }
            });
            picture.MouseMove += new MouseEventHandler(picture_MouseMove);

            picture.MouseUp += new MouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    copyLocation();
                    connections();
                }
                if (e.Button == MouseButtons.Right)
                {
                    drawRange(ID);
                    drawConn(ID);
                }
            });

            picture.Cursor = Cursors.Hand;
            groupBox4.Controls.Add(picture);
            pictureBoxes.Add(picture);
        }
        
        // Metoda sluzaca do narysowania na ekranie pojedynczej polaczenia miedzy urzadzeniami
        private void drawLine(Point startloc, Point endloc, char color)
        {
            Image baseStation = Image.FromFile("base.png");         // grafika stacji bazowej
            Image nodeStation = Image.FromFile("node.png");         // grafika stacji roboczej

            startloc.X = startloc.X + baseStation.Width / 2;
            startloc.Y = startloc.Y + baseStation.Height / 2;

            endloc.X = endloc.X + nodeStation.Width / 2;
            endloc.Y = endloc.Y + nodeStation.Height / 2;

            if (color == 'b')
                groupBox4.CreateGraphics().DrawLine(new Pen(Brushes.Black, 3), startloc, endloc);
            else if (color == 'g')
                groupBox4.CreateGraphics().DrawLine(new Pen(Brushes.Green, 3), startloc, endloc);
            else if (color == 'y')
                groupBox4.CreateGraphics().DrawLine(new Pen(Brushes.Yellow, 3), startloc, endloc);
            else if (color == 'r')
                groupBox4.CreateGraphics().DrawLine(new Pen(Brushes.Red, 3), startloc, endloc);
        }

        // Metoda sluzaca do narysowania na ekranie pojedynczej stacji roboczej
        private void drawNode(Point loc, int ID)
        {
            String nazwa = "    Node";
            nazwa += ID+1;
            picture = new PictureBox();
            picture.Location = new System.Drawing.Point(loc.X, loc.Y);
            picture.Image = new Bitmap("node.png");
            picture.Size = picture.Image.Size;
            float volt = nodes[ID].getVoltage();
            String voltage = volt + "V \n";
            picture.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.DrawString(nazwa, Font, Brushes.Black, 0, 0);
                groupBox4.CreateGraphics().DrawString(voltage, Font, Brushes.Green, loc.X + picture.Width + 2, loc.Y);
            });

            picture.MouseDoubleClick += new MouseEventHandler((sender, e) => 
            {
                Point loca = nodes[ID].getLoc();
                List<int> nb = nodes[ID].getConnDev();
                string nbhood = string.Join(", ", nb);

                info = "ID: " + ID + " \nPolozenie: " + pictureBoxes[ID].Location.X + " " + pictureBoxes[ID].Location.Y + " \nSasiedzi: " + nbhood;
                MessageBox.Show(info, nazwa);           
            });

            picture.MouseDown += new MouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    xPos = e.X;
                    yPos = e.Y;
                }
                if (e.Button == MouseButtons.Right)
                {
                    groupBox4.Invalidate();
                }         
            });
            picture.MouseMove += new MouseEventHandler(picture_MouseMove);
           
            picture.MouseUp += new MouseEventHandler((sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    copyLocation();
                    connections();
                }
                if (e.Button == MouseButtons.Right)
                {
                    drawRange(ID);
                    drawConn(ID);
                }
            });
            
            picture.Cursor = Cursors.Hand;
            groupBox4.Controls.Add(picture);
            pictureBoxes.Add(picture);

        }

        // Metoda sluzaca do narysowania wszystkich dostepnych polaczen
        private void drawConn()
        {
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

        // Metoda sluzaca do narysowania dostepnych dla danego urzadzenia polaczen
        private void drawConn(int ID)
        {
            Point begin = nodes[ID].getLoc();
            List<int> neighbours = nodes[ID].getConnDev();

            for (int j = 0; j < neighbours.Count; j++)
            {
                Point end = nodes[neighbours[j]].getLoc();
                drawLine(begin, end, 'y');
            }
        }
        
        // Metoda sluzaca do narysowania zasiegu urzadzenia o pdanym ID
        private void drawRange(int ID)
        {
            groupBox4.CreateGraphics().DrawEllipse(new Pen(Brushes.Green, 2), pictureBoxes[ID].Location.X - Node.getRange(), pictureBoxes[ID].Location.Y - Node.getRange(), 2 * Node.getRange(), 2 * Node.getRange());
        }



// Przyciski
// --------------------------------------------------------------------------------------------------

        int index = 0;

        // Metoda wywolywana po nacisnieciu przycisku "Dodaj urzadzenie"
        // dodaje nowe urzadzenie o podanych parametrach do kolekcji urzadzen
        private void button1_Click(object sender, EventArgs e)
        {
            loc = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            Node station = new Node(nodes.Count(), loc, checkType.Checked);
            nodes.Add(station);
        }

        // Metoda wywolywana po nacisnieciu przycisku "Rysuj"
        // wizualizuje na ekranie wszystkie obiekty kolekcji z urzadzeniami
        private void button2_Click(object sender, EventArgs e)
        {
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

        // Metoda wywolywana po nacisnieciu przycisku "Sprawdz polaczenia"
        // wizualizuje dostepne polaczenia pomiedzy poszczegolnymi wezlami
        private void button3_Click(object sender, EventArgs e)
        {
            drawConn();
        }

        // Metoda wywolywana po nacisnieciu przycisku "Wyczysc okno"
        // czysci wizualizacje zasiegu i polaczen widoczne w oknie aplikacji
        private void button4_Click(object sender, EventArgs e)
        {
            groupBox4.Invalidate();
        }

        // Metoda wywolywana po nacisnieciu przycisku "Wczytaj topologie z pliku"
        // pobiera dane z pliku i na ich podstawie tworzy nowa topologie sieci
        private void button5_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.Title = "Wybierz plik z topologią.";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .txt file was selected, open it.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String path = openFileDialog1.FileName;
                System.IO.StreamReader file = new System.IO.StreamReader(@path);
                try
                {
                    String line;
                    int i = 0;
                    Point location = new Point();
                    Boolean isItBase;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (i == 0)
                        {
                            range = Int32.Parse(line);
                            numericUpDown3.Value = range;
                            i++;
                        }
                        else
                        {
                            String[] words = line.Split(' ');
                            location.X = Int32.Parse(words[0]);
                            location.Y = Int32.Parse(words[1]);
                            isItBase = Convert.ToBoolean(words[2]);
                            Node station = new Node(nodes.Count(), location, isItBase);
                            nodes.Add(station);
                        }
                    }

                    file.Close();
                }
                catch (IOException)
                {
                }
            }
        }

        // Metoda wywolywana po nacisnieciu przycisku "Zapisz topologie do pliku"
        // zapisuje aktualnie ustawiona topologie do wskazanego pliku
        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            openFileDialog1.Title = "Sciezka do zapisania topologii.";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String path = openFileDialog1.FileName;
                System.IO.StreamWriter file = new System.IO.StreamWriter(@path, false);
                try
                {
                    file.WriteLine(range);
                    for (int i = 0; i < nodes.Count(); i++)
                    {
                        Point location = nodes[i].getLoc();
                        string type;
                        if (nodes[i].getType())
                            type = "true";
                        else
                            type = "false";

                        file.WriteLine(location.X + " " + location.Y + " " + type);
                    }
                    file.Close();
                }
                catch (IOException)
                {
                }
            }
        }

        // Metoda wywolywana po nacisnieciu przycisku "Start"
        // rozpoczyna nasłuch na danym porcie szeregowym
        private void button7_Click_1(object sender, EventArgs e)
        {
            string port = textBox2.Text;
            string speed = textBox3.Text;
            string msg = "serial@" + port.ToLower() + ":" + speed;
            listener = new Listener(msg, this);
        }

        // Metoda wywolywana po nacisnieciu przycisku "Stop"
        // kończy nasłuch na danym porcie szeregowym
        private void button8_Click(object sender, EventArgs e)
        {
            if (listener != null)
                listener.stop();
        }

        // Metoda wywolywana po nacisnieciu przycisku "EXIT"
        // zamyka główne okno programu
        private void button9_Click(object sender, EventArgs e)
        {
            if (listener != null)
                listener.stop();
            this.Close();
        }

// Obługa zdarzeń
// --------------------------------------------------------------------------------------------------

        // Metoda wykorzystywana podczas przeciagania obiektow w oknie aplikacji
        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox p = sender as PictureBox;

                if (p != null)
                {
                    //if (e.Button == MouseButtons.Left)
                   // {
                        p.Top += (e.Y - yPos);
                        p.Left += (e.X - xPos);
                        groupBox4.Invalidate();
                    //}
                }
            }

        }


// Funckja główna
// --------------------------------------------------------------------------------------------------

        public Form1()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }



// Funckje pomocnicze
// --------------------------------------------------------------------------------------------------

        // Metoda sluzaca do obliczania odleglosci pomiedzy danymi punktami
        private int getLenght(Point start, Point end)
        {
            double lenght;
            lenght = Math.Sqrt(Math.Pow((end.Y - start.Y), 2) + Math.Pow((end.X - start.X), 2));
            return (int)lenght;
        }

        // Metoda sluzaca do synchronizacji lokalizacji wezlow i pictureboxow
        private void copyLocation()
        {
            for (int i = 0; i < nodes.Count(); i++)
            {
                nodes[i].setLoc(pictureBoxes[i].Location);
            }
        }

        // Metoda sluzaca do atomatycznej aktualizacji wartosci zasiegu
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            groupBox4.Invalidate();
            range = (int)numericUpDown3.Value;
            Node.setRange(range);
            connections();
        }

        // Metoda sluzaca do obliczania sasiadow wszystkich aktywnych urzadzen
        private void connections()
        {
            for (int i = 0; i < nodes.Count(); i++)
            {
                nodes[i].clearConnDev();
            }

            for (int i = 0; i < nodes.Count(); i++)
            {
                List<int> neighbours = new List<int>();
                for (int j = 0; j < nodes.Count(); j++)
                {
                    if (nodes[i].getID() != nodes[j].getID())
                    {
                        int len = getLenght(nodes[i].getLoc(), nodes[j].getLoc());
                        if (len < Node.getRange())
                        {
                            neighbours.Add(nodes[j].getID());
                        }
                    }
                }
                nodes[i].setConnDev(neighbours);
            }
        }

        // Metody potrzebne do prawidłowego wyświetlania wiadomości otrzymanych w wyniku działania innego wątku
        delegate void SetTextCallback(string text);
        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle| CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        /*
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text + "\r \n");
            }
        }
        */
        // Metoda dodaje tekst do tekstboxa
        public void add_Msg(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(add_Msg);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.AppendText(text + "\r \n");
            }
        }

     }
}
