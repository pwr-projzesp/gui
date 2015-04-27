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
        Listener listener;
        delegate void SetTextCallback(string text);

        public Form2()
        {
            InitializeComponent();     
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

        public void add_Msg(String msg)
        {
            SetText(msg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string port = textBox2.Text;
            string speed = textBox3.Text;
            string msg = "serial@" + port.ToLower() + ":" + speed;
            listener = new Listener(msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listener.stop();
        }

    }
}
