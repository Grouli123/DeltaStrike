using System.Collections.Generic;
using Game.Core.Config;
using Game.Core.DI;
using Game.Systems.Progress;
using TMPro;                 
using UnityEngine;
using UnityEngine.UI;
using Game.Core.App;
using UnityEngine.Serialization;

namespace Game.UI.Upgrade
{
    public sealed class UpgradeWindowView : MonoBehaviour
    {
        [Header("Pause")]
        [SerializeField] private bool pauseWithTimeScale = true;
        private float _savedTimeScale = 1f;
        private bool _pausedByMe;
        
        [SerializeField] private TMP_Text _pointsText;   
        [SerializeField] private Transform _itemsRoot;
        [SerializeField] private UpgradeItemView _itemPrefab;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _closeButton;
        
        [Header("UX")]
        [SerializeField] private GameObject _blocker; 
        [SerializeField] private bool _closeOnEsc = true;   
        [SerializeField] private bool _closeOnBlockerClick = true;

        private UpgradeConfig _cfg;
        private IProgressService _progress;
        private IGameplayBlockService _block;
        private readonly Dictionary<StatType, int> _pendingLevels = new();
        private int _tempPoints;
        private const int UpgradeStep = 1;

        private bool _cursorWasLocked;
        
        private void Awake()
        {
            _cfg = DI.Resolve<UpgradeConfig>();
            _progress = DI.Resolve<IProgressService>();
            _block = DI.Resolve<IGameplayBlockService>(); 

            Build();
            if (_applyButton) _applyButton.onClick.AddListener(Apply);
            if (_cancelButton) _cancelButton.onClick.AddListener(Cancel);
            
            if (_closeButton)
            {
                _closeButton.onClick.RemoveAllListeners();
                _closeButton.onClick.AddListener(Close);
            }

            if (_blocker != null && _closeOnBlockerClick)
            {
                var btn = _blocker.GetComponent<Button>();
                if (btn == null) btn = _blocker.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Close);
            }
        }

        private void OnEnable()
        {
            ResetPending();
            Refresh();

            if (_blocker) _blocker.SetActive(true);

            _block?.SetBlocked(true);
            
            if (pauseWithTimeScale)
            {
                _savedTimeScale = Time.timeScale;
                Time.timeScale  = 0f;
                _pausedByMe     = true;
            }
            
#if !UNITY_ANDROID && !UNITY_IOS
            _cursorWasLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
#endif
        }

        private void Update()
        {
#if !UNITY_ANDROID && !UNITY_IOS
            if (_closeOnEsc && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
                Close();
#endif
        }

        private void OnDisable()
        {
            if (_blocker) _blocker.SetActive(false);

            _block?.SetBlocked(false);

            if (_pausedByMe)
            {
                Time.timeScale = _savedTimeScale;
                _pausedByMe    = false;
            }
            
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
            foreach (Transform c in _itemsRoot) Destroy(c.gameObject);
            foreach (var def in _cfg.Upgrades)
            {
                var item = Instantiate(_itemPrefab, _itemsRoot);
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
            if (_pointsText) _pointsText.text = $"Points: {_tempPoints}";

            foreach (Transform c in _itemsRoot)
            {
                var item = c.GetComponent<UpgradeItemView>();
                if (item == null) continue;
                var def = item.Def;
                int lvl = _pendingLevels[def.type];
                bool canPlus = _tempPoints > 0 && lvl < def.maxLevel;
                item.SetLevel(lvl, canPlus);              
            }

            if (_applyButton)  _applyButton.interactable  = HasPendingChanges();
            if (_cancelButton) _cancelButton.interactable = HasPendingChanges();
        }

        private void OnPlusClicked(UpgradeDef def)
        {
            int lvl = _pendingLevels[def.type];
            if (_tempPoints <= 0 || lvl >= def.maxLevel) return;

            _pendingLevels[def.type] = lvl + UpgradeStep;
            _tempPoints--;
            Refresh();
        }

        private void Apply()
        {
            foreach (var kv in _pendingLevels)
                _progress.SetLevel(kv.Key, kv.Value);

            int delta = _tempPoints - _progress.Points;
            if (delta != 0) _progress.AddPoints(delta);
            else _progress.Save();

            if (DI.TryResolve<Game.Core.App.IPlayerRef>(out var pref) && pref.Health != null)
                pref.Health.RecalculateFromStats();

            gameObject.SetActive(false); 
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