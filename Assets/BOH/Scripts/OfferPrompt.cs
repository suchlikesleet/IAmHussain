using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace BOH
{
    public class OfferPrompt : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI sourceText;
        [SerializeField] private TextMeshProUGUI offerText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private TextMeshProUGUI acceptButtonText;
        [SerializeField] private Button declineButton;
        [SerializeField] private TextMeshProUGUI declineButtonText;
        [SerializeField] private float autoHideDelay = 10f;
        
        private Action onAccept;
        private Action onDecline;
        private float hideTimer;

        private void Start()
        {
            if (acceptButton != null)
                acceptButton.onClick.AddListener(OnAcceptClicked);
            
            if (declineButton != null)
                declineButton.onClick.AddListener(OnDeclineClicked);
            
            HidePrompt();
        }

        private void Update()
        {
            if (promptPanel != null && promptPanel.activeSelf)
            {
                hideTimer -= Time.deltaTime;
                if (hideTimer <= 0)
                {
                    OnDeclineClicked(); // Auto-decline if no response
                }
            }
        }

        public void ShowOffer(string offer, string source, string acceptText, string declineText,
            Action onAcceptCallback, Action onDeclineCallback)
        {
            if (promptPanel == null) return;
            
            offerText.text = offer;
            sourceText.text = $"From: {source}";
            acceptButtonText.text = acceptText;
            declineButtonText.text = declineText;
            
            onAccept = onAcceptCallback;
            onDecline = onDeclineCallback;
            
            promptPanel.SetActive(true);
            hideTimer = autoHideDelay;
            
            // Pause game while showing offer
            Time.timeScale = 0f;
        }

        private void OnAcceptClicked()
        {
            Debug.Log("Offer accepted");
            onAccept?.Invoke();
            HidePrompt();
        }

        private void OnDeclineClicked()
        {
            Debug.Log("Offer declined");
            onDecline?.Invoke();
            HidePrompt();
        }

        private void HidePrompt()
        {
            if (promptPanel != null)
                promptPanel.SetActive(false);
            
            Time.timeScale = 1f;
            onAccept = null;
            onDecline = null;
        }
    }

    // ScriptRole: UI for accepting/declining errand offers
    // RelatedScripts: TriggerSystem
    // SendsTo: TriggerSystem callbacks
}