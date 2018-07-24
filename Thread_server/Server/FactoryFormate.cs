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
		public void Formatieren(NetworkStream socketStream, int formatnr)
		{
			new Thread(() => 
			{
				Formatierer f = FormatiererWaehlen(formatnr, socketStream);
			}).Start();
		}


		//Format für Formatierer waehlen (bisher nur nummer 1 sinnvoll 2 und 3 zwar anders und vorhanden aber ohne richtigen Nutzen)
		private Formatierer FormatiererWaehlen(int format, NetworkStream socketStream)
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
		private Formatierer ErstelleFormatierer(XmlWriterSettings format , NetworkStream socketStream)
		{
			return (new Formatierer(format, socketStream));
		}


		#region Formate

		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInStandert(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, socketStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeineLeerzeichen(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "", NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, socketStream));
		}


		//Erzeugt die Formatierungseinstellungen für den Formatierer
		private Formatierer FormatiereInKeinZeilenumbruch(NetworkStream socketStream)
		{
			XmlWriterSettings settings = new XmlWriterSettings { Indent = true, IndentChars = "  ", NewLineChars = "\r", NewLineHandling = NewLineHandling.Replace };

			return (ErstelleFormatierer(settings, socketStream));
		}
		#endregion

		#endregion
	}
}

