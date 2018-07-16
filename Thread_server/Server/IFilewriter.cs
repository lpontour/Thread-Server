using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
    interface IFilewriter
    {
        #region methods
        //Bereitstellen der Schnittstelle zum erzeugen und benutzen einers XML-Formatierers 
        void DateiSchreiben(XmlDocument xmlDocument);
        #endregion
    }
}
