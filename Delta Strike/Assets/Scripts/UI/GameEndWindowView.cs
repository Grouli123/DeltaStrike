// Assets/Scripts/UI/GameEndWindowView.cs
using Game.Core.App;
using Game.Core.DI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class GameEndWindowView : MonoBehaviour
    {
        private const string VictoryTitle = "Victory!";
        private const string DefeatTitle  = "Defeat";

        [Header("Refs")]
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject blocker;

        private IGameplayBlockService _block;
        private IGameStateService _state;

        private void Awake()
        {
            _block = DI.Resolve<IGameplayBlockService>();
            if (root == null) root = gameObject;
            SetVisible(false);

            if (restartButton) restartButton.onClick.AddListener(Restart);
            if (quitButton)    quitButton.onClick.AddListener(Restart); // или свой выход в меню

            if (blocker)
            {
                var btn = blocker.GetComponent<Button>() ?? blocker.AddComponent<Button>();
                btn.transition = Selectable.Transition.None;
            }

            RewireState();                          // подписка на новый сервис
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            UnwireState();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _, LoadSceneMode __)
        {
            Hide();         // на всякий — снимаем блок/курсор
            RewireState();  // перепривязка после перезагрузки
        }

        private void RewireState()
        {
            UnwireState();
            _state = DI.Resolve<IGameStateService>();
            _state.OnWin  += ShowWin;
            _state.OnLose += ShowLose;

            // БОЛЬШЕ НЕ делаем автопроверку типа "если enemies==0 → ShowWin()".
            // Ранний WIN мешал захвату ввода на старте.
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
            if (titleText)   titleText.text   = title;
            if (messageText) messageText.text = message;

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
            (root ?? gameObject).SetActive(v);
            if (blocker) blocker.SetActive(v);
        }

        private void Restart()
        {
            Hide(); // 
            var idx = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(idx);
        }
    }
}
