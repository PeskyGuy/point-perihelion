using UnityEngine;
using Perihelion.Refs;
using Perihelion.Data;

namespace Perihelion.Character
{
    public class EnemySpawner : MonoBehaviour
    {
        public string enemyRefId = "TestEnemy";
        public float spawnInterval = 5f;

        private float _lastSpawnTime;

        private void Start()
        {
            _lastSpawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - _lastSpawnTime >= spawnInterval)
            {
                Spawn();
                _lastSpawnTime = Time.time;
            }
        }

        private void Spawn()
        {
            var def = RefDatabase.Instance.Get<EnemyRef>(enemyRefId);
            if (def == null || string.IsNullOrEmpty(def.prefabPath))
            {
                Debug.LogWarning($"[EnemySpawner] Could not load enemy '{enemyRefId}' or missing prefabPath.");
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(def.prefabPath);
            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity);
                var entity = obj.GetComponent<EnemyEntity>();
                if (entity != null)
                {
                    entity.Initialize(enemyRefId);
                }

                var renderer = obj.GetComponent<CharacterRenderer>();
                if (renderer == null) renderer = obj.AddComponent<CharacterRenderer>();
                renderer.Initialize(enemyRefId);
            }
            else
            {
                Debug.LogWarning($"[EnemySpawner] Could not find prefab at '{def.prefabPath}' in Resources.");
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}
