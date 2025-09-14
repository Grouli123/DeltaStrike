using UnityEngine;

namespace Game.Input
{
    public sealed class DesktopInputService : IInputService
    {
        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";

        public Vector2 MoveAxis => new Vector2(
            (UnityEngine.Input.GetKey(KeyCode.D) ? 1 : 0) - (UnityEngine.Input.GetKey(KeyCode.A) ? 1 : 0),
            (UnityEngine.Input.GetKey(KeyCode.W) ? 1 : 0) - (UnityEngine.Input.GetKey(KeyCode.S) ? 1 : 0)
        );

        public Vector2 LookDelta => new Vector2(
            UnityEngine.Input.GetAxisRaw(MouseX),
            UnityEngine.Input.GetAxisRaw(MouseY)
        );

        public bool IsFirePressed => UnityEngine.Input.GetMouseButton(0);

        public bool IsOpenUpgradePressed => UnityEngine.Input.GetKeyDown(KeyCode.U);
    }
}