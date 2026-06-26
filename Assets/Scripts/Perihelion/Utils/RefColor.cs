using UnityEngine;

namespace Perihelion.Utils
{
    public static class RefColor
    {
        public static Color Parse(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return Color.white;

            string s = raw.Trim().TrimStart('(').TrimEnd(')');
            string[] parts = s.Split(',');

            if (parts.Length < 3) return Color.white;

            float r = float.Parse(parts[0].Trim()) / 255f;
            float g = float.Parse(parts[1].Trim()) / 255f;
            float b = float.Parse(parts[2].Trim()) / 255f;
            float a = parts.Length > 3 ? float.Parse(parts[3].Trim()) / 255f : 1f;

            return new Color(r, g, b, a);
        }
    }
}
