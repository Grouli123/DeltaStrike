using Game.Core.DI;
using Game.Core.Utils;
using UnityEngine;

namespace Game.UI.Upgrade
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private GameObject upgradeWindowRoot;
        private Game.Input.IInputService _input;
        private CursorLocker _locker;
        private bool _cursorToggled;

        private void Start()
        {
            _input  = DI.Resolve<Game.Input.IInputService>();
            _locker = FindObjectOfType<CursorLocker>();
            if (upgradeWindowRoot != null) upgradeWindowRoot.SetActive(false);
        }

        public void OnOpenUpgrades()
        {
            if (upgradeWindowRoot != null)
            {
                upgradeWindowRoot.SetActive(true);
                _locker?.RequestUiUnlock(true);
            }
            else
            {
                _cursorToggled = !_cursorToggled;
                _locker?.RequestUiUnlock(_cursorToggled);
            }
        }

        private void Update()
        {
            if (_input.IsOpenUpgradePressed)
                OnOpenUpgrades();
        }
    }
}