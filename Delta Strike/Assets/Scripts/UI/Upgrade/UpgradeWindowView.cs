using System.Collections.Generic;
using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;
using TMPro;                 
using UnityEngine;
using UnityEngine.UI;
using Game.Core.App;  

namespace Game.UI.Upgrade
{
    public sealed class UpgradeWindowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text pointsText;          
        [SerializeField] private Transform itemsRoot;
        [SerializeField] private UpgradeItemView itemPrefab;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button closeButton;
        
        [Header("UX")]
        [SerializeField] private GameObject blocker;          
        [SerializeField] private bool closeOnEsc = true;      
        [SerializeField] private bool closeOnBlockerClick = true;

        private UpgradeConfig _cfg;
        private IProgressService _progress;
        private readonly Dictionary<StatType, int> _pendingLevels = new();
        private int _tempPoints;

        private bool _cursorWasLocked;
        
        private IGameplayBlockService _block;

        private void Awake()
        {
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            _block = DI.Resolve<IGameplayBlockService>(); 

            Build();
            if (applyButton) applyButton.onClick.AddListener(Apply);
            if (cancelButton) cancelButton.onClick.AddListener(Cancel);
            
            if (closeButton)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Close);
            }

            if (blocker != null && closeOnBlockerClick)
            {
                var btn = blocker.GetComponent<Button>();
                if (btn == null) btn = blocker.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Close);
            }
        }

        private void OnEnable()
        {
            ResetPending();
            Refresh();

            if (blocker) blocker.SetActive(true);

            _block?.SetBlocked(true);

#if !UNITY_ANDROID && !UNITY_IOS
            _cursorWasLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
#endif
        }

        private void Update()
        {
#if !UNITY_ANDROID && !UNITY_IOS
            if (closeOnEsc && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
                Close();
#endif
        }

        private void OnDisable()
        {
            if (blocker) blocker.SetActive(false);

            _block?.SetBlocked(false);

#if !UNITY_ANDROID && !UNITY_IOS
            if (_cursorWasLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
#endif
        }

        private void Build()
        {
            foreach (Transform c in itemsRoot) Destroy(c.gameObject);
            foreach (var def in _cfg.Upgrades)
            {
                var item = Instantiate(itemPrefab, itemsRoot);
                item.Setup(def, GetLevel(def.type), OnPlusClicked);
            }
        }

        private void ResetPending()
        {
            _pendingLevels.Clear();
            foreach (var def in _cfg.Upgrades)
                _pendingLevels[def.type] = GetLevel(def.type);

            _tempPoints = _progress.Points;
        }

        private int GetLevel(StatType stat) => _progress.GetLevel(stat);

        private bool HasPendingChanges()
        {
            bool changed = _tempPoints != _progress.Points;
            foreach (var def in _cfg.Upgrades)
            {
                if (_pendingLevels[def.type] != _progress.GetLevel(def.type))
                {
                    changed = true;
                    break;
                }
            }
            return changed;
        }
        
        private void Refresh()
        {
            if (pointsText) pointsText.text = $"Points: {_tempPoints}";

            foreach (Transform c in itemsRoot)
            {
                var item = c.GetComponent<UpgradeItemView>();
                if (item == null) continue;
                var def = item.Def;
                int lvl = _pendingLevels[def.type];
                bool canPlus = _tempPoints > 0 && lvl < def.maxLevel;
                item.SetLevel(lvl, canPlus);              
            }

            if (applyButton)  applyButton.interactable  = HasPendingChanges();
            if (cancelButton) cancelButton.interactable = HasPendingChanges();
        }

        private void OnPlusClicked(UpgradeDef def)
        {
            int lvl = _pendingLevels[def.type];
            if (_tempPoints <= 0 || lvl >= def.maxLevel) return;

            _pendingLevels[def.type] = lvl + 1;
            _tempPoints--;
            Refresh();
        }

        private void Apply()
        {
            foreach (var kv in _pendingLevels)
                _progress.SetLevel(kv.Key, kv.Value);

            int delta = _tempPoints - _progress.Points;
            if (delta != 0) _progress.AddPoints(delta); else _progress.Save();

            var playerHealth = FindObjectOfType<Game.Player.PlayerHealth>();
            if (playerHealth != null) playerHealth.RecalculateFromStats();

            ResetPending();
            Refresh();
            Close();
        }

        private void Cancel()
        {
            ResetPending();
            Refresh();
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}