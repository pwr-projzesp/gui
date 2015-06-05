/*
 * Copyright (c) 2013, ADVANTIC Sistemas y Servicios S.L.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions 
 * are met:
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the 
 *   documentation and/or other materials provided with the distribution.
 * - Neither the name of ADVANTIC Sistemas y Servicios S.L. nor the names 
 *   of its contributors may be used to endorse or promote products derived
 *   from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY 
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
 * USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
/**
 * @author Eloy Díaz Álvarez <eldial@gmail.com>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using tinyos.sdk;

namespace graf
{
    class Listener
    {
        MoteIF mote;            // obiekt klasy MoteIF odpowiedzialnej za niskopoziomowe odbieranie danych z określonego portu
        string motecom;         // łańcuch znaków zawierający numer portu i szybkość transmisji w forie "serial@numerportu:szybkosctransmisji" np. "serial@com4:57600"
        Form1 form1;            // zmienna zawierająca formularz do którego zwracamy wszystkie odebrane dane
        Boolean active;         // zmienna zawierająca dane czy jest aktywny jakikolwiek nasłuch

        // Konstruktor przyjmujący jako parametry msg - zawierajaca numer portu i szybkosc transmisji,
        // oraz formularz do którego ma zwrócić odebrane dane
        public Listener(string msg, Form1 form)
        {
            form1 = form;

            motecom = msg;
            try
            {
                mote = new MoteIF(motecom);
            }
            catch (Exception e)
            {
                form1.add_Msg(e.Message);
                return;
            }
            active = true;
            form1.add_Msg("Listening on " + motecom + " (^C or 'exit' returns to prompt)");

            mote.onMessageArrived += newMsgHandler;
        }

        // Metoda zatrzymująca nasłuch na portach
        public void stop(/*Object s, ConsoleCancelEventArgs e*/)
        {
            if (active == true)
            {
                active = false;
                form1.add_Msg("Closing listener on " + motecom + "...");
                //mote.onMessageArrived -= newMsgHandler;
                mote.Close();
            }
        }

        // Metoda przekazująca do formularza dane odebrane od stacji bazowej
        private void newMsgHandler(Object sender, EventArgSerialMessage e)
        {
            String msg = BitConverter.ToString(e.msg.GetMessageBytes(), 0);
            String route = "";
            string newmsg = "";
            string[] msgSplit = msg.Split('-');
            for (int i = 8; i < msgSplit.Count(); i++)
            {
                int value = Convert.ToInt32(msgSplit[i], 16);
                if (value >= 65 && value <= 90 || value == 32 || value == 95 || value >= 97 && value <= 122 || value == 125 || value == 126)
                {
                    char charValue = (char)value;
                    newmsg += charValue;
                }
            }
            for (int i = 23; i < msg.Length; i++)
                route += msg[i];
            routing(route);
            form1.add_Msg(newmsg);
        }
        private void routing(String msg)
        {

            string[] msgSplit = msg.Split('-');
            int ID_MSG = Convert.ToInt32(msgSplit[0], 16);
            if (ID_MSG == 1)
            {
                int ID_ROUTE = Convert.ToInt32(msgSplit[1], 16);
                int ID_MOTE = Convert.ToInt32(msgSplit[2], 16);
                if (form1.routing_table.Exists(x => x.id == ID_ROUTE))
                {
                    Console.WriteLine("znalazł");
                    int index = form1.routing_table.FindIndex(a => a.id == ID_ROUTE);
                    if (ID_MOTE > 0)
                        form1.routing_table[index].motes.Add(ID_MOTE);
                }
                else
                    Console.WriteLine("nie znalazł");
                for (int i = 0; i < form1.routing_table.Count; i++)
                {
                    Console.Write("Trasa nr");
                    Console.WriteLine(form1.routing_table[i].id);
                    for (int j = 0; j < form1.routing_table[i].motes.Count; j++)
                        Console.WriteLine(form1.routing_table[i].motes[j]);

                }
            }
            else if (ID_MSG == 2)
            {
                int ID_MOTE = Convert.ToInt32(msgSplit[1], 16);
                int number = Convert.ToInt32(msgSplit[2], 16);
                //msgSplit[3] to kropka
                int fraction = Convert.ToInt32(msgSplit[4], 16);
                float voltage = number + fraction / 10;
                form1.nodes
            }
            Console.ReadLine();

        }
    }
}
