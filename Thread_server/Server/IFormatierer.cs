using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Sockets;

namespace Server
{
	interface IFormatierer
	{
		#region methods
		//Bereitstellen der Schnittstelle zum erzeugen und benutzen einers XML-Formatierers 
		void Formatieren(NetworkStream socketStream, int formatnr);
		#endregion
	}
}
