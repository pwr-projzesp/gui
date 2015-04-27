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
        MoteIF mote;
        string motecom;
        Form2 form2;
        Boolean active;

        public Listener(string msg, Form2 form)
        {
            form2 = form;

            motecom = msg;
            try
            {
                mote = new MoteIF(motecom);
            }
            catch (Exception e)
            {
                form2.add_Msg(e.Message);
                return;
            }
            active = true;
            form2.add_Msg("Listening on " + motecom + " (^C or 'exit' returns to prompt)");
            mote.onMessageArrived += newMsgHandler;
        }

        public void stop(/*Object s, ConsoleCancelEventArgs e*/)
        {
            active = false;
            form2.add_Msg("Closing listener on " + motecom + "...");
            mote.onMessageArrived -= newMsgHandler;
            mote.Close();
        }

        private void newMsgHandler(Object sender, EventArgSerialMessage e)
        {
            form2.add_Msg(BitConverter.ToString(e.msg.GetMessageBytes(), 0));
        }

    }
}
