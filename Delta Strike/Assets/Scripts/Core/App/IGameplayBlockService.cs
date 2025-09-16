using System;

namespace Game.Core.App
{
    public interface IGameplayBlockService
    {
        bool IsBlocked { get; }
        void SetBlocked(bool value);
        event Action<bool> OnChanged;
    }

    public sealed class GameplayBlockService : IGameplayBlockService
    {
        private bool _blocked;
        public bool IsBlocked => _blocked;
        public event Action<bool> OnChanged;

        public void SetBlocked(bool value)
        {
            if (_blocked == value) return;
            _blocked = value;
            OnChanged?.Invoke(_blocked);
        }
    }
}