using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public class LootEntry
    {
        [XmlElement("itemRef")]
        public string itemRef;

        [XmlElement("weight")]
        public float weight;

        [XmlElement("countMin")]
        public int countMin;

        [XmlElement("countMax")]
        public int countMax;
    }

    [Serializable]
    [XmlRoot("EnemyRef")]
    public class EnemyRef : RefDef
    {
        [XmlElement("category")]
        public string category;

        [XmlElement("health")]
        public float health;

        [XmlElement("damage")]
        public float damage;

        [XmlElement("speed")]
        public float speed;

        [XmlElement("attackPattern")]
        public string attackPattern;

        [XmlArray("lootTable")]
        [XmlArrayItem("li")]
        public LootEntry[] lootTable;

        [XmlElement("prefabPath")]
        public string prefabPath;
    }
}
