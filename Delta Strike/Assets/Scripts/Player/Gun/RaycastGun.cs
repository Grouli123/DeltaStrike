using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;
using UnityEngine;

namespace Game.Player.Gun
{
    public sealed class RaycastGun : MonoBehaviour
    {
        [SerializeField] private Camera fireCamera;
        [SerializeField] private float fireRate = 10f; 
        [SerializeField] private float range = 100f;

        private float _cd;
        private UpgradeConfig _cfg;
        private IProgressService _progress;

        private void Awake()
        {
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            if (fireCamera == null) fireCamera = Camera.main;
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
            if (_cd > 0f) return;
            _cd = 1f / Mathf.Max(0.1f, fireRate);

            var ray = new Ray(fireCamera.transform.position, fireCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, range))
            {
                var h = hit.collider.GetComponentInParent<IHealth>() ?? hit.collider.GetComponent<IHealth>();
                if (h != null) h.TakeDamage(GetDamage());
            }
        }

        private void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }
    }
}