using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BOH
{
    public class JournalPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject journalPanel;
        [SerializeField] private TextMeshProUGUI journalTitle;
        [SerializeField] private TextMeshProUGUI journalContent;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button endDayButton;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onDayEnd;
        
        private GameStateManager gameStateManager;

        private void OnEnable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised+= ShowJournal;
        }

        private void OnDisable()
        {
            if (onDayEnd != null) onDayEnd.OnRaised -= ShowJournal;
        }

        private void Start()
        {
            gameStateManager = FindFirstObjectByType<GameStateManager>();
            
            if (journalTitle != null)
                journalTitle.text = "Day's End";
            
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseJournal);
            
            if (endDayButton != null)
                endDayButton.onClick.AddListener(OnEndDayClicked);
            
            if (journalPanel != null)
                journalPanel.SetActive(false);
        }

        private void ShowJournal()
        {
            Debug.Log("Showing journal");
            
            if (journalPanel != null)
                journalPanel.SetActive(true);
            
            if (journalContent != null)
                journalContent.text = "Today you helped your community.\n\n[Journal stub - will show errands and outcomes]";
        }

        private void CloseJournal()
        {
            Debug.Log("Closing journal");
            
            if (journalPanel != null)
                journalPanel.SetActive(false);
            
            
            
            gameStateManager?.StartDay();
        }

        private void OnEndDayClicked()
        {
            gameStateManager?.EndDay();
        }
    }

    // ScriptRole: Journal UI panel controller (stub)
    // RelatedScripts: GameStateManager
    // UsesSO: GameEventSO
    // ReceivesFrom: GameStateManager day end event
    // SendsTo: GameStateManager for day control
}