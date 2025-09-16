using System.Collections.Generic;
using Game.Core.Pooling;
using Game.Player;
using UnityEngine;

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

        private Rigidbody _rb;
        private Collider _col;
        private PooledObject _pooled;
        private float _life;
        private GameObject _owner;

        private readonly List<Collider> _ignored = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<Collider>();
            _pooled = GetComponent<PooledObject>();

            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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

            if ((hitMask.value & (1 << other.gameObject.layer)) == 0)
            {
                Despawn();
                return;
            }

            if (_owner != null && (other.transform.IsChildOf(_owner.transform)))
            {
                return; 
            }

            var hp = other.GetComponentInParent<IHealth>() ?? other.GetComponent<IHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

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