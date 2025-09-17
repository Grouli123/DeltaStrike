using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Mobile
{
    public enum JoystickMode { Fixed, Floating, Dynamic }

    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Refs")]
        [SerializeField] private RectTransform _background;   
        [SerializeField] private RectTransform _handle;        

        [Header("Mode")]
        [SerializeField] private JoystickMode _mode = JoystickMode.Dynamic;
        [SerializeField] private bool hideBackgroundWhenIdle = true;

        [Header("Tuning")]
        [SerializeField, Min(16f)] private float handleRange = 80f; 
        [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.12f;

        [Header("Inversion")]
        [SerializeField] private bool invertX = false;
        [SerializeField] private bool invertY = false;

        public Vector2 Value { get; private set; }

        Canvas _canvas;
        Camera _uiCamera;
        int _pointerId = -1;
        bool _dragging;

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                _uiCamera = _canvas.worldCamera;

            if (_background == null) _background = transform as RectTransform;
            if (_handle == null && _background != null)
                _handle = _background.GetComponentInChildren<RectTransform>();

            if (_background && hideBackgroundWhenIdle && _mode != JoystickMode.Fixed)
                _background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData e)
        {
            if (_dragging) return;
            _dragging = true;
            _pointerId = e.pointerId;

            if (_mode != JoystickMode.Fixed && _background)
            {
                _background.gameObject.SetActive(true);
                _background.position = e.position;     
                if (_handle) _handle.anchoredPosition = Vector2.zero;
            }

            OnDrag(e);
        }

        public void OnDrag(PointerEventData e)
        {
            if (!_dragging || e.pointerId != _pointerId) return;
            if (!_background || !_handle) return;

            Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(_uiCamera, _background.position);
            Vector2 deltaScreen  = e.position - centerScreen;

            float radius = handleRange;
            if (_mode == JoystickMode.Dynamic && deltaScreen.magnitude > radius)
            {
                Vector2 newCenter = e.position - deltaScreen.normalized * radius;
                _background.position = newCenter;
                centerScreen = newCenter;
                deltaScreen  = e.position - centerScreen; 
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, e.position, _uiCamera, out var local);
            local -= _background.rect.center; 

            var v = local / radius;
            v = Vector2.ClampMagnitude(v, 1f);
            if (v.sqrMagnitude < deadZone * deadZone) v = Vector2.zero;

            if (invertX) v.x = -v.x;
            if (invertY) v.y = -v.y;

            Value = v;

            _handle.anchoredPosition = v * radius;
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (e.pointerId != _pointerId) return;
            _dragging = false;
            _pointerId = -1;

            Value = Vector2.zero;
            if (_handle) _handle.anchoredPosition = Vector2.zero;

            if (_background && hideBackgroundWhenIdle && _mode != JoystickMode.Fixed)
                _background.gameObject.SetActive(false);
        }

        public void SetMode(JoystickMode m) => _mode = m;
        public void SetRange(float px) => handleRange = Mathf.Max(16f, px);
        public void SetDeadZone(float dz) => deadZone = Mathf.Clamp01(dz);
        public void SetInvert(bool x, bool y) { invertX = x; invertY = y; }
    }
}