using UnityEngine;

namespace Game.Core.Utils
{
    public sealed class CursorLocker : MonoBehaviour
    {
        [SerializeField] private bool lockOnStart = true;

        private int _uiRequests = 0; 

        private void Start()
        {
            if (lockOnStart) Lock();
            else Unlock();
        }

        private void Update()
        {
            if (!Application.isFocused || _uiRequests > 0)
            {
                Unlock();
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
                Lock();
        }

        public void RequestUiUnlock(bool enable)
        {
            _uiRequests = Mathf.Clamp(_uiRequests + (enable ? 1 : -1), 0, 1000);
        }

        public void Lock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}