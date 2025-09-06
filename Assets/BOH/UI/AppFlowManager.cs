using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace BOH
{
    public class AppFlowManager : MonoBehaviour
    {
        public static AppFlowManager Instance { get; private set; }
        
        [Header("Scene Names")]
        [SerializeField] private string menuSceneName = "SCN_Menu";
        [SerializeField] private string gameplaySceneName = "SCN_Chawl";
        
        [Header("Transition")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        private TransitionFader fader;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AppFlowManager initialized");
        }

        private void Start()
        {
            fader = GetComponentInChildren<TransitionFader>();
        }

        public void LoadGameplay()
        {
            StartCoroutine(LoadSceneWithFade(gameplaySceneName));
        }

        public void LoadMainMenu()
        {
            StartCoroutine(LoadSceneWithFade(menuSceneName));
        }

        private IEnumerator LoadSceneWithFade(string sceneName)
        {
            Debug.Log($"Loading scene: {sceneName}");
            
            if (fader != null)
                yield return fader.FadeIn(fadeDuration);
            
            yield return SceneManager.LoadSceneAsync(sceneName);
            
            if (fader != null)
                yield return fader.FadeOut(fadeDuration);
        }
    }

    // ScriptRole: Manages scene transitions and app flow
    // RelatedScripts: TransitionFader, MainMenu, GameStateManager
    // SendsTo: Unity Scene Manager
}