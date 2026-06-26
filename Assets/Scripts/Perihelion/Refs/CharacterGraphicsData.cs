using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

namespace Perihelion.Refs
{
    public struct FloatRange
    {
        public float min;
        public float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public static FloatRange Parse(string s)
        {
            if (string.IsNullOrEmpty(s)) return new FloatRange(0, 0);
            
            string[] parts = s.Split('~');
            if (parts.Length == 1)
            {
                if (float.TryParse(parts[0], out float val))
                {
                    return new FloatRange(val, val);
                }
            }
            else if (parts.Length == 2)
            {
                if (float.TryParse(parts[0], out float min) && float.TryParse(parts[1], out float max))
                {
                    return new FloatRange(min, max);
                }
            }
            
            return new FloatRange(0, 0);
        }

        public float Evaluate(float t)
        {
            // Simple ping-pong evaluation for bouncing:
            // t goes from 0 to 1
            // 0 -> min
            // 0.5 -> max
            // 1.0 -> min
            float pingPong = Mathf.PingPong(t * 2f, 1f);
            return Mathf.Lerp(min, max, pingPong);
        }
    }

    [Serializable]
    public class CharacterPartDef
    {
        [XmlAttribute("name")]
        public string name;

        [XmlAttribute("texPath")]
        public string texPath;

        [XmlAttribute("drawOrder")]
        public int drawOrder;

        [XmlAttribute("drawOrderNorth")]
        public string drawOrderNorthRaw;

        public int GetDrawOrder(int directionIndex)
        {
            if (directionIndex == 1 && !string.IsNullOrEmpty(drawOrderNorthRaw) && int.TryParse(drawOrderNorthRaw, out int n))
            {
                return n;
            }
            return drawOrder;
        }
    }

    [Serializable]
    public class CharacterGraphicsDef
    {
        [XmlElement("part")]
        public List<CharacterPartDef> parts = new List<CharacterPartDef>();
    }

    [Serializable]
    public class PartAnimDef
    {
        [XmlAttribute("part")]
        public string part;

        [XmlAttribute("offsetX")]
        public string offsetXRaw;

        [XmlAttribute("offsetY")]
        public string offsetYRaw;

        [XmlAttribute("rotationZ")]
        public string rotationZRaw;

        public FloatRange offsetX => FloatRange.Parse(offsetXRaw);
        public FloatRange offsetY => FloatRange.Parse(offsetYRaw);
        public FloatRange rotationZ => FloatRange.Parse(rotationZRaw);
    }

    [Serializable]
    public class AnimStateDef
    {
        [XmlAttribute("cycleDuration")]
        public float cycleDuration = 1f;

        [XmlElement("partAnim")]
        public List<PartAnimDef> partAnims = new List<PartAnimDef>();
    }

    [Serializable]
    public class CharacterAnimationsDef
    {
        [XmlElement("walk")]
        public AnimStateDef walk;

        [XmlElement("idle")]
        public AnimStateDef idle;
    }
}
