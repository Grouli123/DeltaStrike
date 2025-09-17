using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Save;
using Game.Enemies;
using Game.Systems.Progress;
using UnityEngine;
using Game.Input;
using Game.Core.App;

namespace Game.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public sealed class Bootstrapper : MonoBehaviour
    {
        private const int TargetFrameRate = 120;

        [Header("Configs")]
        public UpgradeConfig upgradeConfig;
        public EnemyConfig enemyConfig;

        [Header("Input")]
        public InputMode inputMode = InputMode.Desktop;

        private void Awake()
        {
            DI.Clear();

            Application.targetFrameRate = TargetFrameRate;

            DI.Bind<ISaveService>(new PlayerPrefsSaveService());

            if (upgradeConfig) DI.Bind(upgradeConfig);
            if (enemyConfig)   DI.Bind(enemyConfig);

            var progress = new ProgressService();
            progress.Load();
            DI.Bind<IProgressService>(progress);

            DI.Bind<IGameplayBlockService>(new GameplayBlockService());
            DI.Bind<IGameStateService>(new GameStateService());

            if (inputMode == InputMode.Desktop || !Application.isMobilePlatform)
                DI.Bind<IInputService>(new DesktopInputService());
            else
                DI.Bind<IInputService>(new MobileInputService());

            if (!DI.TryResolve<IPlayerRef>(out _))
                DI.Bind<IPlayerRef>(new PlayerRef());
        }
    }
}