using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Conversa.Runtime.Events;

namespace BOH.Conversa
{
    public class MyUIController : MonoBehaviour
    {
        
        [SerializeField] private GameObject messageWindow;
        [SerializeField] private GameObject choiceWindow;
        [SerializeField] private Image avatarImage;
        [SerializeField] private TextMeshProUGUI actorNameText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button nextMessageButton;

        [SerializeField] private GameObject choiceOptionButtonPrefab;

        // Exactly two fixed choice buttons to avoid instantiation/destruction
        [Header("Choice Buttons (2 fixed)")]
        [SerializeField] private Button choiceButtonA;
        [SerializeField] private Button choiceButtonB;

        public void ShowMessage(string actor, string message, Sprite avatar, Action onContinue)
        {
            if (messageWindow != null) messageWindow.SetActive(true);
            if (choiceWindow != null) choiceWindow.SetActive(false);
            if (choiceButtonA != null) choiceButtonA.gameObject.SetActive(false);
            if (choiceButtonB != null) choiceButtonB.gameObject.SetActive(false);

            UpdateImage(avatar);
            if (actorNameText != null) actorNameText.text = actor;
            if (messageText != null) messageText.text = message;

            if (nextMessageButton != null)
            {
                nextMessageButton.gameObject.SetActive(true);
                nextMessageButton.interactable = true;
                nextMessageButton.onClick.RemoveAllListeners();
                nextMessageButton.onClick.AddListener(() => { onContinue?.Invoke(); });
            }
        }
        
        public void ShowChoice(string actor, string message, Sprite avatar, List<Option> options)
        {
            if (messageWindow != null) messageWindow.SetActive(true);

            UpdateImage(avatar);
            if (actorNameText != null) actorNameText.text = actor;
            if (messageText != null) messageText.text = message;
            if (nextMessageButton != null)
            {
                // Hide the next button to avoid occluding the choices and blocking raycasts
                nextMessageButton.onClick.RemoveAllListeners();
                nextMessageButton.gameObject.SetActive(false);
            }

            if (choiceWindow != null)
            {
                choiceWindow.SetActive(true);
                var cg = choiceWindow.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 1f;
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                }
            }
            else { Debug.LogWarning("[ConversaUI] choiceWindow is not assigned on MyUIController"); }

            // Configure exactly two fixed buttons for choices (no instantiation)
            EnsureChoiceButtonsCached();
            int count = options != null ? options.Count : 0;
            if (count > 2)
                Debug.LogWarning("[ConversaUI] More than 2 options provided; ignoring extras.");

            // Button A
            if (choiceButtonA != null)
            {
                if (count >= 1)
                {
                    SetupChoiceButton(choiceButtonA, options[0]);
                    choiceButtonA.gameObject.SetActive(true);
                }
                else
                {
                    choiceButtonA.onClick.RemoveAllListeners();
                    choiceButtonA.gameObject.SetActive(false);
                }
            }
            // Button B
            if (choiceButtonB != null)
            {
                if (count >= 2)
                {
                    SetupChoiceButton(choiceButtonB, options[1]);
                    choiceButtonB.gameObject.SetActive(true);
                }
                else
                {
                    choiceButtonB.onClick.RemoveAllListeners();
                    choiceButtonB.gameObject.SetActive(false);
                }
            }
        }

        private void EnsureChoiceButtonsCached()
        {
            if ((choiceButtonA == null || choiceButtonB == null) && choiceWindow != null)
            {
                var buttons = choiceWindow.GetComponentsInChildren<Button>(true);
                if (choiceButtonA == null && buttons.Length > 0) choiceButtonA = buttons[0];
                if (choiceButtonB == null && buttons.Length > 1) choiceButtonB = buttons[1];
            }
        }

        private void SetupChoiceButton(Button btn, Option option)
        {
            if (btn == null || option == null) return;
            // Set text with TMP if present, else legacy Text
            var tmpText = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (tmpText != null) tmpText.text = option.Message;
            else
            {
                var legacy = btn.GetComponentInChildren<UnityEngine.UI.Text>(true);
                if (legacy != null) legacy.text = option.Message;
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => option.Advance());
            btn.interactable = true;
            EnsureUIEnabled(btn.gameObject);
        }


        private static void EnsureUIEnabled(GameObject root)
        {
            if (root == null) return;
            root.SetActive(true);

            // Enable CanvasGroups and allow interactions
            var groups = root.GetComponentsInChildren<CanvasGroup>(true);
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].alpha = Mathf.Max(groups[i].alpha, 1f);
                groups[i].interactable = true;
                groups[i].blocksRaycasts = true;
            }

            // Enable UI graphics
            var graphics = root.GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].enabled = true;
                var c = graphics[i].color;
                if (c.a <= 0f) graphics[i].color = new Color(c.r, c.g, c.b, 1f);
            }

            // Enable TextMeshPro components
            var tmpTexts = root.GetComponentsInChildren<TMPro.TMP_Text>(true);
            for (int i = 0; i < tmpTexts.Length; i++)
            {
                tmpTexts[i].enabled = true;
                var c = tmpTexts[i].color;
                if (c.a <= 0f) tmpTexts[i].color = new Color(c.r, c.g, c.b, 1f);
            }

            // Ensure buttons/selectables are interactable
            var selectables = root.GetComponentsInChildren<UnityEngine.UI.Selectable>(true);
            for (int i = 0; i < selectables.Length; i++)
            {
                selectables[i].interactable = true;
                selectables[i].enabled = true;
            }
        }

        private void UpdateImage(Sprite sprite)
        {
            if (avatarImage == null) return;
            avatarImage.enabled = sprite != null;
            avatarImage.sprite = sprite;
        }
        
        public void Hide()
        {
            messageWindow.SetActive(false);
            choiceWindow.SetActive(false);
        }


        public void Show()
        {
            messageWindow.SetActive(true);
        }
    }
}
