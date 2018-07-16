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
                            File.Delete(_clientName + ".xml");
                            File.Move(_clientName + "_NC.xml", _clientName + ".xml");
                        }
                        else
                        {                     
                            File.Move(_clientName + "_NC.xml", _clientName + ".xml");
                        }
                    }
                }
                else
                {                    
                    Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml"));
                }
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
            XmlDocument oldXml=new XmlDocument();
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
                    XmlNode xmlImport = oldXml.ImportNode(recievedXmlDocNode.FirstChild, true);
                    oldXmlDocNode.ReplaceChild(xmlImport, oldXmlDocNode);
                    notFound = false;
                    break;
                }
            } while (notFound);
            Thread savinThread = new Thread(() => SaveXml(oldXml, _clientName + "_NC.xml"));
            
       
        }

        private void SaveXml(XmlDocument xml1,string dateiName)
        {
            
                Monitor.Enter(dateiName);
                {
                    try
                    {
                         xml1.Save(dateiName);
                    }
                    finally
                    {
                        Monitor.Exit(dateiName);
                    }
                }
            
        }
        #endregion
    }
}
