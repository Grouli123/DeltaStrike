using System;
using Game.Enemies;
using Game.Player;

namespace Game.Core.App
{
    public interface IGameStateService
    {
        int AliveEnemies { get; }
        bool IsPlayerDead { get; }

        event Action<int> OnEnemyCountChanged;
        event Action OnWin;
        event Action OnLose;

        void RegisterEnemy(EnemyHealth enemy);
        void UnregisterEnemy(EnemyHealth enemy);

        void RegisterPlayer(PlayerHealth player);
    }
}