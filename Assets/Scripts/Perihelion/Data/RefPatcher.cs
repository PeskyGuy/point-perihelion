using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Perihelion.Refs;

namespace Perihelion.Data
{
    public static class RefPatcher
    {
        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            { "SkillRef", typeof(SkillRef) },
            { "ItemRef", typeof(ItemRef) },
            { "RecipeRef", typeof(RecipeRef) },
            { "ArchetypeRef", typeof(ArchetypeRef) },
            { "CharacterRef", typeof(CharacterRef) },
            { "EnemyRef", typeof(EnemyRef) },
            { "BuildingRef", typeof(BuildingRef) },
        };

        public static void Apply(List<PatchRef> patches, RefDatabase db)
        {
            foreach (var patch in patches)
            {
                switch (patch.operation?.ToLower())
                {
                    case "modify":
                        ApplyModify(patch, db);
                        break;
                    case "remove":
                        db.Remove(patch.targetRef);
                        break;
                    default:
                        Debug.LogWarning($"[RefPatcher] Unknown operation '{patch.operation}' in patch '{patch.refName}'");
                        break;
                }
            }
        }

        private static void ApplyModify(PatchRef patch, RefDatabase db)
        {
            var target = db.Get(patch.targetRef);
            if (target == null)
            {
                Debug.LogWarning($"[RefPatcher] Target '{patch.targetRef}' not found for patch '{patch.refName}'");
                return;
            }

            if (patch.fields == null) return;

            Type type = target.GetType();
            foreach (var field in patch.fields)
            {
                FieldInfo fi = type.GetField(field.name, BindingFlags.Public | BindingFlags.Instance);
                if (fi == null)
                {
                    Debug.LogWarning($"[RefPatcher] Field '{field.name}' not found on {type.Name}");
                    continue;
                }

                try
                {
                    object converted = Convert.ChangeType(field.value, fi.FieldType);
                    fi.SetValue(target, converted);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RefPatcher] Failed to set {field.name}={field.value}: {e.Message}");
                }
            }
        }
    }
}
