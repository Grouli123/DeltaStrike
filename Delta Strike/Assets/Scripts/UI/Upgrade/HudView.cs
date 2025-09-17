using Game.Core.DI;
using UnityEngine;
using UnityEngine.Serialization;

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

        public void Toggle()
        {
            if (_upgradeWindowRoot == null) return;
            bool next = !_upgradeWindowRoot.activeSelf;
            _upgradeWindowRoot.SetActive(next);
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
                Toggle();
        }
    }
}