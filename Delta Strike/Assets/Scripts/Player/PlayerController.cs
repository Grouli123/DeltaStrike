using Game.Core.Config;
using Game.Core.DI;
using Game.Input;
using UnityEngine;
using Game.Core.App; 

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Game.Player.Gun.ProjectileGun gun;
        [SerializeField] private Transform cameraPivot;

        [Header("Movement")]
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float jumpHeight = 1.2f;

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

            if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
            if (cameraPivot == null && playerCamera != null) cameraPivot = playerCamera.transform;
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
            
            var look = _input.LookDelta * lookSensitivity;
            transform.Rotate(0f, look.x, 0f);
            _pitch = Mathf.Clamp(_pitch - look.y, -80f, 80f);
            if (cameraPivot != null) cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);

            var axis = _input.MoveAxis; 
            var hor = (transform.forward * axis.y + transform.right * axis.x) * GetSpeed();

            if (_cc.isGrounded)
            {
                _vy = -1f;
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                    _vy = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            _vy += gravity * Time.deltaTime;

            var move = new Vector3(hor.x, _vy, hor.z);
            _cc.Move(move * Time.deltaTime);

            if (_input.IsFirePressed && gun != null) gun.TryFire();
        }
    }
}