using System.Collections.Generic;
using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;

namespace Game.Player
{
    public sealed class PlayerStats : IPlayerStats
    {
        private readonly Dictionary<StatType, UpgradeDef> _defs = new();
        private readonly IProgressService _progress;

        public PlayerStats()
        {
            var cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            foreach (var d in cfg.Upgrades) _defs[d.type] = d;
        }

        public float GetStatValue(StatType stat)
        {
            if (!_defs.TryGetValue(stat, out var def)) return 0f;
            var lvl = _progress.GetLevel(stat);
            return def.baseValue + def.perPointAdd * lvl;
        }

        public int GetLevel(StatType stat) => _progress.GetLevel(stat);
    }
}