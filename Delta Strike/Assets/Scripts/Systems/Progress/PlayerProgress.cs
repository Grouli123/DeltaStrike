using System;
using System.Collections.Generic;
using Game.Core.Config;

namespace Game.Systems.Progress
{
    [Serializable]
    public sealed class PlayerProgress
    {
        public int points = 0;
        public List<LevelEntry> levels = new();

        [Serializable]
        public struct LevelEntry
        {
            public StatType stat;
            public int level;
        }
    }
}