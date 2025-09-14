using Game.Core.DI;
using UnityEngine;

namespace Game.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyController : MonoBehaviour
    {
        private Transform _player;
        private EnemyHealth _health;
        private EnemyConfig _cfg;
        private Game.Systems.Progress.IProgressService _progress;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _cfg = DI.Resolve<EnemyConfig>();
            _progress = DI.Resolve<Game.Systems.Progress.IProgressService>();
        }

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
            _health.OnDied += OnDied;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnDied -= OnDied;
        }

        private void Update()
        {
            if (_player == null) return;

            var toPlayer = _player.position - transform.position;
            float dist = toPlayer.magnitude;
            if (dist <= _cfg.detectRange && dist > _cfg.stopDistance)
            {
                toPlayer.y = 0f;
                toPlayer.Normalize();
                transform.position += toPlayer * (_cfg.moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, Quaternion.LookRotation(toPlayer), 10f * Time.deltaTime);
            }
        }

        private void OnDied()
        {
            _progress.AddPoints(1);
            Destroy(gameObject);
        }
    }
}