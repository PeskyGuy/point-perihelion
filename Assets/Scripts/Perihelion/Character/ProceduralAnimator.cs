using System.Collections.Generic;
using UnityEngine;

namespace Perihelion.Character
{
    public class ProceduralAnimator : MonoBehaviour
    {
        public ProceduralAnimProfile profile;

        private bool _isMoving;
        private float _animationTime;

        // Cache transforms by name
        private Dictionary<string, Transform> _childTransforms = new Dictionary<string, Transform>();
        private Dictionary<string, Vector3> _initialLocalPositions = new Dictionary<string, Vector3>();
        private Dictionary<string, Quaternion> _initialLocalRotations = new Dictionary<string, Quaternion>();

        public void SetMoving(bool moving)
        {
            if (_isMoving != moving)
            {
                _isMoving = moving;
                // Don't reset animation time to keep transitions smooth, or reset it if you want fresh sync
                // _animationTime = 0f; 
            }
        }

        private void Awake()
        {
            CacheTransforms(transform);
        }

        private void CacheTransforms(Transform parent)
        {
            foreach (Transform child in parent)
            {
                _childTransforms[child.name] = child;
                _initialLocalPositions[child.name] = child.localPosition;
                _initialLocalRotations[child.name] = child.localRotation;
                
                // Recursively cache
                CacheTransforms(child);
            }
        }

        private void Update()
        {
            if (profile == null) return;

            _animationTime += Time.deltaTime;
            
            // Loop time based on duration
            float normalizedTime = (_animationTime % profile.cycleDuration) / profile.cycleDuration;

            var activeDataList = _isMoving ? profile.walkData : profile.idleData;

            // Reset all cached transforms back to initial before applying new offsets
            // (Only necessary if you have parts that aren't animated in every state)
            foreach (var kvp in _childTransforms)
            {
                kvp.Value.localPosition = _initialLocalPositions[kvp.Key];
                kvp.Value.localRotation = _initialLocalRotations[kvp.Key];
            }

            foreach (var data in activeDataList)
            {
                if (_childTransforms.TryGetValue(data.targetChildName, out Transform t))
                {
                    float yOffset = data.positionY.Evaluate(normalizedTime);
                    float zRot = data.rotationZ.Evaluate(normalizedTime);

                    Vector3 pos = _initialLocalPositions[data.targetChildName];
                    pos.y += yOffset;
                    t.localPosition = pos;

                    Quaternion rot = _initialLocalRotations[data.targetChildName];
                    t.localRotation = rot * Quaternion.Euler(0, 0, zRot);
                }
            }
        }
    }
}
