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
            [System.Obsolete("Use ConversationStarter/ConversationZoneStarter for NPC interactions")] 
            NPC,        // Deprecated: use ConversationStarter/ConversationZoneStarter
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
        
        [Header("NPC Settings (Deprecated)")]
        [Tooltip("Deprecated: NPC triggers are handled by ConversationStarter or ConversationZoneStarter.")]
        [System.Obsolete("NPC triggers are deprecated; move to ConversationStarter/Zone")] public string npcName = "Grocer";
        [System.Obsolete("NPC triggers are deprecated; move to ConversationStarter/Zone")] public float triggerRadius = 2f;
        
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
