using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Enemies
{
    public sealed class EnemyHealth : MonoBehaviour, Game.Player.IHealth
    {
        [SerializeField] private float _max = 100f;
        public float Max => _max;
        public float Current { get; private set; }

        public event Action<float, float> OnChanged;
        public event Action OnDied;

        private void Awake()
        {
            Current = _max;
            OnChanged?.Invoke(Current, Max);
        }

        public void SetMax(float value)
        {
            _max = Mathf.Max(1f, value);
            Current = Mathf.Clamp(Current, 0f, _max);
            OnChanged?.Invoke(Current, Max);
        }

        public void TakeDamage(float amount)
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
    }
}