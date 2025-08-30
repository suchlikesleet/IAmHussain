using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BOH
{
    public class TransitionFader : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private Color fadeColor = Color.black;

        private void Awake()
        {
            if (fadeImage == null)
            {
                GameObject fadeObj = new GameObject("FadeImage");
                fadeObj.transform.SetParent(transform, false);
                fadeImage = fadeObj.AddComponent<Image>();
                
                RectTransform rect = fadeImage.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }
            
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
            fadeImage.raycastTarget = true;
        }

        public IEnumerator FadeIn(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsed / duration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }
        }

        public IEnumerator FadeOut(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - Mathf.Clamp01(elapsed / duration);
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }
        }
    }

    // ScriptRole: Handles fade transitions between scenes
    // RelatedScripts: AppFlowManager
}