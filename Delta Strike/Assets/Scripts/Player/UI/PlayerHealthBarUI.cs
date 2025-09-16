using Game.Core.App;
using Game.Core.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.UI
{
    public sealed class PlayerHealthBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth playerHealth;
        [Header("Targets (either)")]
        [SerializeField] private Image fillImage;   
        [SerializeField] private Slider slider;     
        [SerializeField] private TMP_Text text;

        [Header("Options")]
        [SerializeField] private bool fillFromRight = true; 

        private void OnEnable()
        {
            if (playerHealth == null && DI.TryResolve<IPlayerRef>(out var pref))
                playerHealth = pref.Health as PlayerHealth;

            if (playerHealth != null)
            {
                playerHealth.OnChanged += OnHealthChanged;
                OnHealthChanged(playerHealth.Current, playerHealth.Max);
            }

            if (slider != null)
            {
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.wholeNumbers = false;
                slider.interactable = false;
                slider.direction = fillFromRight
                    ? Slider.Direction.LeftToRight
                    : Slider.Direction.RightToLeft;                
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            float ratio = max > 0f ? current / max : 0f;

            if (fillImage != null) fillImage.fillAmount = ratio;
            if (slider != null)    slider.value = ratio;
            if (text != null)      text.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }
    }
}