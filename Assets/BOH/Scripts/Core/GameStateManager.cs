using Obvious.Soap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BOH
{
    public class GameStateManager : MonoBehaviour
    {
        public enum GamePhase
        {
            Boot,
            InDay,
            Journal,
            Paused
        }

        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onDayStart;
        [SerializeField] private ScriptableEventNoParam onDayEnd;
        [SerializeField] private ScriptableEventNoParam onPauseToggle;
        
        [Header("Current State")]
        [SerializeField] private GamePhase currentPhase = GamePhase.Boot;
        
        private GamePhase previousPhase;
        private bool isPaused = false;

        private void Start()
        {
            Debug.Log("GameStateManager starting...");
            StartDay();
        }
        

        public void StartDay()
        {
            Debug.Log("Starting new day");
            currentPhase = GamePhase.InDay;
            onDayStart?.Raise();
            Time.timeScale = 1f;
        }

        public void EndDay()
        {
            if (currentPhase == GamePhase.InDay || currentPhase == GamePhase.Paused)
            {
                Debug.Log("Ending day");
                currentPhase = GamePhase.Journal;
                onDayEnd?.Raise();
                Time.timeScale = 0f;
            }
            
            
        }

        public void TogglePause()
        {
            if (currentPhase == GamePhase.Journal) return;
            
            isPaused = !isPaused;
            
            if (isPaused)
            {
                previousPhase = currentPhase;
                currentPhase = GamePhase.Paused;
                Time.timeScale = 0f;
            }
            else
            {
                currentPhase = previousPhase;
                Time.timeScale = 1f;
            }
            
            Debug.Log($"Pause toggled: {isPaused}");
            onPauseToggle?.Raise();
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            AppFlowManager.Instance?.LoadMainMenu();
        }

        public GamePhase GetCurrentPhase() => currentPhase;
        public bool IsPaused() => isPaused;
    }

    // ScriptRole: Manages game state phases
    // RelatedScripts: TimeSystem, ResourceSystem, JournalPanel, PauseMenu
    // UsesSO: GameEventSO
    // SendsTo: All systems via events
}