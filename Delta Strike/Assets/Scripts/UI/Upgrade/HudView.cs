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

        public void OnOpenUpgrades() => Open();

        public void Toggle()
        {
            if (upgradeWindowRoot == null) return;
            bool next = !upgradeWindowRoot.activeSelf;
            upgradeWindowRoot.SetActive(next);
        }

        public void Open()
        {
            if (upgradeWindowRoot == null) return;
            upgradeWindowRoot.SetActive(true);
        }

        public void Close()
        {
            if (upgradeWindowRoot == null) return;
            upgradeWindowRoot.SetActive(false);
        }

        private void Update()
        {
            if (_input.IsOpenUpgradePressed)
                Toggle();
        }
    }
}