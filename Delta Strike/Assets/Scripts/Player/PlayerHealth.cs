using System;
using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;
using UnityEngine;

namespace Game.Player
{
    public interface IHealth
    {
        float Max { get; }
        float Current { get; }
        event Action<float, float> OnChanged;
        event Action OnDied;
        void TakeDamage(float amount);
        void HealFull();
    }

    public class HealthBase : MonoBehaviour, IHealth
    {
        [SerializeField] protected float max = 100f;
        public float Max => max;
        public float Current { get; protected set; }
        public event Action<float, float> OnChanged;
        public event Action OnDied;

        protected virtual void Awake()
        {
            Current = max;
            OnChanged?.Invoke(Current, Max);
        }

        public virtual void TakeDamage(float amount)
        {
            if (Current <= 0f) return;
            Current = Mathf.Max(0f, Current - Mathf.Max(0f, amount));
            OnChanged?.Invoke(Current, Max);
            if (Current <= 0f) OnDied?.Invoke();
        }

        public void HealFull()
        {
            Current = Max;
            OnChanged?.Invoke(Current, Max);
        }

        protected void SetMax(float newMax, bool keepRatio = true)
        {
            newMax = Mathf.Max(1f, newMax);
            float ratio = keepRatio && Max > 0f ? Current / Max : 1f;
            max = newMax;
            Current = Mathf.Clamp(newMax * ratio, 1f, newMax);
            OnChanged?.Invoke(Current, Max);
        }
    }

    public sealed class PlayerHealth : HealthBase
    {
        private IProgressService _progress;
        private UpgradeConfig _cfg;

        protected override void Awake()
        {
            base.Awake();
            _progress = DI.Resolve<IProgressService>();
            _cfg = DI.Resolve<UpgradeConfig>();
            RecalculateFromStats();
        }

        public void RecalculateFromStats()
        {
            foreach (var def in _cfg.Upgrades)
            {
                if (def.type != StatType.Health) continue;
                var lvl = _progress.GetLevel(StatType.Health);
                SetMax(def.baseValue + def.perPointAdd * lvl, keepRatio: true);
                break;
            }
        }
    }
}