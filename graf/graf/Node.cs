using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graf
{
    class Node
    {
        private bool isBase = false;                        // zmienna przechowujaca informacje o typie urzadzenia (true - stacja bazowa, false - stacja robocza)
        private int ID;                                     // zmienna przechowujaca ID urzadzenia
        private Point loc = new Point();                    // zmienna przechowujaca polozenie urzadzenia
        private List<int> connDev = new List<int>();        // kolekcja przechowujaca ID urzadzen, z ktorymi dane urzadzenie ma polaczenie     
        private bool isconnected = false;                     // zmienna przechowująca informację o tym, czy dane urządzenie ma połączenie ze stacją bazową


// Constructors
        public Node(int ID, Point loc)
        {
            this.ID = ID;
            this.loc = loc;
        }
        public Node(int ID, Point loc, int[] connDev)
        {
            this.ID = ID;
            this.loc = loc;
            setConnDev(connDev);
        }
        public Node(int ID, Point loc, bool isBase)
        {
            this.ID = ID;
            this.loc = loc;
            this.isBase = isBase;
            this.isconnected = isBase;
        }
        public Node(int ID, Point loc, bool isBase, int[] connDev)
        {
            this.ID = ID;
            this.loc = loc;
            this.isBase = isBase;
            setConnDev(connDev);
        }

// Getters
        public bool         getType()
        {
            return isBase;
        }
        public int          getID()
        {
            return ID;
        }
        public Point        getLoc()
        {
            return loc;
        }
        public List<int>    getConnDev()
        {
            return connDev;
        }
        public bool getisconnected()
        {
            return isconnected;
        }

// Setters
        public void setType(bool isBase)
        {
            this.isBase = isBase;
        }
        public void setID(int ID)
        {
            this.ID = ID;
        }
        public void setLoc(Point loc)
        {
            this.loc = loc;
        }
        public void setConnDev(int[] connDev)
        {
            for (int i = 0; i < connDev.Length; i++)
                this.connDev.Add(connDev[i]);
        }
        public void setisconnected(bool con)
        {
            this.isconnected = con;
        }

    }
}
