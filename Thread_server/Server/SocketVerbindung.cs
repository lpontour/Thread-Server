using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class SocketVerbindung
    {
        #region fields
        int anzVerbindungen = 0;
        int maxVerbindungen = 2;
        int port = 3;
        IPAddress ipAdresse;
        string nachricht;
        byte[] buffer = new Byte[256];
        string data = null;
        #endregion


        #region ctor


        #endregion


        #region methods
        void VerbindungAnnehmen()
        {
            TcpListener server = new TcpListener(ipAdresse, port);
            Socket socket = server.AcceptSocket();

            server.Start();


            if (anzVerbindungen <=2)
            {
                nachricht = "Verbindung wird aufgebaut";
            }
            else
            {
                nachricht = "Maximale Anzahl Verbindungen schon erreicht. Versuche es später erneut.";
            }
        }
        #endregion

    }
}
