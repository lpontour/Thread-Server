using System;
using System.Collections.Concurrent;
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
        private static ConcurrentDictionary<string, object> _keyDict = new ConcurrentDictionary<string, object>();
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


                        Thread appendingThread = new Thread(() => AppendXml(_xml, 1));
                        appendingThread.Start();
                    }
                    else if (CheckForRoot(_xml))
                    {
                        if (File.Exists(_clientName + ".xml"))
                        {

                            Thread savinThread = new Thread(() => AppendXml(_xml, 2));
                            savinThread.Start();
                        }
                        else
                        {

                            Thread savinThread = new Thread(() => AppendXml(_xml, 3));
                            savinThread.Start();
                        }
                    }
                }
                else if (File.Exists(_clientName + "_NC.xml") == false)
                {
                    if (!CheckForRoot(_xml))
                    {
                        string dateiname = _clientName + "_1NC.xml";
                        object thisThreadSyncObject = new object();
                        thisThreadSyncObject = _keyDict.GetOrAdd(dateiname, thisThreadSyncObject);
                        if (Monitor.TryEnter(thisThreadSyncObject))
                            try
                            {
                                Console.WriteLine("versuche:Speichere nicht fertige xml erstmalig");
                                Thread savinThread = new Thread(() => SaveXml(_xml, _clientName + "_NC.xml", 1));
                                savinThread.Start();
                            }
                            finally
                            {
                                Monitor.Exit(thisThreadSyncObject);
                            }
                        else
                        {
                            Thread.Sleep(30);
                            DateiSchreiben(xmlDocument);
                        }
                    }
                    else
                    {

                        Console.WriteLine("Achtung das root element ist als erstes angekommen");
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
            if ((!xml.DocumentElement.HasChildNodes) || (xml.DocumentElement.FirstChild.Name == "datei"))
            { return true; }
            else
            { return false; }
        }


        //Fügt die gegebene XML an eine Vorhandene XML eines Clienten und Speichert diese
        private void AppendXml(XmlDocument xml1, int mode)
        {

            XmlDocument oldXml = new XmlDocument();
            string dateiName = _clientName + "_NC.xml";
            object thisThreadSyncObject = new object();
            bool thisTrhreadSyncLockTaken = false;
            Monitor.Enter(thisThreadSyncObject, ref thisTrhreadSyncLockTaken);
            try
            {

                for (; ; )
                {
                    object runningThreadSyncObject = _keyDict.GetOrAdd(dateiName, thisThreadSyncObject);
                    if (runningThreadSyncObject == thisThreadSyncObject) { break; }
                    Thread.Sleep(20);
                }

                oldXml.Load(_clientName + "_NC.xml");

            }
            finally
            {
                // Remove the key from the lock dictionary
                object dummy;
                _keyDict.TryRemove(dateiName, out dummy);
                if (thisTrhreadSyncLockTaken)
                {
                    Monitor.Exit(thisThreadSyncObject);
                }
            }



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
                    Console.WriteLine("Achtung doppelte Information");
                }
            }
            if (!dopplung)
            {
                if (mode == 2 || mode == 3)
                {
                    foreach (XmlNode node in xml1.DocumentElement)
                    {
                        xmlImport = oldXml.ImportNode(node, true);
                        Console.WriteLine("Füge neue informationen(datei) bei root hinzu");
                        oldXml.DocumentElement.AppendChild(xmlImport);
                    }
                }
                else
                {
                    Console.WriteLine("Füge neue informationen hinzu");
                    oldXml.DocumentElement.AppendChild(xmlImport);
                }
            }

            Thread savinThread = new Thread(() => SaveXml(oldXml, _clientName + "_NC.xml", mode));
            savinThread.Start();

        }

        private void SaveXml(XmlDocument xml1, string dateiName, int mode)
        {
            if (CheckForRoot(xml1))
            {
                Thread.Sleep(1000);
            }
            object thisThreadSyncObject = new object();
            bool thisThreadSyncLockTaken = false;

            Monitor.Enter(thisThreadSyncObject, ref thisThreadSyncLockTaken);
            {
                try
                {
                    for (; ; )
                    {
                        object runningThreadSyncObject = _keyDict.GetOrAdd(dateiName, thisThreadSyncObject);

                        if (runningThreadSyncObject == thisThreadSyncObject) { break; }

                        Thread.Sleep(20);

                    }

                    try
                    {

                        switch (mode)
                        {
                            case 1:
                                Console.WriteLine("Speichere");
                                xml1.Save(dateiName);
                                break;
                            case 2:
                                xml1.Save(_clientName + "_NC.xml");
                                Console.WriteLine("Lösche alte fertige xml");
                                Console.WriteLine("Speichere fertige xml");
                                File.Delete(_clientName + ".xml");
                                File.Move(_clientName + "_NC.xml", _clientName + ".xml");
                                break;

                            case 3:
                                xml1.Save(_clientName + "_NC.xml");
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

                }
                finally
                {
                    // Remove the key from the lock dictionary
                    object dummy;
                    _keyDict.TryRemove(dateiName, out dummy);
                    Monitor.Exit(thisThreadSyncObject);
                }




            }
        }
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }


    #endregion
}

