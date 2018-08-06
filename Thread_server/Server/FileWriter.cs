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
                if (File.Exists(_clientName + "_NC.xml"))
                {
                    if (CheckForRoot(_xml) != true)
                    {
                        AppendXml(_xml);
                    }else
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
                else
                {
                    Console.WriteLine("Füge neue informationen hinzu");
                    Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml",1));
                    savinThread.Start();
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
            if (xml.DocumentElement.HasChildNodes) { return false; }

            return true;
        }


        //Fügt die gegebene XML an eine Vorhandene XML eines Clienten und Speichert diese
        private void AppendXml(XmlDocument xml1)
        {
            XmlDocument oldXml = new XmlDocument();
            oldXml.Load (_clientName + "_NC.xml");
            XmlNode recievedXmlDocNode = _xml.DocumentElement.FirstChild;
			XmlNode oldXmlDocNode;

			oldXmlDocNode = oldXml.DocumentElement.FirstChild;

			bool notFound = false;
      
            do
            {
                if ((oldXmlDocNode.Attributes[0].Value==recievedXmlDocNode.Attributes[0].Value))
                {
                    if (oldXmlDocNode.FirstChild == null)
                    {
                        XmlNode xmlImport = oldXml.ImportNode(recievedXmlDocNode, true);
                        oldXmlDocNode.ParentNode.ReplaceChild(xmlImport, oldXmlDocNode);
                        notFound = false;
                        break;
                    }
                    else
                    {
                        oldXmlDocNode = oldXmlDocNode.FirstChild;
                        recievedXmlDocNode = recievedXmlDocNode.FirstChild;
                        notFound = true;
                    }
                }
                else if(oldXmlDocNode.NextSibling!=null)
                {
                    oldXmlDocNode = oldXmlDocNode.NextSibling;
                   
                    notFound = true;
                }
                else 
                {
                    if (recievedXmlDocNode.FirstChild != null)
                    {
                        XmlNode xmlImport = oldXml.ImportNode(recievedXmlDocNode.FirstChild, true);
                        oldXmlDocNode.ReplaceChild(xmlImport, oldXmlDocNode);
                        notFound = false;
                        break;
                    }

                }
            } while (notFound);
            Thread savinThread = new Thread(() => SaveXml(oldXml, _clientName + "_NC.xml",1));
            savinThread.Start();
       
        }

        private void SaveXml(XmlDocument xml1,string dateiName,int mode)
        {
            bool done = false;
  
            do
            {
                if (Monitor.TryEnter(dateiName))
                {

                    {
                        try
                        {
                            switch (mode)
                            {
                                case 1:
                                    Console.WriteLine("Speichere");
                                    xml1.Save(dateiName);
                                    break;
                                case 2 :
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
                        catch(System.IO.IOException e)
                        {
                            done = false;
                            Console.WriteLine("Fehler: war in gesicherten bereich , versuche nochmal");
                        }
                        finally
                        {
                            done = true;
                            Monitor.Exit(dateiName);
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Speichern nicht möglich da schon jemand drinne ist versuche nochmal");
                    done = false;
                }
            } while(!done);
        }
        #endregion
    }
}
