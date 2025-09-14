using System.Collections.Generic;
using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Save;
using UnityEngine;

namespace Game.Systems.Progress
{
    public sealed class ProgressService : IProgressService
    {
        private const string SaveKey = "player_progress_v1";
        private readonly Dictionary<StatType, int> _levels = new();
        public int Points { get; private set; }

        public IReadOnlyDictionary<StatType, int> Levels => _levels;

        public int GetLevel(StatType stat) => _levels.TryGetValue(stat, out var lvl) ? lvl : 0;

        public void AddPoints(int amount)
        {
            Points = Mathf.Max(0, Points + Mathf.Max(0, amount));
            Save();
        }

        public void SetLevel(StatType stat, int level)
        {
            _levels[stat] = Mathf.Max(0, level);
        }

        public void Save()
        {
            var data = new PlayerProgress { points = Points };
            foreach (var kv in _levels)
                data.levels.Add(new PlayerProgress.LevelEntry { stat = kv.Key, level = kv.Value });

            var json = JsonUtility.ToJson(data);
            var save = DI.Resolve<ISaveService>();
            save.SaveString(SaveKey, json);
        }

        public void Load()
        {
            _levels.Clear();
            var save = DI.Resolve<ISaveService>();
            var json = save.LoadString(SaveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                var data = JsonUtility.FromJson<PlayerProgress>(json);
                Points = data.points;
                foreach (var e in data.levels)
                    _levels[e.stat] = Mathf.Max(0, e.level);
            }
            else
            {
                Points = 0;
            }
        }
    }
}