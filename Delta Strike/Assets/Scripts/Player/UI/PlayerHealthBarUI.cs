using Game.Core.App;
using Game.Core.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Player.UI
{
    public sealed class PlayerHealthBarUI : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _playerHealth;
        
        [Header("Targets (either)")]
        [SerializeField] private Image _fillImage; 
        [SerializeField] private Slider _slider;  
        [SerializeField] private TMP_Text _text;

        [Header("Options")]
        [SerializeField] private bool _fillFromRight = true; 
        
        private const float SliderMin = 0f;
        private const float SliderMax = 1f;

        private void OnEnable()
        {
            if (_playerHealth == null && DI.TryResolve<IPlayerRef>(out var pref))
                _playerHealth = pref.Health as PlayerHealth;

            if (_playerHealth != null)
            {
                _playerHealth.OnChanged += OnHealthChanged;
                OnHealthChanged(_playerHealth.Current, _playerHealth.Max);
            }

            if (_slider != null)
            {
                _slider.minValue = SliderMin;
                _slider.maxValue = SliderMax;
                _slider.wholeNumbers = false;
                _slider.interactable = false;
                _slider.direction = _fillFromRight
                    ? Slider.Direction.LeftToRight
                    : Slider.Direction.RightToLeft;                
            }
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
                _playerHealth.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            float ratio = max > 0f ? current / max : 0f;

            if (_fillImage != null) _fillImage.fillAmount = ratio;
            if (_slider != null)    _slider.value = ratio;
            if (_text != null)      _text.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }
    }
}