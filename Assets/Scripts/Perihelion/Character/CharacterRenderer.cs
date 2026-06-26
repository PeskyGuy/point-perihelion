using UnityEngine;
using System.Collections.Generic;
using Perihelion.Refs;
using Perihelion.Data;

namespace Perihelion.Character
{
    public class CharacterRenderer : MonoBehaviour
    {
        private CharacterRef _def;
        
        private class PartData
        {
            public CharacterPartDef def;
            public GameObject go;
            public SpriteRenderer sr;
            public Sprite texSouth;
            public Sprite texNorth;
            public Sprite texEast;
            
            // Animation state
            public Vector3 baseLocalPos;
            public Quaternion baseLocalRot;
        }
        
        private List<PartData> _parts = new List<PartData>();
        
        private int _currentDirection = 0; // 0=south, 1=north, 2=east
        private bool _isMoving = false;
        private float _animTime = 0f;

        public void Initialize(string characterRefId)
        {
            _def = RefDatabase.Instance.Get<CharacterRef>(characterRefId);
            if (_def == null || _def.graphics == null) return;
            
            // Cleanup existing parts if re-initializing
            foreach (var p in _parts)
            {
                if (p.go != null) Destroy(p.go);
            }
            _parts.Clear();

            // Build parts
            foreach (var partDef in _def.graphics.parts)
            {
                var pd = new PartData { def = partDef };
                
                // Load sprites
                pd.texSouth = Resources.Load<Sprite>($"{partDef.texPath}_south");
                pd.texNorth = Resources.Load<Sprite>($"{partDef.texPath}_north");
                pd.texEast = Resources.Load<Sprite>($"{partDef.texPath}_east");
                
                // Create GameObject
                pd.go = new GameObject(partDef.name);
                pd.go.transform.SetParent(transform);
                pd.go.transform.localPosition = Vector3.zero;
                pd.go.transform.localRotation = Quaternion.identity;
                
                pd.baseLocalPos = Vector3.zero;
                pd.baseLocalRot = Quaternion.identity;
                
                pd.sr = pd.go.AddComponent<SpriteRenderer>();
                pd.sr.sortingOrder = partDef.drawOrder;
                
                _parts.Add(pd);
            }
            
            UpdateSprites();
        }

        public void SetDirection(Vector2 aimDirection, bool isMoving)
        {
            _isMoving = isMoving;

            int newDir = _currentDirection;
            if (Mathf.Abs(aimDirection.x) > Mathf.Abs(aimDirection.y))
            {
                newDir = 2; // East (or West via flip)
            }
            else
            {
                newDir = aimDirection.y > 0 ? 1 : 0; // North or South
            }

            bool facingLeft = (newDir == 2) && aimDirection.x < -0.1f;

            bool changed = (newDir != _currentDirection);
            _currentDirection = newDir;

            foreach (var p in _parts)
            {
                if (p.sr != null)
                {
                    p.sr.flipX = facingLeft;
                }
            }

            if (changed)
            {
                UpdateSprites();
            }
        }

        private void UpdateSprites()
        {
            foreach (var p in _parts)
            {
                if (p.sr != null)
                {
                    p.sr.sortingOrder = p.def.GetDrawOrder(_currentDirection);

                    if (_currentDirection == 0) p.sr.sprite = p.texSouth;
                    else if (_currentDirection == 1) p.sr.sprite = p.texNorth;
                    else if (_currentDirection == 2) p.sr.sprite = p.texEast;
                }
            }
        }

        private void Update()
        {
            if (_def == null || _def.animations == null) return;

            AnimStateDef activeState = _isMoving ? _def.animations.walk : _def.animations.idle;
            if (activeState == null)
            {
                // Reset to base
                foreach (var p in _parts)
                {
                    p.go.transform.localPosition = p.baseLocalPos;
                    p.go.transform.localRotation = p.baseLocalRot;
                }
                return;
            }

            // Advance time
            _animTime += Time.deltaTime;
            float normalizedTime = (_animTime % activeState.cycleDuration) / activeState.cycleDuration;

            // Apply animation
            foreach (var p in _parts)
            {
                // Find animation for this part
                PartAnimDef animDef = null;
                foreach (var a in activeState.partAnims)
                {
                    if (a.part == p.def.name)
                    {
                        animDef = a;
                        break;
                    }
                }

                if (animDef != null)
                {
                    float xOff = animDef.offsetX.Evaluate(normalizedTime);
                    float yOff = animDef.offsetY.Evaluate(normalizedTime);
                    float zRot = animDef.rotationZ.Evaluate(normalizedTime);
                    
                    Vector3 pos = p.baseLocalPos;
                    pos.x += xOff;
                    pos.y += yOff;
                    p.go.transform.localPosition = pos;
                    
                    p.go.transform.localRotation = p.baseLocalRot * Quaternion.Euler(0, 0, zRot);
                }
                else
                {
                    // Reset if no anim
                    p.go.transform.localPosition = p.baseLocalPos;
                    p.go.transform.localRotation = p.baseLocalRot;
                }
            }
        }
    }
}
