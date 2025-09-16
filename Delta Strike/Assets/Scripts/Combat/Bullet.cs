using System.Collections.Generic;
using Game.Core.Pooling;
using Game.Player;               
using UnityEngine;
using Game.Audio; 
using Game.Core.DI;

namespace Game.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(PooledObject))]
    public sealed class Bullet : MonoBehaviour
    {
        [Header("Flight")]
        public float speed = 60f;
        public float maxLifeTime = 3f;

        [Header("Damage")]
        public float damage = 20f;
        public LayerMask hitMask = ~0;

        [Header("Impact FX")]
        [SerializeField] private GameObject impactVfxPrefab;

        private ObjectPool _audioPool;        
        private AudioClips _audioClips; 

        private Rigidbody _rb;
        private Collider _col;
        private PooledObject _pooled;
        private float _life;
        private GameObject _owner;

        private readonly List<Collider> _ignored = new();
        
        private IOneShotAudioService _audio;
        [SerializeField] private Game.AudioClips audioClips;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<Collider>();
            _pooled = GetComponent<PooledObject>();
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            DI.TryResolve(out _audio); 
        }

        private void OnEnable()
        {
            _life = maxLifeTime;
            if (_rb != null) _rb.velocity = Vector3.zero;
        }

        private void Update()
        {
            _life -= Time.deltaTime;
            if (_life <= 0f)
                Despawn();
        }

        public void Inject(ObjectPool audioPool, Game.AudioClips clips)
        {
            _audioPool = audioPool;
            _audioClips = clips;
        }

        public void Launch(Vector3 position, Vector3 direction, float speedOverride, float dmg, LayerMask mask, GameObject owner)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            damage = dmg;
            hitMask = mask;
            _owner = owner;

            ClearIgnored();
            if (_owner != null && _col != null)
            {
                var ownerCols = _owner.GetComponentsInChildren<Collider>();
                foreach (var oc in ownerCols)
                {
                    if (oc == null) continue;
                    Physics.IgnoreCollision(_col, oc, true);
                    _ignored.Add(oc);
                }
            }

            var spd = speedOverride > 0f ? speedOverride : speed;
            _rb.velocity = direction.normalized * spd;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var other = collision.collider;

            Vector3 hitPoint  = transform.position;
            Vector3 hitNormal = -_rb.velocity.normalized;
            if (collision.contactCount > 0)
            {
                var c = collision.GetContact(0);
                hitPoint  = c.point;
                hitNormal = c.normal;
            }

            if ((hitMask.value & (1 << other.gameObject.layer)) == 0)
            {
                Despawn();
                return;
            }

            if (_owner != null && other.transform.IsChildOf(_owner.transform))
                return;

            var hp = other.GetComponentInParent<IHealth>() ?? other.GetComponent<IHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            if (impactVfxPrefab)
            {
                var vfx = Instantiate(impactVfxPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
                Destroy(vfx, 2f);
            }

            if (audioClips && audioClips.impact)
                _audio?.PlayAt(hitPoint, audioClips.impact);

            Despawn();
        }

        private void Despawn()
        {
            ClearIgnored();
            _pooled.ReturnToPool();
        }

        private void ClearIgnored()
        {
            if (_col == null || _ignored.Count == 0) { _ignored.Clear(); return; }
            foreach (var c in _ignored)
                if (c != null) Physics.IgnoreCollision(_col, c, false);
            _ignored.Clear();
        }
    }
}