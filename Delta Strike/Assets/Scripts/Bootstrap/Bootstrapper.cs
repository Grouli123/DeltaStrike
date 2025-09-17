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
        [Header("Configs")]
        public UpgradeConfig upgradeConfig;
        public EnemyConfig enemyConfig;

        [Header("Input")]
        public InputMode inputMode = InputMode.Desktop;
        
        private const int _TargetFrameRate = 120;

        private void Awake()
        {
            if (!DI.TryResolve<IPlayerRef>(out _))
                DI.Bind<IPlayerRef>(new Game.Core.App.PlayerRef());

            Application.targetFrameRate = _TargetFrameRate;

            DI.Clear();

            DI.Bind<ISaveService>(new PlayerPrefsSaveService());

            if (upgradeConfig != null) DI.Bind(upgradeConfig);
            if (enemyConfig  != null) DI.Bind(enemyConfig);

            var progress = new ProgressService();
            progress.Load();
            DI.Bind<IProgressService>(progress);
            DI.Bind<Game.Core.App.IGameplayBlockService>(new Game.Core.App.GameplayBlockService());

            if (inputMode == InputMode.Desktop || !Application.isMobilePlatform)
                DI.Bind<IInputService>(new DesktopInputService());
            else
                DI.Bind<IInputService>(new MobileInputService());
        }
    }
}