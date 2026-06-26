using UnityEngine;
using Perihelion.Data;
using Perihelion.Refs;

namespace Perihelion.Character
{
    public class CharacterEntity : MonoBehaviour
    {
        [Header("Data Link")]
        public string defName;

        public CharacterRef Def { get; private set; }

        private void Start()
        {
            if (!string.IsNullOrEmpty(defName))
            {
                Def = RefDatabase.Instance.Get<CharacterRef>(defName);
                if (Def == null)
                {
                    Debug.LogError($"[CharacterEntity] Could not find CharacterRef with defName '{defName}' on GameObject {gameObject.name}");
                }
                else
                {
                    // Apply size scale from XML
                    transform.localScale = new Vector3(Def.size, Def.size, 1f);
                }
            }
            else
            {
                Debug.LogWarning($"[CharacterEntity] No defName assigned to GameObject {gameObject.name}");
            }
        }
    }
}
