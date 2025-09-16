using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Mobile
{
    public sealed class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 80f; 

        public Vector2 Value { get; private set; } 

        private Vector2 _startAnchor;

        private void Awake()
        {
            if (background == null) background = transform as RectTransform;
            if (handle != null) _startAnchor = handle.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || handle == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out var localPoint);

            var clamped = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = _startAnchor + clamped;
            Value = clamped / handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (handle != null) handle.anchoredPosition = _startAnchor;
        }
    }
}