using UnityEngine;
using Perihelion.Refs;
using Perihelion.Data;

namespace Perihelion.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileEntity : MonoBehaviour
    {
        public string projectileDefName;
        public ProjectileRef Def { get; private set; }

        private Rigidbody2D _rb;
        private float _spawnTime;
        private GameObject _shooter;
        private Vector2 _spawnPos;

        public void Initialize(string defName, Vector2 direction, GameObject shooter = null)
        {
            projectileDefName = defName;
            Def = RefDatabase.Instance.Get<ProjectileRef>(defName);
            
            if (Def == null)
            {
                Debug.LogError($"[ProjectileEntity] Could not find ProjectileRef '{defName}'");
                Destroy(gameObject);
                return;
            }

            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            
            // Set velocity
            _rb.linearVelocity = direction.normalized * Def.speed;

            // Rotate towards direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            _spawnTime = Time.time;
            _shooter = shooter;
            _spawnPos = transform.position;

            if (Def.isAttached && shooter != null)
            {
                transform.SetParent(shooter.transform);
                _rb.isKinematic = true;
                _rb.linearVelocity = Vector2.zero;
            }
        }

        private void Update()
        {
            if (Def == null) return;

            if (Time.time - _spawnTime >= Def.lifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (Def.range > 0f)
            {
                float dist = Vector2.Distance(_spawnPos, transform.position);
                if (dist >= Def.range)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_shooter != null && collision.transform.IsChildOf(_shooter.transform)) return;
            if (collision.GetComponentInParent<ProjectileEntity>() != null) return;

            var enemy = collision.GetComponentInParent<Perihelion.Character.EnemyEntity>();
            if (enemy != null && Def != null)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(Def.damage, Def.knockback, knockbackDir);
            }

            if (Def != null && !Def.isAttached)
            {
                Destroy(gameObject);
            }
        }
    }
}
