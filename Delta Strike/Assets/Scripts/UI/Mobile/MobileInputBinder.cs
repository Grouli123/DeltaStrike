using Game.Core.DI;
using Game.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.UI.Mobile
{
    public sealed class MobileInputBinder : MonoBehaviour
    {
        private const float OpenUpgradePulseValue = 1f;
        
        [Header("UI")]
        [SerializeField] private VirtualJoystick _moveJoystick;
        [SerializeField] private VirtualJoystick _lookJoystick; 
        [SerializeField] private FireButton     _fireButton;

        [Header("Sensitivity")]
        [SerializeField] private float _lookSensitivity = 120f; 

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

            _mobile.moveAxis = _moveJoystick ? _moveJoystick.Value : Vector2.zero;

            var look = _lookJoystick ? _lookJoystick.Value : Vector2.zero;
            _mobile.lookDelta = look * (_lookSensitivity * Time.deltaTime);

            _mobile.firePressed = _fireButton && _fireButton.IsPressed;

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
            _openUpgradesPulse = OpenUpgradePulseValue;
        }
    }
}