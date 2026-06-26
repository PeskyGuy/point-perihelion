using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    [XmlRoot("BuildingRef")]
    public class BuildingRef : RefDef
    {
        [XmlElement("category")]
        public string category;

        [XmlElement("health")]
        public float health;

        [XmlElement("destructible")]
        public bool destructible;

        [XmlElement("interactionType")]
        public string interactionType;

        [XmlElement("blockMovement")]
        public bool blockMovement;

        [XmlArray("inventory")]
        [XmlArrayItem("li")]
        public string[] inventory;
    }
}
