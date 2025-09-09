using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;
using Conversa.Runtime;

namespace BOH
{
    public class TriggerSystem : MonoBehaviour
    {
        [Header("Trigger Database")]
        [SerializeField] private List<TriggerSO> allTriggers = new List<TriggerSO>();
        
        [Header("Events")]
        [SerializeField] private ScriptableEventNoParam onDayStart;
        
        [Header("References")]
        [SerializeField] private OfferPrompt offerPromptUI;
        [SerializeField] private BOH.Conversa.MyConversaController conversaController;
        
        [Header("State")]
        [SerializeField] private List<string> triggeredToday = new List<string>();
        [SerializeField] private int currentDay = 0; // Will become 1 on first onDayStart
        
        private TimeSystem timeSystem;
        private ErrandSystem errandSystem;
        private InventorySystem inventorySystem; // Add this
        private bool phoneCheckScheduled = false;

        private void OnEnable()
        {
            if (onDayStart != null) onDayStart.OnRaised += OnNewDay;
        }

        private void OnDisable()
        {
            if (onDayStart != null) onDayStart.OnRaised -= OnNewDay;
        }

        private void Start()
        {
            // Resolve references (prefer GameServices, then auto-find)
            timeSystem = GameServices.Time ?? timeSystem ?? FindFirstObjectByType<TimeSystem>();
            errandSystem = GameServices.Errands ?? errandSystem ?? FindFirstObjectByType<ErrandSystem>();
            inventorySystem = GameServices.Inventory ?? inventorySystem ?? FindFirstObjectByType<InventorySystem>();
            if (conversaController == null)
                conversaController = FindFirstObjectByType<BOH.Conversa.MyConversaController>();
                

            // Inform about deprecated NPC triggers
            int npcCount = allTriggers != null ? allTriggers.Count(t => t != null && t.type == TriggerSO.TriggerType.NPC) : 0;
            if (npcCount > 0)
            {
                Debug.LogWarning($"[TriggerSystem] {npcCount} NPC triggers present but ignored. Use ConversationStarter or ConversationZoneStarter on NPCs.");
            }

            Debug.Log($"TriggerSystem initialized with {allTriggers.Count} triggers (Phone/Ambient only)");
        }

        private void Update()
        {
            if (timeSystem == null) return;
            
            CheckPhoneTriggers();
            CheckAmbientTriggers();
        }

        private void OnNewDay()
        {
            currentDay++;
            triggeredToday.Clear();
            phoneCheckScheduled = false;
            Debug.Log($"New day started: Day {currentDay}");
        }

        private void CheckPhoneTriggers()
        {
            if (phoneCheckScheduled) return;

            int totalMinutes = timeSystem.GetTotalMinutes();
            int hour = totalMinutes / 60;

            foreach (var trigger in allTriggers.Where(t => t.type == TriggerSO.TriggerType.Phone))
            {
                if (CanTrigger(trigger) && hour >= trigger.startHour && hour < trigger.endHour)
                {
                    ShowPhoneOffer(trigger);
                    phoneCheckScheduled = true;
                    break;
                }
            }
        }

        // NPC triggers are deprecated: use ConversationStarter/ConversationZoneStarter instead.

        private void CheckAmbientTriggers()
        {
            // Check time window for ambient triggers
            int totalMinutes = timeSystem.GetTotalMinutes();
            int hour = totalMinutes / 60;

            foreach (var trigger in allTriggers.Where(t => t.type == TriggerSO.TriggerType.Ambient))
            {
                if (CanTrigger(trigger) && hour >= trigger.startHour && hour < trigger.endHour)
                {
                    ShowAmbientOffer(trigger);
                    break;
                }
            }
        }

        private bool CanTrigger(TriggerSO trigger)
        {
            // Check if already triggered today
            if (triggeredToday.Contains(trigger.triggerId) && !trigger.isRepeatable)
                return false;
            
            // Check if correct day
            if (trigger.activationDay > currentDay)
                return false;
            
            // Check prerequisites
            if (trigger.prerequisiteErrand != null && 
                !errandSystem.IsErrandCompleted(trigger.prerequisiteErrand.errandId))
                return false;
            
            return true;
        }

        private void ShowPhoneOffer(TriggerSO trigger)
        {
            Debug.Log($"Phone call trigger: {trigger.triggerId}");
            ShowOffer(trigger, "Phone Call");
        }

        // NPC offers removed: handled by ConversationStarter on NPCs

        private void ShowAmbientOffer(TriggerSO trigger)
        {
            Debug.Log($"Ambient trigger: {trigger.triggerId}");
            ShowOffer(trigger, "Overheard");
        }

        private void ShowOffer(TriggerSO trigger, string source)
        {
            // Phone triggers: prefer Conversa if available
            if (trigger.type == TriggerSO.TriggerType.Phone && trigger.conversation != null && conversaController != null)
            {
                Debug.Log($"Starting Conversa conversation for trigger: {trigger.triggerId}");
                conversaController.StartConversation(trigger.conversation);
                triggeredToday.Add(trigger.triggerId);
                return;
            }

            if (offerPromptUI != null)
            {
                offerPromptUI.ShowOffer(
                    trigger.offerText,
                    source,
                    trigger.acceptText,
                    trigger.declineText,
                    () => OnAcceptOffer(trigger),
                    () => OnDeclineOffer(trigger)
                );
            }
            else
            {
                Debug.LogWarning("OfferPrompt UI not assigned!");
            }
            
            triggeredToday.Add(trigger.triggerId);
        }

        private void OnAcceptOffer(TriggerSO trigger)
        {
            Debug.Log($"Accepted errand from trigger: {trigger.triggerId}");
            
            // Add errand if specified
            if (errandSystem != null && trigger.errandToOffer != null)
            {
                errandSystem.AddErrand(trigger.errandToOffer);
            }
            
            // Give items if specified
            if (inventorySystem != null && trigger.itemsToGive.Count > 0)
            {
                foreach (var itemReward in trigger.itemsToGive)
                {
                    if (itemReward.item != null)
                    {
                        inventorySystem.AddItem(itemReward.item, itemReward.count);
                        Debug.Log($"Received {itemReward.count}x {itemReward.item.displayName} from {trigger.triggerId}");
                    }
                }
            }
        }

        private void OnDeclineOffer(TriggerSO trigger)
        {
            Debug.Log($"Declined errand from trigger: {trigger.triggerId}");
        }
        
        // Public method to manually trigger an offer (useful for testing)
        public void ManuallyTrigger(string triggerId)
        {
            var trigger = allTriggers.FirstOrDefault(t => t.triggerId == triggerId);
            if (trigger != null)
            {
                Debug.Log($"Manually triggering: {triggerId}");
                if (trigger.type == TriggerSO.TriggerType.Phone && trigger.conversation != null && conversaController != null)
                {
                    conversaController.StartConversation(trigger.conversation);
                    triggeredToday.Add(trigger.triggerId);
                }
                else
                {
                    switch (trigger.type)
                    {
                        case TriggerSO.TriggerType.Phone:
                            ShowPhoneOffer(trigger);
                            break;
                        case TriggerSO.TriggerType.NPC:
                            Debug.LogWarning("[TriggerSystem] Manual NPC trigger ignored. Use ConversationStarter/ConversationZoneStarter.");
                            break;
                        case TriggerSO.TriggerType.Ambient:
                            ShowAmbientOffer(trigger);
                            break;
                    }
                }
            }
        }
    }

    // ScriptRole: Manages trigger conditions, offers errands, and gives items
    // RelatedScripts: ErrandSystem, InventorySystem, OfferPrompt, TimeSystem
    // UsesSO: TriggerSO, ErrandSO, GameEventSO
    // ReceivesFrom: TimeSystem, GameStateManager
    // SendsTo: ErrandSystem, InventorySystem, OfferPrompt UI
}
