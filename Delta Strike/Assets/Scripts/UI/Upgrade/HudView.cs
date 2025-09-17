using Game.Core.DI;
using UnityEngine;

namespace Game.UI.Upgrade
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private GameObject _upgradeWindowRoot;

        private Game.Input.IInputService _input;

        private void Awake()
        {
            _input = DI.Resolve<Game.Input.IInputService>();
            if (_upgradeWindowRoot != null)
                _upgradeWindowRoot.SetActive(false);
        }

        public void OnOpenUpgrades() => Open();
        public void OpenUpgrades()   => Open();   

        public void Toggle()
        {
            if (_upgradeWindowRoot == null) return;
            _upgradeWindowRoot.SetActive(!_upgradeWindowRoot.activeSelf);
        }

        public void Open()
        {
            if (_upgradeWindowRoot == null) return;
            _upgradeWindowRoot.SetActive(true);
        }

        public void Close()
        {
            if (_upgradeWindowRoot == null) return;
            _upgradeWindowRoot.SetActive(false);
        }

        private void Update()
        {
            if (_input.IsOpenUpgradePressed)
                OpenUpgrades();
        }
    }
}