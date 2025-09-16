using Game.Core.DI;
using Game.Input;
using UnityEngine;

namespace Game.UI.Mobile
{
    public sealed class MobileInputBinder : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private VirtualJoystick moveJoystick;
        [SerializeField] private VirtualJoystick lookJoystick;  
        [SerializeField] private FireButton     fireButton;

        [Header("Sensitivity")]
        [SerializeField] private float lookSensitivity = 120f; 

        private MobileInputService _mobile;
        private float _openUpgradesPulse; 

        private void Awake()
        {
            if (!DI.TryResolve<IInputService>(out var input) || input is not MobileInputService mobile)
            {
                Debug.LogWarning("[MobileInputBinder] MobileInputService is not bound. Did you set inputMode=Mobile in Bootstrapper?");
                enabled = false;
                return;
            }
            _mobile = mobile;
        }

        private void Update()
        {
            if (_mobile == null) return;

            _mobile.moveAxis = moveJoystick ? moveJoystick.Value : Vector2.zero;

            var look = lookJoystick ? lookJoystick.Value : Vector2.zero;
            _mobile.lookDelta = look * (lookSensitivity * Time.deltaTime);

            _mobile.firePressed = fireButton && fireButton.IsPressed;

            if (_openUpgradesPulse > 0f)
            {
                _mobile.openUpgradePressed = true;
                _openUpgradesPulse = 0f;
            }
            else
            {
                _mobile.openUpgradePressed = false;
            }
        }

        public void OnOpenUpgradesButton()
        {
            _openUpgradesPulse = 1f;
        }
    }
}