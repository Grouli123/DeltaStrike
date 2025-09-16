using Game.Core.DI;
using Game.Player;
using UnityEngine;

namespace Game.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public sealed class EnemyMeleeAttack : MonoBehaviour
    {
        [Header("Melee")]
        public float damage = 10f;
        public float attackCooldown = 1f;
        public float attackRange = 1.8f;

        private float _cd;
        private Transform _player;
        private IHealth _playerHealth;

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _player = p.transform;
                _playerHealth = p.GetComponentInChildren<IHealth>() ?? p.GetComponent<IHealth>();
            }

            if (DI.TryResolve<EnemyConfig>(out var cfg))
            {
                if (cfg.stopDistance > 0f) attackRange = cfg.stopDistance + 0.1f;
                if (cfg is { }) {  }
            }
        }

        private void Update()
        {
            if (_player == null || _playerHealth == null) return;

            if (_cd > 0f) _cd -= Time.deltaTime;

            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist <= attackRange && _cd <= 0f)
            {
                _playerHealth.TakeDamage(damage);
                _cd = Mathf.Max(0.1f, attackCooldown);
            }
        }
    }
}