using UnityEngine;
using UnityEngine.InputSystem;
using BOH.Conversa;
using Conversa.Runtime;

namespace BOH
{
    public class NPCInteraction : MonoBehaviour
    {
        [Header("NPC Data")]
        [SerializeField] private ContactSO contactData;
        [SerializeField] private float interactionRadius = 2f;
        
        [Header("Visual")]
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private GameObject specialItemIndicator;
        
        private Transform playerTransform;
        private InventorySystem inventorySystem;
        [Header("Conversa")]
        [SerializeField] private MyConversaController conversaController;
        [SerializeField] private Conversation defaultConversation;
        [SerializeField] private ConversationSelector selector;
        private bool playerInRange = false;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            
            if (conversaController == null)
                conversaController = FindFirstObjectByType<MyConversaController>();
            if (selector == null)
                selector = GetComponent<ConversationSelector>();
            inventorySystem = FindFirstObjectByType<InventorySystem>();
            
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            
            if (specialItemIndicator != null)
                specialItemIndicator.SetActive(false);
        }

        private void Update()
        {
            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= interactionRadius;
            
            if (playerInRange != wasInRange)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(playerInRange);
                
                // Show special indicator if player has relevant equipped item
                UpdateSpecialIndicator();
            }
            
            // Handle interaction input
            if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Interact();
            }
        }

        //Callfrom inputhandler.cs using UnityEvent
        public void HandleInteraction()
        {
            if (playerInRange)
            {
                Interact();
            }
        }

        private void UpdateSpecialIndicator()
        {
            if (specialItemIndicator == null || !playerInRange) return;
            
            // Check if player has equipped item that this NPC can receive
            string equippedTag = inventorySystem?.GetEquippedTag() ?? "";
            bool hasRelevantItem = !string.IsNullOrEmpty(equippedTag);
            
            specialItemIndicator.SetActive(hasRelevantItem);
        }

        private void Interact()
        {
            // Prefer a context-selected conversation; fall back to default
            var convo = selector != null ? selector.Select() : defaultConversation;
            if (convo == null)
            {
                Debug.LogWarning($"[NPCInteraction] No conversation assigned/selected on {name}");
                return;
            }
            if (conversaController == null)
            {
                Debug.LogWarning("[NPCInteraction] MyConversaController not found in scene");
                return;
            }
            conversaController.StartConversation(convo);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }

    // ScriptRole: NPC interaction handler with special item awareness
    // RelatedScripts: InventorySystem, MyConversaController, ConversationSelector
    // UsesSO: ContactSO
    // SendsTo: MyConversaController
}
