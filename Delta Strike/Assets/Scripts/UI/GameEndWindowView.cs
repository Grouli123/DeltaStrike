using Game.Core.App;
using Game.Core.DI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class GameEndWindowView : MonoBehaviour
    {
        private const string VictoryTitle = "Victory!";
        private const string DefeatTitle  = "Defeat";

        [Header("Refs")]
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private GameObject _blocker;

        private IGameplayBlockService _block;
        private IGameStateService _state;

        private void Awake()
        {
            _block = DI.Resolve<IGameplayBlockService>();
            if (_root == null) _root = gameObject;
            SetVisible(false);

            if (_restartButton) _restartButton.onClick.AddListener(Restart);
            if (_quitButton)    _quitButton.onClick.AddListener(QuitGame);

            if (_blocker)
            {
                var btn = _blocker.GetComponent<Button>() ?? _blocker.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
            }

            RewireState();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            UnwireState();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _, LoadSceneMode __)
        {
            Hide();         
            RewireState();  
        }

        private void RewireState()
        {
            UnwireState();
            _state = DI.Resolve<IGameStateService>();
            _state.OnWin  += ShowWin;
            _state.OnLose += ShowLose;

        }

        private void UnwireState()
        {
            if (_state != null)
            {
                _state.OnWin  -= ShowWin;
                _state.OnLose -= ShowLose;
                _state = null;
            }
        }

        private void ShowWin()  => Show(VictoryTitle, "All enemies eliminated.");
        private void ShowLose() => Show(DefeatTitle,  "You died.");

        private void Show(string title, string message)
        {
            if (_titleText)   _titleText.text   = title;
            if (_messageText) _messageText.text = message;

            SetVisible(true);
            _block?.SetBlocked(true);
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
#endif
        }

        private void Hide()
        {
            SetVisible(false);
            _block?.SetBlocked(false);
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
#endif
        }

        private void SetVisible(bool v)
        {
            (_root ?? gameObject).SetActive(v);
            if (_blocker) _blocker.SetActive(v);
        }

        private void Restart()
        {
            Hide(); 
            var idx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(idx);
        }
        
        private void QuitGame()
        {
            Hide();

            if (Game.Core.DI.DI.TryResolve<Game.Systems.Progress.IProgressService>(out var progress))
                progress.Save();
            PlayerPrefs.Save();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
    try
    {
        Application.Quit();

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            activity.Call<bool>("moveTaskToBack", true);
        }
    }
    catch { Application.Quit(); }
#else
    Application.Quit();
#endif
        }
    }
}
