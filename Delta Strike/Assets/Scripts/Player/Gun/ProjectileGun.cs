using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Pooling;
using Game.Systems.Progress;
using Game.Combat;
using UnityEngine;

namespace Game.Player.Gun
{
    public sealed class ProjectileGun : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private ObjectPool bulletPool;

        [Header("Ballistics")]
        [SerializeField] private float muzzleSpeed = 60f;
        [SerializeField] private float fireRate = 10f; 
        [SerializeField] private LayerMask hitMask = ~0; 

        private float _cd;
        private UpgradeConfig _cfg;
        private IProgressService _progress;
        private GameObject _ownerGO;

        private void Awake()
        {
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            _ownerGO = gameObject;
        }

        private void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }

        private float GetDamage()
        {
            foreach (var def in _cfg.Upgrades)
                if (def.type == StatType.Damage)
                    return def.baseValue + def.perPointAdd * _progress.GetLevel(StatType.Damage);
            return 10f;
        }

        public void TryFire()
        {
            if (_cd > 0f || bulletPool == null || muzzle == null) return;

            _cd = 1f / Mathf.Max(0.1f, fireRate);

            var go = bulletPool.Get();
            if (go == null) return;

            var bullet = go.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("[ProjectileGun] Bullet prefab in pool has no Bullet component.", bulletPool);
                bulletPool.Return(go);
                return;
            }

            Vector3 startPos = muzzle.position + muzzle.forward * 0.1f;
            bullet.Launch(startPos, muzzle.forward, muzzleSpeed, GetDamage(), hitMask, _ownerGO);
        }
    }
}