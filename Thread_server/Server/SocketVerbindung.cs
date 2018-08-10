using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    internal class SocketVerbindung
    {
        #region fields
        private int maxVerbindungen;
        private int port;
        private IPAddress ipAdresse;
        private TcpClient tcpClient;
        private IFormatierer formartierer = new FactoryFormate();
        TcpListener listenerServer;
        bool semaLock = false;


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

            listenerServer = new TcpListener(ipAdresse, port);

            listenerServer.Start();
            Console.WriteLine("Starten");

            Console.WriteLine("Verbindungen werden gesucht.");

            Console.WriteLine("locken");
            // Erzeugen der Semaphore...
            Semaphore semaphore = new Semaphore(maxVerbindungen, maxVerbindungen);
            Console.WriteLine("erstellen");

            // Dauerhaftes warten auf Verbindungsanfragen des Clients
            while (true)
            {
                // Initialisieren und starten des TCPListeners

                // Variable zum Blockieren des kritischen Codes

                // ...wenn eine Verbindungsanfrage besteht...
                if (listenerServer.Pending())
                {
                    //Console.WriteLine("pending");
                    // ...und die Semaphore noch nicht blockiert wird,...
                    if (!semaLock)
                    {
                        Console.WriteLine("semalock");
                        try
                        {
                            Console.WriteLine("try");
                            // ...wird ein neuer Thread erstellt 
                            new Thread(() =>
                            {
								XmlDocument xml = new XmlDocument();
                            
								Console.WriteLine("neuer thread neu");
                                Thread.CurrentThread.IsBackground = true;
								// Blockieren ist abhängig vom Status der Semaphore + WaitOne dekrementiert die Semaphore
								Thread.Sleep(70);
								semaLock = semaphore.WaitOne();
								Thread.Sleep(70);
								// Annehmen der Socketverbindung
								tcpClient = listenerServer.AcceptTcpClient();
                                Console.WriteLine("Verbindung entgegengenommen.");

                                // Übergeben des NetworkStreams
                                NetworkStream stream = tcpClient.GetStream();
								if(stream.DataAvailable)
								{
									xml.LoadXml(new StreamReader(stream).ReadToEnd());//XmlReader.Create(stream));
                                    formartierer.Formatieren(xml, 1);

									// Release inkrementiert die Semaphore
									Thread.Sleep(70);
									semaphore.Release();
									Thread.Sleep(70);
								}
                            }).Start();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception("Fehler bei Verbindung", exception);
                        }
                    }
                }
            }
        }
        #endregion

    }
}
