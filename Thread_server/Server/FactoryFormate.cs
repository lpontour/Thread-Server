using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Sockets;

namespace Server
{
	abstract class FactoryFormate : IFormatierer
	{
		#region methods
		//Implementation der Schnittstelle, startet erzeugen eines konkreten Formatierers
		public void Formatieren(NetworkStream socketStream, int formatnr)
		{
			Formatierer f = ErstelleFormatierer(formatnr, socketStream);
		}


		//Format für Formatierer waehlen (bisher nur nummer 1 sinnvoll 2 und 3 zwar anders und vorhanden aber ohne richtigen Nutzen)
		private Formatierer ErstelleFormatierer(int format, NetworkStream socketStream)
		{
			switch (format)
			{
				case 1:
					return (FormatiereInStandert(socketStream));
				case 2:
					return (FormatiereInKeineLeerzeichen(socketStream));
				case 3:
					return (FormatiereInKeinZeilenumbruch(socketStream));
				default:
					return (FormatiereInStandert(socketStream));
			}
		}


		//Konkretes erstellen des Formatierers mit dem gewünschten Format
		private Formatierer CreatFormatierer(XmlWriterSettings format , NetworkStream socketStream)
		{
			return (new Formatierer(format, socketStream));
		}


		#region Formate

		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInStandert(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (CreatFormatierer(settings, socketStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeineLeerzeichen(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (CreatFormatierer(settings, socketStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeinZeilenumbruch(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r", NewLineHandling = NewLineHandling.Replace };

			return (CreatFormatierer(settings, socketStream));
		}
		#endregion

		#endregion
	}
}

