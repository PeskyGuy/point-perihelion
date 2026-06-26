using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public class StatModifier
    {
        [XmlElement("stat")]
        public string stat;

        [XmlElement("value")]
        public float value;

        [XmlElement("operation")]
        public string operation;
    }

    [Serializable]
    [XmlRoot("ItemRef")]
    public class ItemRef : RefDef
    {
        [XmlElement("category")]
        public string category;

        [XmlElement("rarity")]
        public string rarity;

        [XmlElement("stackable")]
        public bool stackable;

        [XmlElement("maxStack")]
        public int maxStack;

        [XmlArray("statModifiers")]
        [XmlArrayItem("li")]
        public StatModifier[] statModifiers;
    }
}
