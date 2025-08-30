using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;
using UnityEngine.UI;
using TMPro;

namespace BOH
{
    public class DialogueSystem : MonoBehaviour
    {
        [Header("Dialogue Database")]
        [SerializeField] private List<DialogueGateSO> dialogueGates = new List<DialogueGateSO>();
        
        [Header("UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onDialogueStart;
        [SerializeField] private ScriptableEventNoParam onDialogueEnd;
        
        private InventorySystem inventorySystem;
        private GiftingSystem giftingSystem;
        private string currentNPCId;
        private ContactSO currentContact;
        private List<GameObject> activeChoiceButtons = new List<GameObject>();

        private void Start()
        {
            inventorySystem = FindFirstObjectByType<InventorySystem>();
            giftingSystem = FindFirstObjectByType<GiftingSystem>();
            
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        public void StartDialogue(ContactSO contact)
        {
            if (contact == null) return;
            
            currentContact = contact;
            currentNPCId = contact.contactId;
            
            Debug.Log($"Starting dialogue with {contact.displayName}");
            ShowDialogue(contact.displayName, GetGreeting(contact));
            
            // Check for special item options
            CheckSpecialItemOptions();
            
            onDialogueStart?.Raise();
            Time.timeScale = 0f;
        }

        private void CheckSpecialItemOptions()
        {
            ClearChoices();
            
            // Get equipped item
            var equippedItem = inventorySystem?.GetEquippedItem();
            if (equippedItem == null) return;
            
            string equipTag = equippedItem.itemData.equipTag;
            
            // Find matching dialogue gates
            var validGates = dialogueGates
                .Where(g => g.requiredEquipTag == equipTag)
                .Where(g => IsValidForNPC(g, currentContact))
                .OrderBy(g => g.priority)
                .ToList();
            
            // Create choice buttons
            foreach (var gate in validGates)
            {
                CreateGiftChoice(gate, equippedItem.itemData);
            }
            
            // Add default choices
            AddDefaultChoices();
        }

        private bool IsValidForNPC(DialogueGateSO gate, ContactSO contact)
        {
            // Check NPC filter
            if (gate.npcFilter.Count > 0 && !gate.npcFilter.Contains(contact))
                return false;
            
            // Check tags
            if (gate.npcTags.Count > 0)
            {
                bool hasTag = gate.npcTags.Any(t => contact.tags.Contains(t));
                if (!hasTag) return false;
            }
            
            return true;
        }

        private void CreateGiftChoice(DialogueGateSO gate, ItemSO item)
        {
            if (choiceButtonPrefab == null || choicesContainer == null) return;
            
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            string offerText = string.Format(gate.offerText, item.displayName);
            buttonText.text = offerText;
            
            button.onClick.AddListener(() => OnGiftChoice(item.itemId, currentNPCId));
            
            activeChoiceButtons.Add(buttonObj);
        }

        private void OnGiftChoice(string itemId, string recipientId)
        {
            Debug.Log($"Offering {itemId} to {recipientId}");
            
            if (inventorySystem.GiftItem(itemId, recipientId))
            {
                ShowDialogue(currentContact.displayName, GetGiftResponse(currentContact, true));
            }
            else
            {
                ShowDialogue(currentContact.displayName, GetGiftResponse(currentContact, false));
            }
            
            Invoke(nameof(EndDialogue), 2f);
        }

        private void AddDefaultChoices()
        {
            // Add standard dialogue options
            CreateChoice("How are you?", () => {
                ShowDialogue(currentContact.displayName, "I'm doing well, thank you.");
                Invoke(nameof(EndDialogue), 2f);
            });
            
            CreateChoice("Goodbye", () => EndDialogue());
        }

        private void CreateChoice(string text, System.Action action)
        {
            if (choiceButtonPrefab == null || choicesContainer == null) return;
            
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = text;
            button.onClick.AddListener(() => action());
            
            activeChoiceButtons.Add(buttonObj);
        }

        private void ShowDialogue(string speaker, string text)
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                
                if (speakerText != null)
                    speakerText.text = speaker;
                
                if (dialogueText != null)
                    dialogueText.text = text;
            }
        }

        private void ClearChoices()
        {
            foreach (var button in activeChoiceButtons)
            {
                Destroy(button);
            }
            activeChoiceButtons.Clear();
        }

        public void EndDialogue()
        {
            Debug.Log("Ending dialogue");
            
            ClearChoices();
            
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
            
            currentContact = null;
            currentNPCId = null;
            
            Time.timeScale = 1f;
            onDialogueEnd?.Raise();
        }

        private string GetGreeting(ContactSO contact)
        {
            // Context-aware greetings
            switch (contact.role)
            {
                case "Bookseller":
                    return "Looking for something to read?";
                case "Nurse":
                    return "Oh, hello! Are you here to help?";
                case "Teacher":
                    return "Good to see you again.";
                default:
                    return "Hello there.";
            }
        }

        private string GetGiftResponse(ContactSO contact, bool success)
        {
            if (success)
            {
                switch (contact.role)
                {
                    case "Bookseller":
                        return "Oh my! This is beautiful. Thank you so much!";
                    case "Poor Man":
                        return "Bless you, child. Your kindness means everything.";
                    case "Shopkeeper":
                        return "I can give you a fair price for this.";
                    default:
                        return "Thank you for this.";
                }
            }
            else
            {
                return "I appreciate the thought.";
            }
        }
    }

    // ScriptRole: Manages dialogue flow and special item interactions
    // RelatedScripts: InventorySystem, GiftingSystem, NPCInteraction
    // UsesSO: DialogueGateSO, ContactSO, GameEventSO
    // ReceivesFrom: NPCInteraction triggers
    // SendsTo: GiftingSystem, UI
}