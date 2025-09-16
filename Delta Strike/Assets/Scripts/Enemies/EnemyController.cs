using Game.Core.DI;
using Game.Core.App;
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
        
        [Header("Move")]
        public float speed = 2.5f;
        public float stopDistance = 1.8f;   

        private Transform _target;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _cfg = DI.Resolve<EnemyConfig>();
            _progress = DI.Resolve<Game.Systems.Progress.IProgressService>();
            
            if (DI.TryResolve<IPlayerRef>(out var pref) && pref.Transform != null)
                _target = pref.Transform;
            else
                Debug.LogWarning("[EnemyController] PlayerRef not resolved. Enemy won't move.", this);
        }

        private void Start()
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) _player = p.transform;
        }

        private void Update()
        {
            if (_player == null) return;

            var topPlayer = _player.position - transform.position;
            float dist = topPlayer.magnitude;
            if (dist <= _cfg.detectRange && dist > _cfg.stopDistance)
            {
                topPlayer.y = 0f;
                topPlayer.Normalize();
                transform.position += topPlayer * (_cfg.moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, Quaternion.LookRotation(topPlayer), 10f * Time.deltaTime);
            }
        }
    }
}