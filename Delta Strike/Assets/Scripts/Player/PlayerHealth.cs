using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;

namespace Game.Player
{
    public sealed class PlayerHealth : HealthBase
    {
        private IProgressService _progress;
        private UpgradeConfig _cfg;

        protected override void Awake()
        {
            base.Awake();
            _progress = DI.Resolve<IProgressService>();
            _cfg      = DI.Resolve<UpgradeConfig>();
            RecalculateFromStats();
        }

        public void RecalculateFromStats()
        {
            foreach (var def in _cfg.Upgrades)
            {
                if (def.type != StatType.Health) continue;

                int lvl = _progress.GetLevel(StatType.Health);
                float newMax = def.baseValue + def.perPointAdd * lvl;

                SetMax(newMax, keepRatio: true);
                break;
            }
        }
    }
}