using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    internal class SocketVerbindung
    {
        #region fields
        // Semaphore erzeugen
        int maxVerbindungen = 2;
        private static Semaphore semaphore = new Semaphore(2, 2);
        // Konfiguration Server
        int port = 3;
        IPAddress ipAdresse = IPAddress.Any;
        // Verbindung
        string nachricht;
        byte[] buffer = new Byte[256];
        string data = null;
        #endregion


        #region ctor
        internal SocketVerbindung()
        {
            Thread wartenThread = new Thread(new ThreadStart(this.WartenAufVerbindung));
            wartenThread.Start();
        }
        #endregion


        #region methods
        internal void WartenAufVerbindung()
        {
            TcpListener listenerServer = new TcpListener(ipAdresse, port);

            try
            {
                listenerServer.Start();
                Socket socket = listenerServer.AcceptSocket();

            }
            catch (Exception exception)
            {
                throw new Exception("Fehler bei Verbindungserkennung", exception);
            }


        }

        internal void VerbindungSemaphore()
        {
            semaphore.WaitOne();



            semaphore.Release();
        }
        #endregion

    }
}
