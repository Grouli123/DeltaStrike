using Game.Core.Config;
using Game.Core.DI;
using Game.Core.Save;
using Game.Systems.Progress;
using Game.Input;
using Game.Core.App;
using UnityEngine;
using Game.Enemies;

namespace Game.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public sealed class Bootstrapper : MonoBehaviour
    {
        private const int DesktopTargetFPS = 120;
        private const int MobileTargetFPS  = 60;

        [Header("Configs")]
        public UpgradeConfig upgradeConfig;
        public EnemyConfig enemyConfig;

        [Header("Input")]
        public InputMode inputMode = InputMode.Auto; 

        private void Awake()
        {
            DI.Clear();

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Application.isMobilePlatform ? MobileTargetFPS : DesktopTargetFPS;

            DI.Bind<ISaveService>(new PlayerPrefsSaveService());
            if (upgradeConfig) DI.Bind(upgradeConfig);
            if (enemyConfig)   DI.Bind(enemyConfig);

            var progress = new ProgressService();
            progress.Load();
            DI.Bind<IProgressService>(progress);

            DI.Bind<IGameplayBlockService>(new GameplayBlockService());
            DI.Bind<IGameStateService>(new GameStateService());

            IInputService inputSvc;
            switch (inputMode)
            {
                case InputMode.Desktop:
                    inputSvc = new DesktopInputService();
                    break;
                case InputMode.Mobile:
                    inputSvc = new MobileInputService();
                    break;
                default: 
                    inputSvc = Application.isMobilePlatform
                        ? new MobileInputService()
                        : new DesktopInputService();
                    break;
            }
            DI.Bind<IInputService>(inputSvc);

            if (!DI.TryResolve<IPlayerRef>(out _))
                DI.Bind<IPlayerRef>(new PlayerRef());
        }
    }
}