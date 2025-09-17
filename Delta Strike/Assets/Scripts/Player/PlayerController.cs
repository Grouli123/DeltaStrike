using Game.Core.Config;
using Game.Core.DI;
using Game.Input;
using UnityEngine;
using Game.Core.App;
using UnityEngine.Serialization;

namespace Game.Player
{
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

        private CharacterController _cc;
        private IInputService _input;
        private float _pitch;
        private float _vy;

        private UpgradeConfig _cfg;
        private Game.Systems.Progress.IProgressService _progress;
        private IGameplayBlockService _block;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = DI.Resolve<IInputService>();
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<Game.Systems.Progress.IProgressService>();
            _block = DI.Resolve<IGameplayBlockService>();

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
            
            var look = _input.LookDelta * _lookSensitivity;
            transform.Rotate(0f, look.x, 0f);
            _pitch = Mathf.Clamp(_pitch - look.y, -80f, 80f);
            if (_cameraPivot != null) _cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);

            var axis = _input.MoveAxis; 
            var hor = (transform.forward * axis.y + transform.right * axis.x) * GetSpeed();

            if (_cc.isGrounded)
            {
                _vy = -1f;
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                    _vy = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
            _vy += _gravity * Time.deltaTime;

            var move = new Vector3(hor.x, _vy, hor.z);
            _cc.Move(move * Time.deltaTime);

            if (_input.IsFirePressed && _gun != null) _gun.TryFire();
        }
    }
}