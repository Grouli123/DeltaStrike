using UnityEngine;

namespace Game.Input
{
    public sealed class MobileInputService : IInputService
    {
        public Vector2 moveAxis;
        public Vector2 lookDelta;
        public bool firePressed;
        public bool openUpgradePressed;

        public Vector2 MoveAxis => moveAxis;
        public Vector2 LookDelta => lookDelta;
        public bool IsFirePressed => firePressed;
        public bool IsOpenUpgradePressed => openUpgradePressed;
    }
}