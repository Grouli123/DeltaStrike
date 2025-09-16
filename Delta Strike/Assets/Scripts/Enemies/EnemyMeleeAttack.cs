using Game.Core.App;
using Game.Core.DI;
using Game.Player;
using UnityEngine;

namespace Game.Enemies
{
    [RequireComponent(typeof(EnemyController))]
    public sealed class EnemyMeleeAttack : MonoBehaviour
    {
        [Header("Melee")]
        [Min(0f)] public float damage = 10f;
        [Min(0.05f)] public float attackCooldown = 1f;
        [Min(0.1f)] public float attackRange = 2.0f; 
        [SerializeField] private Transform attackOrigin;
        
        [SerializeField] private bool autoRangeFromConfig = true;
        [SerializeField] private float rangeMargin = 0.25f; 

        private float _cd;
        private Transform _playerT;
        private IHealth _playerHealth;
        private CharacterController _playerCC;
        private CapsuleCollider _enemyCapsule;
        private EnemyConfig _cfg;


        private void Start()
        {
            var pref = DI.Resolve<IPlayerRef>();
            _playerT = pref.Transform;
            _playerHealth = pref.Health;
            _playerCC = pref.CC;

            TryGetComponent(out _enemyCapsule);

            // NEW: подтянем EnemyConfig и автонастроим радиус удара
            if (DI.TryResolve(out _cfg) && autoRangeFromConfig)
            {
                // удар должен доставать хотя бы до дистанции, на которой враг останавливается
                attackRange = Mathf.Max(attackRange, _cfg.stopDistance + rangeMargin);
            }

            if (attackOrigin == null) attackOrigin = transform;

            if (_playerT == null || _playerHealth == null)
                Debug.LogWarning("[EnemyMeleeAttack] PlayerRef not attached. Ensure PlayerController.Awake() binds IPlayerRef.", this);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.K))
            {
                Game.Core.DI.DI.Resolve<Game.Core.App.IPlayerRef>().Health.TakeDamage(10f);
            }
            
            if (_playerT == null || _playerHealth == null) return;

            if (_cd > 0f) _cd -= Time.deltaTime;

            Vector3 a = (attackOrigin ? attackOrigin.position : transform.position);

            Vector3 playerCenter = _playerT.position + new Vector3(0f, _playerCC != null ? _playerCC.center.y : 0f, 0f);
            Vector3 b = _playerT.position + new Vector3(0f, _playerCC ? _playerCC.center.y : 0f, 0f);

            a.y = 0f; b.y = 0f;
            
            float planarDist = Vector3.Distance(a, b);

            float expand = 0f;
            if (_playerCC != null)      expand += _playerCC.radius;
            if (_enemyCapsule != null)  expand += _enemyCapsule.radius * 0.5f; 

            bool inRange = (planarDist - expand) <= attackRange;

            if (inRange && _cd <= 0f)
            {
                _playerHealth.TakeDamage(damage);
                _cd = Mathf.Max(0.05f, attackCooldown);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.6f);
            const int seg = 36;
            var origin = attackOrigin ? attackOrigin.position : transform.position;
            origin.y += 0.05f;
            var prev = origin + Vector3.forward * attackRange;
            for (int i = 1; i <= seg; i++)
            {
                float ang = i * Mathf.PI * 2f / seg;
                var p = origin + new Vector3(Mathf.Sin(ang), 0f, Mathf.Cos(ang)) * attackRange;
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
        }
    }
}