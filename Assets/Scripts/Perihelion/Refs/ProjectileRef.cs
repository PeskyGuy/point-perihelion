using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    [XmlRoot("ProjectileRef")]
    public class ProjectileRef : RefDef
    {
        [XmlElement("speed")]
        public float speed = 10f;

        [XmlElement("damage")]
        public int damage = 10;

        [XmlElement("lifetime")]
        public float lifetime = 5f;

        [XmlElement("range")]
        public float range = 0f;

        [XmlElement("isAttached")]
        public bool isAttached = false;

        [XmlElement("cooldown")]
        public float cooldown = 0.5f;

        [XmlElement("knockback")]
        public float knockback = 0f;

        [XmlElement("prefabPath")]
        public string prefabPath;
    }
}
