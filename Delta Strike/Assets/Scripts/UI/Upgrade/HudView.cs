using Game.Core.DI;
using UnityEngine;

namespace Game.UI.Upgrade
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private GameObject upgradeWindowRoot;

        private Game.Input.IInputService _input;

        private void Awake()
        {
            _input = DI.Resolve<Game.Input.IInputService>();
            if (upgradeWindowRoot != null)
                upgradeWindowRoot.SetActive(false);
        }

        public void OnOpenUpgrades()
        {
            if (upgradeWindowRoot != null)
                upgradeWindowRoot.SetActive(true);
        }

        private void Update()
        {
            if (_input.IsOpenUpgradePressed)
                OnOpenUpgrades();
        }
    }
}