using Game.Core.Config;
using Game.Core.DI;
using Game.Input;
using UnityEngine;
using Game.Core.App;

namespace Game.Player
{
    public enum LookYawAxis { X, Y }
    
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Gun.ProjectileGun _gun;
        [SerializeField] private Transform _cameraPivot;

        [Header("Movement")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _lookSensitivity = 2f;
        [SerializeField] private float _jumpHeight = 1.2f;

        [SerializeField] private float _mouseLookSensitivity  = 2f; 
        [SerializeField] private float _mobileLookSensitivity = 220f;
        
        [Header("Look Options")]
        [SerializeField] private bool _horizontalOnly = true;   
        [SerializeField] private LookYawAxis _mouseYawAxis  = LookYawAxis.X;
        [SerializeField] private LookYawAxis _mobileYawAxis = LookYawAxis.X;
        [SerializeField] private bool _invertYaw = false;
        
        private CharacterController _cc;
        private IInputService _input;
        private float _pitch;
        private float _vy;
        private float _pitchMinDegrees = -80;
        private float _pitchMaxDegrees = 80;
        
        private const float MaxLookMagnitude = 20f;

        private UpgradeConfig _cfg;
        private Game.Systems.Progress.IProgressService _progress;
        private IGameplayBlockService _block;
        
        private bool IsMobileInput => _input is MobileInputService;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = DI.Resolve<IInputService>();
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<Game.Systems.Progress.IProgressService>();
            _block = DI.Resolve<IGameplayBlockService>();

            if (_cameraPivot == null && _playerCamera != null) 
                _cameraPivot = _playerCamera.transform;
            
            _pitch = 0f;
            
            if (_cameraPivot) _cameraPivot.localEulerAngles = new Vector3(0f, 0f, 0f);
            
            if (_playerCamera == null) _playerCamera = GetComponentInChildren<Camera>();
            if (_cameraPivot == null && _playerCamera != null) _cameraPivot = _playerCamera.transform;

            if (!Game.Core.DI.DI.TryResolve<IPlayerRef>(out var playerRef))
            {
                playerRef = new PlayerRef();
                Game.Core.DI.DI.Bind<IPlayerRef>(playerRef);
            }
            playerRef.Attach(this);
        }

        private float GetSpeed()
        {
            foreach (var def in _cfg.Upgrades)
                if (def.type == StatType.Speed)
                    return def.baseValue + def.perPointAdd * _progress.GetLevel(StatType.Speed);
            return 5f;
        }

        private void Update()
        {
            if (_block != null && _block.IsBlocked) return;

            var lookInput = _input.LookDelta; 

            float yawDeltaDeg;
            if (IsMobileInput)
            {
                float yawAxisVal = (_mobileYawAxis == LookYawAxis.X) ? lookInput.x : lookInput.y;
                if (_invertYaw) yawAxisVal = -yawAxisVal;

                yawDeltaDeg = yawAxisVal * _mobileLookSensitivity * Time.deltaTime;
            }
            else
            {
                const float MaxMouseDelta = 20f;
                if (lookInput.magnitude > MaxMouseDelta)
                    lookInput = lookInput.normalized * MaxMouseDelta;

                float yawAxisVal = (_mouseYawAxis == LookYawAxis.X) ? lookInput.x : lookInput.y;
                if (_invertYaw) yawAxisVal = -yawAxisVal;

                yawDeltaDeg = yawAxisVal * _mouseLookSensitivity;
            }

            transform.Rotate(0f, yawDeltaDeg, 0f);

            if (_horizontalOnly)
            {
                _pitch = 0f;
                if (_cameraPivot) _cameraPivot.localEulerAngles = Vector3.zero;
            }
            else
            {
                _pitch = Mathf.Clamp(_pitch - lookInput.y * (IsMobileInput ? _mobileLookSensitivity * Time.deltaTime : _mouseLookSensitivity), -80f, 80f);
                if (_cameraPivot) _cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
            }

            var axis = _input.MoveAxis;
            var hor  = (transform.forward * axis.y + transform.right * axis.x) * GetSpeed();

            if (_cc.isGrounded)
            {
                _vy = -1f;
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                    _vy = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
            _vy += _gravity * Time.deltaTime;

            _cc.Move(new Vector3(hor.x, _vy, hor.z) * Time.deltaTime);

            if (_input.IsFirePressed && _gun != null) _gun.TryFire();
        }

    }
}