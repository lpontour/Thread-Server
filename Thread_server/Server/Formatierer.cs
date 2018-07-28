using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Sockets;
using System.IO;

namespace Server
{
	class Formatierer : FactoryFormate
	{
		#region fields
		private IFilewriter fileWriter = new Filewriter();
		protected string xmlString;
		protected XmlWriterSettings settings;
		protected XmlDocument xml;
		#endregion

		#region ctor
		//Erstellen des Formatierers, uebernehmen der Formateinstellungen, auslesen des Networkstreams und umwandlen in ein XmlDocument, danach Formatieren und starten des Schreibens
		public Formatierer(XmlWriterSettings neueSettings, XmlDocument neueXml)
		{
			xml = neueXml;

			settings = neueSettings;

			Formatieren(xml);

			Schreiben(xml);
		}
		#endregion

		#region methods
		//Aufrufen des schreibenden Interfaces
		private void Schreiben(XmlDocument fertigeXml)
		{
			fileWriter.DateiSchreiben(fertigeXml);
		}


		//Formatieren des XML-Dokumentes
		public string Formatieren(XmlDocument doc)
		{
			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, settings))
			{
				doc.Save(writer);
			}
			return sb.ToString();
		}
		#endregion
	}
}

