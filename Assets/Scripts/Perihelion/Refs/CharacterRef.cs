using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public class SkillSlot
    {
        [XmlElement("skillRef")]
        public string skillRef;

        [XmlElement("unlockLevel")]
        public int unlockLevel;
    }

    [Serializable]
    [XmlRoot("CharacterRef")]
    public class CharacterRef : RefDef
    {
        [XmlElement("archetypeRef")]
        public string archetypeRef;

        [XmlElement("movementType")]
        public string movementType;

        [XmlElement("portraitPath")]
        public string portraitPath;

        [XmlArray("defaultLoadout")]
        [XmlArrayItem("li")]
        public string[] defaultLoadout;

        [XmlArray("skillSlots")]
        [XmlArrayItem("li")]
        public SkillSlot[] skillSlots;

        [XmlElement("maxActiveSkills")]
        public int maxActiveSkills;
    }
}
