using UnityEngine;
using Perihelion.Refs;
using Perihelion.Data;

namespace Perihelion.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyEntity : MonoBehaviour
    {
        public string enemyDefName;
        public EnemyRef Def { get; private set; }

        private float _currentHealth;
        private Rigidbody2D _rb;
        private Transform _playerTransform;

        private Vector2 _knockbackVelocity;
        private SpriteRenderer _sr;
        private Color _originalColor;
        private float _flashEndTime;

        public void Initialize(string defName)
        {
            enemyDefName = defName;
            Def = RefDatabase.Instance.Get<EnemyRef>(defName);

            if (Def == null)
            {
                Debug.LogError($"[EnemyEntity] Could not find EnemyRef '{defName}'");
                Destroy(gameObject);
                return;
            }

            _currentHealth = Def.health;
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;

            // Find player
            var playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                _playerTransform = playerController.transform;
            }
        }

        private void Start()
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
            if (_sr != null)
            {
                _originalColor = _sr.color;
            }

            if (Def == null && !string.IsNullOrEmpty(enemyDefName))
            {
                Initialize(enemyDefName);
            }
        }

        private void Update()
        {
            if (_sr != null && Time.time > _flashEndTime && _sr.color == Color.white)
            {
                _sr.color = _originalColor;
            }
        }

        private void FixedUpdate()
        {
            if (Def == null || _playerTransform == null)
            {
                if (_rb != null) _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_knockbackVelocity.sqrMagnitude > 0.1f)
            {
                _rb.linearVelocity = _knockbackVelocity;
                _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 10f);
            }
            else
            {
                Vector2 direction = (_playerTransform.position - transform.position).normalized;
                _rb.linearVelocity = direction * Def.speed;
            }
        }

        public void TakeDamage(float damage, float knockback = 0f, Vector2 knockbackDir = default)
        {
            if (Def == null) return;

            _currentHealth -= damage;
            
            if (_sr != null)
            {
                _sr.color = Color.white;
                _flashEndTime = Time.time + 0.1f;
            }

            if (knockback > 0f)
            {
                _knockbackVelocity = knockbackDir.normalized * knockback;
            }

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Drop loot later
            Destroy(gameObject);
        }
    }
}
