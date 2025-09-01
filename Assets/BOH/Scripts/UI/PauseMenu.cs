using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BOH
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private TextMeshProUGUI pauseTitle;
        [SerializeField] private Button endDayButton;
        [SerializeField] private bool isEndDayManual = false;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onPauseToggle;
        //[SerializeField] private ScriptableEventNoParam onEndDayClicked;
        
        [SerializeField]private GameStateManager gameStateManager;

        private void OnEnable()
        {
            if (onPauseToggle != null) onPauseToggle.OnRaised += TogglePauseUI;
        }

        private void OnDisable()
        {
            if (onPauseToggle != null) onPauseToggle.OnRaised -= TogglePauseUI;
        }

        private void Start()
        {
            gameStateManager = FindFirstObjectByType<GameStateManager>();
            
            if (pauseTitle != null)
                pauseTitle.text = "PAUSED";
            
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
            if (endDayButton != null)
            {
                endDayButton.onClick.AddListener(OnEndDayClicked);
            }
            
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        private void TogglePauseUI()
        {
            Debug.Log("Toggle pause UI");
            if (pausePanel != null && gameStateManager != null)
                pausePanel.SetActive(gameStateManager.IsPaused());
        }

        private void OnResumeClicked()
        {
            Debug.Log("Resume clicked");
            gameStateManager?.TogglePause();
        }

        private void OnMenuClicked()
        {
            Debug.Log("Return to menu clicked");
            gameStateManager?.ReturnToMenu();
        }
        
        private void OnEndDayClicked()
        {
            if (!isEndDayManual)
            {
                return; 
            }
            Debug.Log("End day clicked");
            pausePanel.SetActive(false);
            gameStateManager?.EndDay();
        }
    }

    // ScriptRole: Pause menu UI controller
    // RelatedScripts: GameStateManager
    // UsesSO: GameEventSO
    // ReceivesFrom: GameStateManager pause events
    // SendsTo: GameStateManager for pause/menu actions
}