using Game.Core.Config;
using TMPro;                 
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.Upgrade
{
    public sealed class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;  
        [SerializeField] private TMP_Text _levelText;  
        [SerializeField] private Button _plusButton;
        [SerializeField] private Image _iconImage;

        public UpgradeDef Def { get; private set; }

        private System.Action<UpgradeDef> _onPlus;

        public void Setup(UpgradeDef def, int currentLevel, System.Action<UpgradeDef> onPlus)
        {
            Def = def;
            _onPlus = onPlus;
            if (_label) _label.text = string.IsNullOrEmpty(def.displayName) ? def.type.ToString() : def.displayName;
            if (_iconImage) _iconImage.sprite = def.icon;
            SetLevel(currentLevel, canPlus: true);
            if (_plusButton)
            {
                _plusButton.onClick.RemoveAllListeners();
                _plusButton.onClick.AddListener(() => _onPlus?.Invoke(def));
            }
        }

        public void SetLevel(int level, bool canPlus)
        {
            if (_levelText) _levelText.text = $"Lv {level}/{Def.maxLevel}";
            if (_plusButton) _plusButton.interactable = canPlus;
        }
    }
}