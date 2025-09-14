using Game.Core.DI;
using UnityEngine;

namespace Game.Enemies
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        public int count = 10;
        public Vector3 areaSize = new Vector3(30, 0, 30);
        public GameObject enemyPrefab;

        private EnemyConfig _cfg;

        private void Start()
        {
            _cfg = DI.Resolve<EnemyConfig>();
            
            if (enemyPrefab == null)
            {
                enemyPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemyPrefab.name = "Enemy";
                enemyPrefab.AddComponent<EnemyHealth>();
                enemyPrefab.AddComponent<EnemyController>();
            }

            for (int i = 0; i < count; i++)
            {
                var pos = transform.position + new Vector3(
                    Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                    0f,
                    Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
                );
                var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
                var hp = e.GetComponent<EnemyHealth>();
                float randHP = Random.Range(_cfg.minHP, _cfg.maxHP);
                hp.SetMax(randHP);
                hp.HealFull();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.z));
        }
    }
}