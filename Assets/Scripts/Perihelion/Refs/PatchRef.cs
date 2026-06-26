using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public class PatchField
    {
        [XmlElement("name")]
        public string name;

        [XmlElement("value")]
        public string value;
    }

    [Serializable]
    [XmlRoot("PatchRef")]
    public class PatchRef
    {
        [XmlElement("refName")]
        public string refName;

        [XmlElement("operation")]
        public string operation;

        [XmlElement("targetRef")]
        public string targetRef;

        [XmlElement("targetType")]
        public string targetType;

        [XmlArray("fields")]
        [XmlArrayItem("li")]
        public PatchField[] fields;
    }
}
