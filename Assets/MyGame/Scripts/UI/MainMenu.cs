using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BOH
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI quoteText;
        
        [Header("Content")]
        [SerializeField] private string gameTitle = "Blessings of Humanity";
        [SerializeField] private string[] quotes = {
            "Every little act of kindness matters.",
            "Small steps lead to great journeys.",
            "In helping others, we find ourselves."
        };

        private void Start()
        {
            if (titleText != null)
                titleText.text = gameTitle;
            
            if (quoteText != null && quotes.Length > 0)
                quoteText.text = quotes[Random.Range(0, quotes.Length)];
            
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnPlayClicked()
        {
            Debug.Log("Play button clicked");
            AppFlowManager.Instance?.LoadGameplay();
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit button clicked");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }

    // ScriptRole: Main menu UI controller
    // RelatedScripts: AppFlowManager
    // SendsTo: AppFlowManager for scene loading
}