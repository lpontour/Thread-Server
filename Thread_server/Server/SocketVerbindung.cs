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
            // Variable "erlaubt" lässt Dialog solange erscheinen, bis erlaubter Wert eingegeben
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

            // Starten der Suche nach Verbindungen mit TCPListener
            listenerServer = new TcpListener(ipAdresse, port);
            listenerServer.Start();
            Console.WriteLine("Verbindungen werden gesucht.");

            // Erzeugen der Semaphore
            Semaphore semaphore = new Semaphore(maxVerbindungen, maxVerbindungen);

            // Dauerhaftes warten auf Verbindungsanfragen des Clients
            while (true)
            {

                // Wenn eine Verbindungsanfrage besteht...
                if (listenerServer.Pending())
                {
                    // ...und die Semaphore noch nicht blockiert ist,...
                    //if (!semaLock)
                    //{                     
					try
					{
						Console.WriteLine("semalock");
						semaphore.WaitOne();
						// ...wird ein neuer Thread erstellt 
						try
						{
							Thread.Sleep(50);
							new Thread(() =>
							{
								XmlDocument xml = new XmlDocument();
								Thread.CurrentThread.IsBackground = true;

								// Annehmen der Socketverbindung
								tcpClient = listenerServer.AcceptTcpClient();
								Console.WriteLine("Verbindung entgegengenommen.");

								// Übergeben des XML-Schnippsels
								NetworkStream stream = tcpClient.GetStream();

								// Wenn lesbare Daten verfügbar sind...
								if (stream.DataAvailable)
								{
									/////////////////////////////////////////////////////////////////
									string str;
									//using (stream)
									{
										byte[] data = new byte[2048];
										using (MemoryStream ms = new MemoryStream())
										{

											int numBytesRead;
											while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
											{
												ms.Write(data, 0, numBytesRead);
											}

											str = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);											
										}
									}
									///////////////////////////////////////////////////////////////
									Console.WriteLine(str);
									xml.LoadXml(str);

									if (!xml.DocumentElement.HasChildNodes)
									{
										Console.WriteLine("Root empfangen");
									}

									formartierer.Formatieren(xml, 1);

									Console.WriteLine("semafreigaben");
									semaphore.Release();
								}
							}).Start();
						}
						catch (Exception exception)
						{
							throw new Exception("Fehler bei Verbindung", exception);
						}
					}
					finally
					{

					}
                    //}
                }
            }
        }
        #endregion
    }
}
