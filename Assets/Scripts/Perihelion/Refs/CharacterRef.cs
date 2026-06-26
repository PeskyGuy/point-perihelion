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

        [XmlElement("moveSpeed")]
        public float moveSpeed = 5f;

        [XmlElement("size")]
        public float size = 1f;

        [XmlElement("defaultProjectileRef")]
        public string defaultProjectileRef;

        [XmlElement("defaultSecondaryAttack")]
        public string defaultSecondaryAttack;

        [XmlElement("portraitPath")]
        public string portraitPath;

        [XmlElement("graphics")]
        public CharacterGraphicsDef graphics;

        [XmlElement("animations")]
        public CharacterAnimationsDef animations;

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
