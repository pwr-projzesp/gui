using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private List<int> connBase = new List<int>();       // kolekcja przechowujaca ID urzadzen, przez ktore stacja bazowa wysyla kolejno pakiet do danej stacji docelowej
        private static int range;                           // zmienna statyczna przechowujaca aktualny zasieg urzadzen
        private float voltage;

// Constructors
        public Node(int ID, Point loc)
        {
            this.ID = ID;
            this.loc = loc;
            this.voltage = (float)(0.1);
        }
        public Node(int ID, Point loc, List<int> connDev)
        {
            this.ID = ID;
            this.loc = loc;
            setConnDev(connDev);
            this.voltage = (float)(0.1);
        }
        public Node(int ID, Point loc, bool isBase)
        {
            this.ID = ID;
            this.loc = loc;
            this.isBase = isBase;
            this.voltage = (float)(0.1);
            if (ID == 5)
            {
                connBase.Add(1);
                connBase.Add(3);
                connBase.Add(5);
            }
        }
        public Node(int ID, Point loc, bool isBase, List<int> connDev)
        {
            this.ID = ID;
            this.loc = loc;
            this.isBase = isBase;
            setConnDev(connDev);
            this.voltage = (float)(0.1);
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
        public static int   getRange()
        {
            return range;
        }
        public Point        getLoc()
        {
            return loc;
        }
        public List<int>    getConnDev()
        {
            return connDev;
        }
        public List<int>    getConnBase()
        {
            return connBase;
        }
        public float        getVoltage()
        {
            return voltage;
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
        public static void setRange(int range1)
        {
            range = range1;
        }
        public void setLoc(Point loc)
        {
            this.loc = loc;
        }
        public void setConnDev(List<int> connDev)
        {
            for (int i = 0; i < connDev.Count(); i++)
                this.connDev.Add(connDev[i]);
        }
        public void clearConnDev()
        {
            this.connDev.Clear();
        }
        public void setConnBase(List<int> connBase)
        {
            for (int i = 0; i < connBase.Count(); i++)
                this.connBase.Add(connBase[i]);
        }
        public void clearConnBase()
        {
            this.connBase.Clear();
        }
        public void setVoltage(float voltage)
        {
            this.voltage = voltage;
        }

    }
}
