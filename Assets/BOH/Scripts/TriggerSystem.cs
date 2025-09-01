using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;

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
        [SerializeField] private Transform playerTransform;
        
        [Header("State")]
        [SerializeField] private List<string> triggeredToday = new List<string>();
        [SerializeField] private int currentDay = 1;
        
        private TimeSystem timeSystem;
        private ErrandSystem errandSystem;
        private InventorySystem inventorySystem; // Add this
        private Dictionary<string, Transform> npcTransforms = new Dictionary<string, Transform>();
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
            timeSystem = FindObjectOfType<TimeSystem>();
            errandSystem = FindObjectOfType<ErrandSystem>();
            inventorySystem = FindObjectOfType<InventorySystem>(); // Add this
            
            // Find all NPCs in scene
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            foreach (var npc in npcs)
            {
                npcTransforms[npc.name] = npc.transform;
                Debug.Log($"Registered NPC: {npc.name}");
            }
            
            // Get player reference if not assigned
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
            }
            
            Debug.Log($"TriggerSystem initialized with {allTriggers.Count} triggers");
        }

        private void Update()
        {
            if (timeSystem == null) return;
            
            CheckPhoneTriggers();
            CheckNPCTriggers();
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
            
            string currentTime = timeSystem.GetTimeString();
            int hour = int.Parse(currentTime.Substring(0, 2));
            
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

        private void CheckNPCTriggers()
        {
            if (playerTransform == null) return;
            
            foreach (var trigger in allTriggers.Where(t => t.type == TriggerSO.TriggerType.NPC))
            {
                if (!CanTrigger(trigger)) continue;
                
                // Check time window
                string currentTime = timeSystem.GetTimeString();
                int hour = int.Parse(currentTime.Substring(0, 2));
                if (hour < trigger.startHour || hour >= trigger.endHour) continue;
                
                if (npcTransforms.TryGetValue(trigger.npcName, out Transform npcTransform))
                {
                    float distance = Vector3.Distance(playerTransform.position, npcTransform.position);
                    if (distance <= trigger.triggerRadius)
                    {
                        ShowNPCOffer(trigger);
                        break;
                    }
                }
            }
        }

        private void CheckAmbientTriggers()
        {
            // Check time window for ambient triggers
            string currentTime = timeSystem.GetTimeString();
            int hour = int.Parse(currentTime.Substring(0, 2));
            
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

        private void ShowNPCOffer(TriggerSO trigger)
        {
            Debug.Log($"NPC trigger: {trigger.triggerId} from {trigger.npcName}");
            ShowOffer(trigger, trigger.npcName);
        }

        private void ShowAmbientOffer(TriggerSO trigger)
        {
            Debug.Log($"Ambient trigger: {trigger.triggerId}");
            ShowOffer(trigger, "Overheard");
        }

        private void ShowOffer(TriggerSO trigger, string source)
        {
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
                switch (trigger.type)
                {
                    case TriggerSO.TriggerType.Phone:
                        ShowPhoneOffer(trigger);
                        break;
                    case TriggerSO.TriggerType.NPC:
                        ShowNPCOffer(trigger);
                        break;
                    case TriggerSO.TriggerType.Ambient:
                        ShowAmbientOffer(trigger);
                        break;
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