using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public abstract class RefDef
    {
        [XmlElement("refName")]
        public string refName;

        [XmlElement("label")]
        public string label;

        [XmlElement("description")]
        public string description;

        [XmlElement("texturePath")]
        public string texturePath;

        [XmlArray("tags")]
        [XmlArrayItem("li")]
        public string[] tags;
    }
}
