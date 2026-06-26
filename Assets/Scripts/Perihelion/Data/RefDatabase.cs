using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Perihelion.Refs;

namespace Perihelion.Data
{
    public class RefDatabase : MonoBehaviour
    {
        public static RefDatabase Instance { get; private set; }

        private readonly Dictionary<string, RefDef> _allRefs = new();
        private readonly Dictionary<Type, Dictionary<string, RefDef>> _typedRefs = new();

        [SerializeField] private string dataPath = "Data";
        [SerializeField] private string patchPath = "Patches";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }

        public void LoadAll()
        {
            _allRefs.Clear();
            _typedRefs.Clear();

            string basePath = Path.Combine(Application.dataPath, dataPath);

            Register(RefLoader.LoadAll<SkillRef>(Path.Combine(basePath, "Skills"), "SkillRefs"));
            Register(RefLoader.LoadAll<ItemRef>(Path.Combine(basePath, "Items"), "ItemRefs"));
            Register(RefLoader.LoadAll<RecipeRef>(Path.Combine(basePath, "Recipes"), "RecipeRefs"));
            Register(RefLoader.LoadAll<ArchetypeRef>(Path.Combine(basePath, "Archetypes"), "ArchetypeRefs"));
            Register(RefLoader.LoadAll<CharacterRef>(Path.Combine(basePath, "Characters"), "CharacterRefs"));
            Register(RefLoader.LoadAll<EnemyRef>(Path.Combine(basePath, "Enemies"), "EnemyRefs"));
            Register(RefLoader.LoadAll<BuildingRef>(Path.Combine(basePath, "Buildings"), "BuildingRefs"));

            string patchDir = Path.Combine(Application.dataPath, patchPath);
            var patches = RefLoader.LoadPatches(patchDir);
            if (patches.Count > 0)
            {
                RefPatcher.Apply(patches, this);
            }

#if DEBUG
            Debug.Log($"[RefDatabase] Loaded {_allRefs.Count} total refs across {_typedRefs.Count} types. {patches.Count} patches applied.");
#endif
        }

        public T Get<T>(string refName) where T : RefDef
        {
            if (_typedRefs.TryGetValue(typeof(T), out var typed) && typed.TryGetValue(refName, out var def))
                return (T)def;
            return null;
        }

        public RefDef Get(string refName)
        {
            _allRefs.TryGetValue(refName, out var def);
            return def;
        }

        public List<T> GetAll<T>() where T : RefDef
        {
            var results = new List<T>();
            if (_typedRefs.TryGetValue(typeof(T), out var typed))
            {
                foreach (var def in typed.Values)
                    results.Add((T)def);
            }
            return results;
        }

        public void Remove(string refName)
        {
            if (_allRefs.TryGetValue(refName, out var def))
            {
                _allRefs.Remove(refName);
                var type = def.GetType();
                if (_typedRefs.TryGetValue(type, out var typed))
                    typed.Remove(refName);
            }
        }

        private void Register<T>(List<T> refs) where T : RefDef
        {
            var type = typeof(T);
            if (!_typedRefs.ContainsKey(type))
                _typedRefs[type] = new Dictionary<string, RefDef>();

            foreach (var def in refs)
            {
                if (string.IsNullOrEmpty(def.refName))
                {
                    Debug.LogWarning($"[RefDatabase] Skipping {type.Name} with empty refName");
                    continue;
                }

                if (_allRefs.ContainsKey(def.refName))
                {
                    Debug.LogWarning($"[RefDatabase] Duplicate refName '{def.refName}' — overwriting");
                }

                _allRefs[def.refName] = def;
                _typedRefs[type][def.refName] = def;
            }
        }
    }
}
