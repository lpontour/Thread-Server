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
        private int maxVerbindungen;
        private int port;
        private IPAddress ipAdresse;
        private TcpClient tcpClient;
        private IFormatierer formartierer;
        #endregion

        #region ctor
        internal SocketVerbindung()
        {
            // Ermitteln der maximalen Verbindungen durch Konsoleneingabe
            bool erlaubt = false;
            while(erlaubt == false)
            {
                Console.WriteLine("Wie viele maximale Verbindungen sind erlaubt? \nMit Enter wird der Standardwert von 2 Verbindungen übernommen.");
                string eingabe = Console.ReadLine();
                if (eingabe == "")
                {
                    maxVerbindungen = 2;
                    erlaubt = true;
                }
                else
                {
                    erlaubt = int.TryParse(eingabe, out maxVerbindungen);
                }
            }
            
            // Ermitteln der IP-Adresse durch Konsoleneingabe
            erlaubt = false;
            while (erlaubt == false)
            {
                Console.WriteLine("Welche IP-Adresse wird genutzt?");
                string eingabe = Console.ReadLine();
                erlaubt = IPAddress.TryParse(eingabe, out ipAdresse);
            }

            // Ermitteln des Ports durch Konsoleneingabe
            erlaubt = false;
            while (erlaubt == false)
            {
                Console.WriteLine("Welcher Port soll genutzt werden?");
                string eingabe = Console.ReadLine();
                erlaubt = int.TryParse(eingabe, out port);
            }
            
            Console.ReadKey();

            // Dauerhaftes warten auf Verbindungsanfragen des Clients
            while(true)
            { 
                WartenAufVerbindung();
            }
        }
        #endregion


        #region methods
        private void WartenAufVerbindung()
        {
            // Initialisieren und starten des TCPListeners
            TcpListener listenerServer;
            listenerServer = new TcpListener(ipAdresse, port);
            listenerServer.Start();

            // Variable zum Blockieren des kritischen Codes
            bool semaLock = false;

            // Erzeugen der Semaphore...
            Semaphore semaphore = new Semaphore(maxVerbindungen, maxVerbindungen);

            // ...wenn eine Verbindungsanfrage besteht...
            if (listenerServer.Pending())
            {
                // ...und die Semaphore noch nicht blockiert wird,...
                if (!semaLock)
                {
                    try
                    {
                        // ...wird ein neuer Thread erstellt 
                        new Thread (() =>
                        {
                            Thread.CurrentThread.IsBackground = true;
                            // Blockieren ist abhängig vom Status der Semaphore + WaitOne dekrementiert die Semaphore
                            semaLock = semaphore.WaitOne();
                            // Annehmen der Socketverbindung
                            tcpClient = listenerServer.AcceptTcpClient();

                            // Übergeben des NetworkStreams
                            NetworkStream stream = tcpClient.GetStream();
                            formartierer.Formatieren(stream,1);

                            // Release inkrementiert die Semaphore
                            semaphore.Release();
                        });
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Fehler bei Verbindung", exception);
                    }
                }
            }

        }
        #endregion
    }
}
