using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    [XmlRoot("ArchetypeRef")]
    public class ArchetypeRef : RefDef
    {
        [XmlElement("role")]
        public string role;
    }
}
