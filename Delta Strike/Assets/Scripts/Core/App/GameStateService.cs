using System;
using System.Collections.Generic;
using Game.Enemies;
using Game.Player;
using UnityEngine;

namespace Game.Core.App
{
    public sealed class GameStateService : IGameStateService
    {
        public int AliveEnemies => _enemies.Count;
        public bool IsPlayerDead => _player != null && _player.Current <= 0f;

        public event Action<int> OnEnemyCountChanged;
        public event Action OnWin;
        public event Action OnLose;

        private readonly HashSet<EnemyHealth> _enemies = new();
        private PlayerHealth _player;
        private bool _ended;
        private bool _hadEnemies; // <-- добавили

        public void RegisterEnemy(EnemyHealth enemy)
        {
            if (enemy == null || _ended) return;
            if (_enemies.Add(enemy))
            {
                enemy.OnDied += HandleEnemyDied;
                _hadEnemies = true; // <-- отмечаем, что враги в этой сессии были
                OnEnemyCountChanged?.Invoke(AliveEnemies);
            }
        }

        public void UnregisterEnemy(EnemyHealth enemy)
        {
            if (enemy == null) return;
            if (_enemies.Remove(enemy))
            {
                enemy.OnDied -= HandleEnemyDied;
                OnEnemyCountChanged?.Invoke(AliveEnemies);
                TryWin(); // победу проверяем тут
            }
        }

        public void RegisterPlayer(PlayerHealth player)
        {
            if (_ended) return;
            if (_player != null) _player.OnDied -= HandlePlayerDied;
            _player = player;
            if (_player != null) _player.OnDied += HandlePlayerDied;

            // ВАЖНО: больше не вызываем TryWin() здесь — это и было ранним WIN.
        }

        private void HandleEnemyDied() => TryWin();

        private void HandlePlayerDied()
        {
            if (_ended) return;
            _ended = true;
            OnLose?.Invoke();
        }

        private void TryWin()
        {
            if (_ended) return;
            if (_hadEnemies && _enemies.Count == 0 && !IsPlayerDead) // <-- учитываем _hadEnemies
            {
                _ended = true;
                OnWin?.Invoke();
            }
        }
    }
}