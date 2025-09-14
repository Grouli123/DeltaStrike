using System.Collections.Generic;
using Game.Core.Config;

namespace Game.Systems.Progress
{
    public interface IProgressService
    {
        int Points { get; }
        int GetLevel(StatType stat);
        IReadOnlyDictionary<StatType, int> Levels { get; }

        void AddPoints(int amount);
        void SetLevel(StatType stat, int level);

        void Save();
        void Load();
    }
}