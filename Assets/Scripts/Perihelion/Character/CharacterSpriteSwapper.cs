using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Perihelion.Character
{
    public class CharacterSpriteSwapper : MonoBehaviour
    {
        [SerializeField] private SpriteLibrary spriteLibrary;
        [SerializeField] private SpriteResolver[] resolvers;

        [Header("Sorting Orders")]
        [SerializeField] private DirectionSortConfig southSort = new(0, 2, 1, 1);
        [SerializeField] private DirectionSortConfig northSort = new(0, 2, -1, -1);
        [SerializeField] private DirectionSortConfig eastSort  = new(0, 2, -1, 1);

        private SpriteRenderer _bodyRenderer;
        private SpriteRenderer _headRenderer;
        private SpriteRenderer _handLRenderer;
        private SpriteRenderer _handRRenderer;
        private ProceduralAnimator _proceduralAnimator;

        private string _currentDirection = "south";
        private bool _currentFlip;
        private bool _isMoving;

        private void Awake()
        {
            if (spriteLibrary == null)
                spriteLibrary = GetComponent<SpriteLibrary>();

            if (resolvers == null || resolvers.Length == 0)
                resolvers = GetComponentsInChildren<SpriteResolver>();

            _proceduralAnimator = GetComponent<ProceduralAnimator>();

            foreach (Transform child in transform)
            {
                string lower = child.name.ToLowerInvariant();
                var sr = child.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                if (lower.Contains("body"))       _bodyRenderer  = sr;
                else if (lower.Contains("head"))   _headRenderer  = sr;
                else if (lower.Contains("handl") || lower.Contains("hand_l") || lower.Contains("lefthand"))
                    _handLRenderer = sr;
                else if (lower.Contains("handr") || lower.Contains("hand_r") || lower.Contains("righthand"))
                    _handRRenderer = sr;
            }

#if DEBUG
            if (_bodyRenderer == null) Debug.LogWarning("[Swapper] No child with 'body' in name found");
            if (_headRenderer == null) Debug.LogWarning("[Swapper] No child with 'head' in name found");
            if (_handLRenderer == null) Debug.LogWarning("[Swapper] No child with 'handl' in name found");
            if (_handRRenderer == null) Debug.LogWarning("[Swapper] No child with 'handr' in name found");
            if (_proceduralAnimator == null) Debug.LogWarning("[Swapper] No ProceduralAnimator component found on the character");
#endif
        }

        public void SetDirection(Vector2 direction, bool isMoving = true)
        {
            if (_proceduralAnimator != null && isMoving != _isMoving)
            {
                _isMoving = isMoving;
                _proceduralAnimator.SetMoving(_isMoving);
            }

            if (direction.sqrMagnitude < 0.01f) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            string directionStr;
            int dirIndex;
            bool flipX;

            if (angle > 45f && angle <= 135f)
            {
                directionStr = "north";
                dirIndex = 1;
                flipX = false;
            }
            else if (angle > -135f && angle <= -45f)
            {
                directionStr = "south";
                dirIndex = 0;
                flipX = false;
            }
            else if (angle > -45f && angle <= 45f)
            {
                directionStr = "east";
                dirIndex = 2;
                flipX = false;
            }
            else
            {
                directionStr = "east";
                dirIndex = 2;
                flipX = true;
            }

            if (directionStr != _currentDirection || flipX != _currentFlip)
            {
                _currentDirection = directionStr;
                _currentFlip = flipX;
            
                ApplyDirection(directionStr, flipX);
            }
        }

        private void ApplyDirection(string direction, bool flipX)
        {
            foreach (var resolver in resolvers)
            {
                string category = resolver.GetCategory();
                if (!string.IsNullOrEmpty(category))
                    resolver.SetCategoryAndLabel(category, direction);
            }

            DirectionSortConfig sort = direction switch
            {
                "north" => northSort,
                "east"  => eastSort,
                _       => southSort
            };

            ApplySort(sort, flipX);
            ApplyFlip(flipX);
        }

        private void ApplySort(DirectionSortConfig sort, bool flipX)
        {
            if (_bodyRenderer != null)  _bodyRenderer.sortingOrder  = sort.body;
            if (_headRenderer != null)  _headRenderer.sortingOrder  = sort.head;

            if (!flipX)
            {
                if (_handLRenderer != null) _handLRenderer.sortingOrder = sort.handL;
                if (_handRRenderer != null) _handRRenderer.sortingOrder = sort.handR;
            }
            else
            {
                if (_handLRenderer != null) _handLRenderer.sortingOrder = sort.handR;
                if (_handRRenderer != null) _handRRenderer.sortingOrder = sort.handL;
            }
        }

        private void ApplyFlip(bool flipX)
        {
            if (_bodyRenderer != null)  _bodyRenderer.flipX  = flipX;
            if (_headRenderer != null)  _headRenderer.flipX  = flipX;
            if (_handLRenderer != null) _handLRenderer.flipX = flipX;
            if (_handRRenderer != null) _handRRenderer.flipX = flipX;
        }

        [Serializable]
        public struct DirectionSortConfig
        {
            public int body;
            public int head;
            public int handL;
            public int handR;

            public DirectionSortConfig(int body, int head, int handL, int handR)
            {
                this.body = body;
                this.head = head;
                this.handL = handL;
                this.handR = handR;
            }
        }
    }
}
