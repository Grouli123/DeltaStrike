using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class HealthBase : MonoBehaviour, IHealth
    {
        protected const  float _DefaultMaxHealth = 100f;
        private   const  float _MinNewMax        = 1f;   
        private   const  float _MinCurrentClamp  = 1f;   

        [SerializeField] protected float _max = _DefaultMaxHealth;
        public float Max => _max;

        public float Current { get; protected set; }

        public event Action<float, float> OnChanged;
        public event Action OnDied;

        [SerializeField] private float debugCurrent;

        protected virtual void Awake()
        {
            Current = _max;
            OnChanged?.Invoke(Current, Max);
        }

        public virtual void TakeDamage(float amount)
        {
            if (Current <= 0f) return;

            float before = Current;
            Current = Mathf.Max(0f, Current - Mathf.Max(0f, amount));
            Debug.Log($"[HealthBase] {name} TakeDamage {amount} | {before} -> {Current}");

            OnChanged?.Invoke(Current, Max);
            if (Current <= 0f) OnDied?.Invoke();
        }

        private void LateUpdate()
        {
            debugCurrent = Current;
        }

        public void HealFull()
        {
            Current = Max;
            OnChanged?.Invoke(Current, Max);
        }

        protected void SetMax(float newMax, bool keepRatio = true)
        {
            newMax = Mathf.Max(_MinNewMax, newMax);

            float ratio = keepRatio && Max > 0f ? Current / Max : 1f;

            _max = newMax;
            Current = Mathf.Clamp(newMax * ratio, _MinCurrentClamp, newMax);

            OnChanged?.Invoke(Current, Max);
        }
    }
}