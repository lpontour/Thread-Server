using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
    class Filewriter : IFilewriter
    {
        #region fields
        //Variablen
        protected XmlDocument _xml;
        protected string _clientName;
        #endregion

        #region ctor
        //Leerer Standart Constructor
        public Filewriter()
        {
        }
        #endregion

        #region methods
        //Speichert die XML in den Ordner des Servers
        //Überwacht zusätzlich den status der xml, ob diese Abgeschlossen sind oder nicht
        public void DateiSchreiben(XmlDocument xmlDocument)
        {
            if (xmlDocument != null)
            {
                _xml = xmlDocument;
                _clientName = _xml.DocumentElement.Attributes[0].Value;
                if(!CheckForRoot(_xml))
                {
                    Console.WriteLine("Ordner Angekommen:" + _xml.DocumentElement.FirstChild.Attributes[0].Value);
                }
                if (File.Exists(_clientName + "_NC.xml"))
                {
                    if (CheckForRoot(_xml) != true)
                    {
                        Console.WriteLine("Füge neue informationen hinzu");
                        AppendXml(_xml);
                    }else if(CheckForRoot(_xml))
                    {
                        if(File.Exists(_clientName+".xml"))
                        {
                            Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml", 2));
                            savinThread.Start();
                        }
                        else
                        {
                            Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml", 3));
                            savinThread.Start();
                        }
                    }
                }
                else if(File.Exists(_clientName + "_NC.xml")==false)
                {
                    if (!CheckForRoot(_xml))
                        {
                        Console.WriteLine("Speichere nicht fertige xml erstmalig");
                        Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml", 1));
                        savinThread.Start();
                    }
                    else
                    {
                        Console.WriteLine("Achtung das root element ist als erstes angekommen, datei wird leer fertig gespeichert");
                        Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + ".xml", 1));
                        savinThread.Start();
                    }
                }
            }
            else
            {
                Console.WriteLine("Fehler:XmlDocument entspricht null");
            }
        }

        //Überprüft ob bei den gegebenen Dokument die Rootwurzel Leer ist oder nicht
        private bool CheckForRoot(XmlDocument xml)
        {
            if (xml.DocumentElement.HasChildNodes)
            { return false; }
            else
            { return true; }

         
        }


        //Fügt die gegebene XML an eine Vorhandene XML eines Clienten und Speichert diese
        private void AppendXml(XmlDocument xml1)
        {
            XmlDocument oldXml = new XmlDocument();
            oldXml.Load (_clientName + "_NC.xml");
            XmlNode recievedXmlDocNode = xml1.DocumentElement.FirstChild;
			XmlNode oldXmlDocNode;
            bool dopplung = false;
			oldXmlDocNode = oldXml.DocumentElement.FirstChild;
            XmlNode xmlImport = oldXml.ImportNode(recievedXmlDocNode, true);
            

            foreach (XmlNode node in oldXml.DocumentElement)
            {
                if (node.Attributes[0].Value == recievedXmlDocNode.Attributes[0].Value)
                {
                    oldXml.DocumentElement.ReplaceChild(xmlImport, node);
                    dopplung = true;
                }
            }
            if (!dopplung)
            {
                oldXml.DocumentElement.AppendChild(xmlImport);
            }

            Thread savinThread = new Thread(() => SaveXml(oldXml, _clientName + "_NC.xml", 1));
                savinThread.Start();
            
        }

        private void SaveXml(XmlDocument xml1,string dateiName,int mode)
        {
         
         if((mode == 2) || (mode == 3)) { Thread.Sleep(500); }

           Monitor.Enter(dateiName);
                
                try
                {
                    switch (mode)
                    {
                        case 1:
                            Console.WriteLine("Speichere");
                            xml1.Save(dateiName);
                            break;
                        case 2:
                            Console.WriteLine("Lösche alte fertige xml");
                            Console.WriteLine("Speichere fertige xml");
                            File.Delete(_clientName + ".xml");
                            File.Move(_clientName + "_NC.xml", _clientName + ".xml");
                            break;
                        case 3:
                            Console.WriteLine("Speichere fertige xml");
                            File.Move(_clientName + "_NC.xml", _clientName + ".xml");

                            break;
                        default:
                            break;
                    }

                }
                catch (System.IO.IOException e)
                {
                   
                    Console.WriteLine(e);
                }
                finally
                {
                    Monitor.Exit(dateiName);
                }
                        
                }
             
            
        }
        #endregion
    }

