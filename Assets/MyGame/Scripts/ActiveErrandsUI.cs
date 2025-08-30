using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Obvious.Soap;

namespace BOH
{
    public class ActiveErrandsUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI errandsListText;
        [SerializeField] private GameObject errandsPanel;
        [SerializeField] private string emptyText = "No active errands";
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onErrandCompleted;
        [SerializeField] private float updateInterval = 1f;
        
        private ErrandSystem errandSystem;
        private float updateTimer;

        private void OnEnable()
        {
            if (onErrandCompleted != null) 
                onErrandCompleted.OnRaised += UpdateErrandsList;;
        }

        private void OnDisable()
        {
            if (onErrandCompleted != null) 
                onErrandCompleted.OnRaised -= UpdateErrandsList;
        }

        private void Start()
        {
            errandSystem = FindObjectOfType<ErrandSystem>();
            UpdateErrandsList();
        }

        private void Update()
        {
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0;
                UpdateErrandsList();
            }
        }

        private void UpdateErrandsList()
        {
            if (errandSystem == null || errandsListText == null) return;
            
            var activeErrands = errandSystem.GetActiveErrands();
            
            if (activeErrands.Count == 0)
            {
                errandsListText.text = emptyText;
            }
            else
            {
                string listText = "<b>Active Errands:</b>\n";
                foreach (var errand in activeErrands)
                {
                    string timeLimit = "";
                    if (errand.errandData.type == ErrandSO.ErrandType.Strict)
                    {
                        timeLimit = $" (by {errand.errandData.endHour}:00)";
                    }
                    listText += $"• {errand.errandData.errandTitle}{timeLimit}\n";
                }
                errandsListText.text = listText;
            }
        }
    }

    // ScriptRole: Displays list of active errands
    // RelatedScripts: ErrandSystem
    // UsesSO: GameEventSO
    // ReceivesFrom: ErrandSystem via events
}