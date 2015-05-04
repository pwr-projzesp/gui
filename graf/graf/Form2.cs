using System;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Diagnostics;
using tinyos.sdk;
using System.Windows.Forms;

namespace graf
{
    public partial class Form2 : Form
    {
        Listener listener;                              // nowy obiekt klasy Listener

        public Form2()
        {
            InitializeComponent();     
        }

        // Metody potrzebne do prawidłowego wyświetlania wiadomości otrzymanych w wyniku działania innego wątku
        delegate void SetTextCallback(string text);
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
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

        // Metoda dodaje tekst do tekstboxa
        public void add_Msg(String msg)
        {
            string newmsg = "";
            string[] msgSplit = msg.Split('-');
            for (int i = 8; i < msgSplit.Count(); i++)
            {
                int value = Convert.ToInt32(msgSplit[i], 16);
                if (value >= 65 && value <= 90 || value == 32 || value == 95 || value >= 97 && value <= 122)
                {
                    char charValue = (char)value;
                    newmsg += charValue;
                }
            }
            SetText(newmsg);
        }

// Przyciski 

        // Metoda wywoływana po naciśnieciu przycisku start
        // odczytuje dane z textboxów i tworzy nowy obiekt klasy Listener
        private void button1_Click(object sender, EventArgs e)
        {
            string port = textBox2.Text;
            string speed = textBox3.Text;
            string msg = "serial@" + port.ToLower() + ":" + speed;
            listener = new Listener(msg, this);
        }

        // Metoda wywoływana po naciśnieciu przycisku stop
        // zatrzymuje nasłuch na wszystich portach
        private void button2_Click(object sender, EventArgs e)
        {
            if (listener != null)
                listener.stop();
        }

        // Metoda wywoływana po naciśnieciu przycisku exit
        // w bezpieczny sposób wyłącza formularz 2 i zatrzymuje nasłuch na portach nie powodując wyrzucania wyjątków
        private void button3_Click(object sender, EventArgs e)
        {
            if (listener != null)
                listener.stop();
            this.Close();
        }

    }
}
