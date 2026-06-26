using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    [XmlRoot("SkillRef")]
    public class SkillRef : RefDef
    {
        [XmlElement("parentRef")]
        public string parentRef;

        [XmlElement("category")]
        public string category;

        [XmlElement("maxLevel")]
        public int maxLevel;

        [XmlElement("xpPerLevel")]
        public int xpPerLevel;

        [XmlElement("color")]
        public string color;

        [XmlElement("cooldown")]
        public float cooldown;
    }
}
