using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Pooling;
using Game.Systems.Progress;
using Game.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player.Gun
{
    public sealed class ProjectileGun : MonoBehaviour
    {
        private const float OneSecond            = 1f;     
        private const float FireRateMin          = 0.1f;   
        private const float MuzzleSpawnOffset    = 0.1f;   

        [FormerlySerializedAs("muzzle")]
        [Header("Refs")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private ObjectPool _bulletPool;

        [Header("Ballistics")]
        [SerializeField] private float _muzzleSpeed = 60f;
        [SerializeField] private float _fireRate    = 10f; 
        [SerializeField] private LayerMask _hitMask = ~0;

        [Header("FX/SFX")]
        [SerializeField] private Game.VFX.MuzzleFlash _muzzleFlash;
        [SerializeField] private Game.AudioClips     _gunClips;

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
            if (_cd > 0f || _bulletPool == null || _muzzle == null) return;

            if (_muzzleFlash) _muzzleFlash.Play();

            _cd = OneSecond / Mathf.Max(FireRateMin, _fireRate);

            var go = _bulletPool.Get();
            if (!go) return;

            var bullet = go.GetComponent<Bullet>();
            if (!bullet)
            {
                Debug.LogError("[ProjectileGun] Bullet prefab in pool has no Bullet component.", _bulletPool);
                _bulletPool.Return(go);
                return;
            }

            Vector3 startPos = _muzzle.position + _muzzle.forward * MuzzleSpawnOffset;
            bullet.Launch(startPos, _muzzle.forward, _muzzleSpeed, GetDamage(), _hitMask, _ownerGO);
        }
    }
}