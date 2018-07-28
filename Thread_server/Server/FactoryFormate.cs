using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
	abstract class FactoryFormate : IFormatierer
	{
		#region methods
		//Implementation der Schnittstelle, startet erzeugen eines konkreten Formatierers in einem neuen Thread
		public void Formatieren(XmlDocument neueXml, int formatnr)
		{
			new Thread(() => 
			{
				Formatierer f = FormatiererWaehlen(formatnr, neueXml);
			}).Start();
		}


		//Format für Formatierer waehlen (bisher nur nummer 1 sinnvoll 2 und 3 zwar anders und vorhanden aber ohne richtigen Nutzen)
		private Formatierer FormatiererWaehlen(int format, XmlDocument xmlStream)
		{
			switch (format)
			{
				case 1:
					return (FormatiereInStandert(xmlStream));
				case 2:
					return (FormatiereInKeineLeerzeichen(xmlStream));
				case 3:
					return (FormatiereInKeinZeilenumbruch(xmlStream));
				default:
					return (FormatiereInStandert(xmlStream));
			}
		}


		//Konkretes erstellen des Formatierers mit dem gewünschten Format
		private Formatierer ErstelleFormatierer(XmlWriterSettings format , XmlDocument xmlStream)
		{
			return (new Formatierer(format, xmlStream));
		}


		#region Formate

		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInStandert(XmlDocument xmlStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, xmlStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeineLeerzeichen(XmlDocument xmlStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, xmlStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeinZeilenumbruch(XmlDocument xmlStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, xmlStream));
		}
		#endregion

		#endregion
	}
}

