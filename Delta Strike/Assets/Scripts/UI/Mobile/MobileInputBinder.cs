using Game.Core.DI;
using Game.Input;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Mobile
{
    public sealed class MobileInputBinder : MonoBehaviour
    {
        private const float DefaultDeadZone = 0.12f;
        private const float MoveSmoothTime  = 0.05f; 
        private const float LookScale       = 1f;    

        [Header("UI")]
        [SerializeField] private VirtualJoystick _moveJoystick;
        [SerializeField] private VirtualJoystick _lookJoystick;
        [SerializeField] private FireButton _fireButton;
        [SerializeField] private Button _upgradesButton; 

        [Header("Tuning")]
        [SerializeField, Range(0f, 0.5f)] private float _deadZone = DefaultDeadZone;
        
        [Header("Fire on Look")]
        [SerializeField] private bool _fireWhenLooking = true;
        [SerializeField, Range(0f, 1f)] private float _fireLookThreshold = 0.35f; 

        [Header("Inversion")]
        [SerializeField] private bool invertMoveX = false;
        [SerializeField] private bool invertMoveY = false;
        [SerializeField] private bool invertLookX = false;
        [SerializeField] private bool invertLookY = false;

        private MobileInputService _mobile;
        private bool _pulseOpenUpgrade;

        private Vector2 _moveSmoothed, _moveVel;

        private void Awake()
        {
            if (!DI.TryResolve<IInputService>(out var i) || i is not MobileInputService mobile)
            {
                Debug.LogWarning("[MobileInputBinder] MobileInputService is not bound. Set InputMode=Auto/Mobile in Bootstrapper.");
                enabled = false;
                return;
            }
            _mobile = mobile;

            if (_upgradesButton) _upgradesButton.onClick.AddListener(OnOpenUpgradesButton);
        }

        private void Update()
        {
            if (_mobile == null) return;

            var mv = _moveJoystick ? _moveJoystick.Value : Vector2.zero;
            if (mv.sqrMagnitude < _deadZone * _deadZone) mv = Vector2.zero;
            if (invertMoveX) mv.x = -mv.x;
            if (invertMoveY) mv.y = -mv.y;
            _mobile.moveAxis = Vector2.SmoothDamp(_mobile.moveAxis, mv, ref _moveVel, 0.05f, Mathf.Infinity, Time.unscaledDeltaTime);

            var lk = _lookJoystick ? _lookJoystick.Value : Vector2.zero;
            if (lk.sqrMagnitude < _deadZone * _deadZone) lk = Vector2.zero;
            if (invertLookX) lk.x = -lk.x;
            if (invertLookY) lk.y = -lk.y;
            _mobile.lookDelta = lk;

            bool fireByLook = _fireWhenLooking && (lk.sqrMagnitude >= _fireLookThreshold * _fireLookThreshold);
            bool fireByBtn  = _fireButton && _fireButton.IsPressed;
            _mobile.firePressed = fireByLook || fireByBtn;

            _mobile.openUpgradePressed = _pulseOpenUpgrade;
            _pulseOpenUpgrade = false;
        }

        public void OnOpenUpgradesButton() => _pulseOpenUpgrade = true;
    }
}