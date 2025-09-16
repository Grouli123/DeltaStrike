using Game.Core.Config;
using TMPro;                 
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Upgrade
{
    public sealed class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;               
        [SerializeField] private TMP_Text levelText;           
        [SerializeField] private Button plusButton;
        [SerializeField] private Image iconImage;

        public UpgradeDef Def { get; private set; }

        private System.Action<UpgradeDef> _onPlus;

        public void Setup(UpgradeDef def, int currentLevel, System.Action<UpgradeDef> onPlus)
        {
            Def = def;
            _onPlus = onPlus;
            if (label) label.text = string.IsNullOrEmpty(def.displayName) ? def.type.ToString() : def.displayName;
            if (iconImage) iconImage.sprite = def.icon;
            SetLevel(currentLevel, canPlus: true);
            if (plusButton)
            {
                plusButton.onClick.RemoveAllListeners();
                plusButton.onClick.AddListener(() => _onPlus?.Invoke(def));
            }
        }

        public void SetLevel(int level, bool canPlus)
        {
            if (levelText) levelText.text = $"Lv {level}/{Def.maxLevel}";
            if (plusButton) plusButton.interactable = canPlus;
        }
    }
}