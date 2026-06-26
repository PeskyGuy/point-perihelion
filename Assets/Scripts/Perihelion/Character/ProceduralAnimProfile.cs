using System;
using System.Collections.Generic;
using UnityEngine;

namespace Perihelion.Character
{
    [Serializable]
    public class PartAnimationData
    {
        public string targetChildName;
        
        [Tooltip("Local Y offset applied over time")]
        public AnimationCurve positionY = AnimationCurve.Constant(0f, 1f, 0f);
        
        [Tooltip("Local Z rotation applied over time")]
        public AnimationCurve rotationZ = AnimationCurve.Constant(0f, 1f, 0f);
    }

    [CreateAssetMenu(fileName = "NewProceduralAnimProfile", menuName = "Perihelion/Animation/ProceduralAnimProfile")]
    public class ProceduralAnimProfile : ScriptableObject
    {
        public float cycleDuration = 1f;

        [Header("States")]
        public List<PartAnimationData> idleData = new List<PartAnimationData>();
        public List<PartAnimationData> walkData = new List<PartAnimationData>();
        
        // Attack animations can be defined here later if needed
    }
}
