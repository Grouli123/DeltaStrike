using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.UI
{
    public sealed class PlayerHealthBarUI : MonoBehaviour
    {
        [SerializeField] private Game.Player.PlayerHealth playerHealth;
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            if (playerHealth != null)
            {
                playerHealth.OnChanged += OnHealthChanged;
                OnHealthChanged(playerHealth.Current, playerHealth.Max);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            if (fillImage != null && max > 0f)
                fillImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
}