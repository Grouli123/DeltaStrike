using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Pooling;
using Game.Systems.Progress;
using Game.Combat;
using UnityEngine;
using Game.Audio;

namespace Game.Player.Gun
{
    public sealed class ProjectileGun : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private ObjectPool bulletPool;
        [SerializeField] private ObjectPool audioPool;            

        [Header("Ballistics")]
        [SerializeField] private float muzzleSpeed = 60f;
        [SerializeField] private float fireRate = 10f;            
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("FX/SFX")]
        [SerializeField] private Game.VFX.MuzzleFlash muzzleFlash;
        [SerializeField] private Game.AudioClips gunClips; 
        
        private IOneShotAudioService _audio;

        private float _cd;
        private UpgradeConfig _cfg;
        private IProgressService _progress;
        private GameObject _ownerGO;

        private void Awake()
        {
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            _ownerGO = gameObject;
            _audio = DI.Resolve<IOneShotAudioService>(); 
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

            if (muzzleFlash) muzzleFlash.Play();
            if (gunClips && gunClips.fire) _audio?.PlayAt(muzzle.position, gunClips.fire);

            _cd = 1f / Mathf.Max(0.1f, fireRate);

            var go = bulletPool.Get();
            if (!go) return;
            var bullet = go.GetComponent<Bullet>();
            if (!bullet) { Debug.LogError("Bullet missing", bulletPool); bulletPool.Return(go); return; }

            Vector3 startPos = muzzle.position + muzzle.forward * 0.1f;
            bullet.Launch(startPos, muzzle.forward, muzzleSpeed, GetDamage(), hitMask, _ownerGO);
        }
    }
}