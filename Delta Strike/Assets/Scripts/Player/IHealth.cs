using System;

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
}