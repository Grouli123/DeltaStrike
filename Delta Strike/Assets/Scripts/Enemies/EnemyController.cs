using Game.Core.App;
using Game.Core.DI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyController : MonoBehaviour
    {
        [Header("Move")]
        [Min(0f)] public float speed = 2.5f;
        [Min(0f)] public float stopDistance = 1.8f;
        [SerializeField] private float _turnLerp = 10f;

        private Transform _target;
        private const float _DirNormEpsilon = 0.0001f;

        private void Awake()
        {
            if (DI.TryResolve<IPlayerRef>(out var pref) && pref.Transform != null)
                _target = pref.Transform;
            else
                Debug.LogWarning("[EnemyController] PlayerRef not resolved. Enemy won't move.", this);
        }

        private void Update()
        {
            if (_target == null) return;

            Vector3 a = transform.position; a.y = 0f;
            Vector3 b = _target.position;  b.y = 0f;
            Vector3 to = b - a;
            float dist = to.magnitude;
            if (dist <= stopDistance) return;

            Vector3 dir = to / Mathf.Max(dist, _DirNormEpsilon);
            transform.position += dir * (speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir, Vector3.up),
                _turnLerp * Time.deltaTime);
        }
    }
}