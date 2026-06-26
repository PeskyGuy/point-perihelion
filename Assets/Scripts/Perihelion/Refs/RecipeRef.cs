using System;
using System.Xml.Serialization;

namespace Perihelion.Refs
{
    [Serializable]
    public class RecipeIngredient
    {
        [XmlElement("itemRef")]
        public string itemRef;

        [XmlElement("count")]
        public int count;
    }

    [Serializable]
    [XmlRoot("RecipeRef")]
    public class RecipeRef : RefDef
    {
        [XmlArray("ingredients")]
        [XmlArrayItem("li")]
        public RecipeIngredient[] ingredients;

        [XmlElement("outputRef")]
        public string outputRef;

        [XmlElement("outputCount")]
        public int outputCount;

        [XmlElement("category")]
        public string category;
    }
}
