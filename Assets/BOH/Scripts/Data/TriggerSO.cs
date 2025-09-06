using UnityEngine;
using System.Collections.Generic;
using Conversa.Runtime;

namespace BOH
{
    [CreateAssetMenu(fileName = "Trigger", menuName = "BOH/Data/Trigger")]
    public class TriggerSO : ScriptableObject
    {
        public enum TriggerType
        {
            NPC,        // Proximity to NPC
            Phone,      // Time-based phone call
            Ambient     // Area/condition based
        }

        [Header("Trigger Info")]
        public string triggerId;
        public TriggerType type = TriggerType.NPC;
        public ErrandSO errandToOffer;
        
        [Header("Items Given on Accept")]
        public List<ItemReward> itemsToGive = new List<ItemReward>();
        
        [Header("Activation")]
        public int activationDay = 1;
        public int startHour = 6;
        public int endHour = 22;
        public bool isRepeatable = false;
        
        [Header("NPC Settings")]
        public string npcName = "Grocer";
        public float triggerRadius = 2f;
        
        [Header("Conversa")]
        [Tooltip("If true and a Conversation is assigned, the trigger starts this Conversa flow instead of the legacy prompt.")]
        public bool useConversa = false;
        [Tooltip("Conversa Conversation asset to run when this trigger fires.")]
        public Conversation conversation;

        [Header("Legacy Prompt (fallback)")]
        [TextArea(3, 5)]
        public string offerText = "Can you help me with something?";
        public string acceptText = "Yes, I'll help";
        public string declineText = "Sorry, not now";
        
        [Header("Preconditions")]
        public int requiredTrustLevel = 0;
        public ErrandSO prerequisiteErrand;
        
        [System.Serializable]
        public class ItemReward
        {
            public ItemSO item;
            public int count = 1;
        }
    }

    // ScriptRole: Trigger configuration for offering errands with item rewards
    // RelatedScripts: TriggerSystem, ErrandSO
}
