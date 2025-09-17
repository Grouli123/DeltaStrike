using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Mobile
{
    public sealed class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _handleRange = 80f; 

        public Vector2 Value { get; private set; } 

        private Vector2 _startAnchor;

        private void Awake()
        {
            if (_background == null) _background = transform as RectTransform;
            if (_handle != null) _startAnchor = _handle.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            if (_background == null || _handle == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, eventData.pressEventCamera, out var localPoint);

            var clamped = Vector2.ClampMagnitude(localPoint, _handleRange);
            _handle.anchoredPosition = _startAnchor + clamped;
            Value = clamped / _handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (_handle != null) _handle.anchoredPosition = _startAnchor;
        }
    }
}